using BibliotecaBackend.Data;
using BibliotecaBackend.DTOs.Factura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("facturas")]
    public class FacturaController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;
        private readonly RabbitMqService _rabbitMq;
        private readonly ILogger<FacturaController> _logger;
        public FacturaController(BibliotecaDbContext context, RabbitMqService rabbitMq, ILogger<FacturaController> logger)
        {
            _context = context;
            _rabbitMq = rabbitMq;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CrearFactura([FromBody] FacturaRequestDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in dto.Detalles)
                {
                    var articulo = await _context.Articulos.FindAsync(item.CodigoArticulo);
                    if (articulo == null)
                        return BadRequest(new { mensaje = $"Artículo {item.CodigoArticulo} no existe." });

                    if (item.Cantidad > articulo.SaldoInventario)
                        return BadRequest(new { mensaje = $"Inventario insuficiente para {articulo.NombreArticulo}. Stock: {articulo.SaldoInventario}" });

                    // Descontar saldo
                    articulo.SaldoInventario -= item.Cantidad;
                }

                var cabecera = new FacturaCabecera
                {
                    Fecha = dto.Fecha,
                    CodigoCiudadEntrega = dto.CodigoCiudadEntrega,
                    Ruccliente = dto.RucCliente
                };

                _context.FacturaCabeceras.Add(cabecera);
                await _context.SaveChangesAsync();

                foreach (var item in dto.Detalles)
                {
                    _context.FacturaDetalles.Add(new FacturaDetalle
                    {
                        NumeroFactura = cabecera.NumeroFactura,
                        CodigoArticulo = item.CodigoArticulo,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Enviar mensaje a RabbitMQ en lugar de registrar asiento contable directamente
                var total = dto.Detalles.Sum(d => d.Cantidad * d.Precio);

                var mensaje = new AsientoContable
                {
                    Fecha = dto.Fecha,
                    TipoOperacion = "Factura",
                    Referencia = cabecera.NumeroFactura,
                    CuentaDebito = "1101-Cuentas por Cobrar",
                    CuentaCredito = "4101-Ventas",
                    Monto = total
                };

                await _rabbitMq.PublishFacturaAsync(mensaje);
                _logger.LogInformation("Mensaje de factura {NumeroFactura} enviado correctamente al broker RabbitMQ.", cabecera.NumeroFactura);

                return Ok(new { cabecera.NumeroFactura });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? rucCliente, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var query = _context.FacturaCabeceras
                .Include(f => f.RucclienteNavigation)
                .Include(f => f.CodigoCiudadEntregaNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(rucCliente))
                query = query.Where(f => f.Ruccliente == rucCliente);

            if (desde.HasValue)
                query = query.Where(f => f.Fecha >= desde.Value.Date);

            if (hasta.HasValue)
                query = query.Where(f => f.Fecha <= hasta.Value.Date);

            var result = await query
                .OrderByDescending(f => f.Fecha)
                .Select(f => new
                {
                    f.NumeroFactura,
                    f.Fecha,
                    Cliente = f.RucclienteNavigation.Ruc,
                    CiudadEntrega = f.CodigoCiudadEntregaNavigation.NombreCiudad,
                    Total = _context.FacturaDetalles
                        .Where(d => d.NumeroFactura == f.NumeroFactura)
                        .Sum(d => d.Cantidad * d.Precio)
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _context.FacturaCabeceras
                .Include(f => f.RucclienteNavigation)
                .Include(f => f.CodigoCiudadEntregaNavigation)
                .Include(f => f.FacturaDetalles)
                    .ThenInclude(d => d.CodigoArticuloNavigation)
                .FirstOrDefaultAsync(f => f.NumeroFactura == id);

            if (factura == null) return NotFound();

            return Ok(new
            {
                factura.NumeroFactura,
                factura.Fecha,
                Cliente = factura.RucclienteNavigation.Nombre,
                Ciudad = factura.CodigoCiudadEntregaNavigation.NombreCiudad,
                Detalles = factura.FacturaDetalles.Select(d => new
                {
                    d.CodigoArticulo,
                    Articulo = d.CodigoArticuloNavigation.NombreArticulo,
                    d.Cantidad,
                    d.Precio,
                    Subtotal = d.Cantidad * d.Precio
                }),
                Total = factura.FacturaDetalles.Sum(d => d.Cantidad * d.Precio)
            });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var factura = await _context.FacturaCabeceras
                .Include(f => f.FacturaDetalles)
                .FirstOrDefaultAsync(f => f.NumeroFactura == id);

            if (factura == null)
                return NotFound();

            _context.FacturaCabeceras.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
