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

        public FacturaController(BibliotecaDbContext context)
        {
            _context = context;
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

                // Asiento contable manual
                var total = dto.Detalles.Sum(d => d.Cantidad * d.Precio);
                _context.AsientoContables.Add(new AsientoContable
                {
                    Fecha = dto.Fecha,
                    TipoOperacion = "Factura",
                    Referencia = cabecera.NumeroFactura,
                    CuentaDebito = "1101-Cuentas por Cobrar",
                    CuentaCredito = "4101-Ventas",
                    Monto = total
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { cabecera.NumeroFactura });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
            }
        }

    }
}
