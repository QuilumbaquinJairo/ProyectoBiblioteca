using BibliotecaBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend.Controllers;

[Authorize]
[ApiController]
[Route("reportes")]
public class ReportesController : ControllerBase
{
    private readonly BibliotecaDbContext _context;

    public ReportesController(BibliotecaDbContext context)
    {
        _context = context;
    }

    [HttpGet("ventas-por-ciudad")]
    public async Task<IActionResult> ObtenerVentasPorCiudad([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var resultado = await _context.FacturaCabeceras
            .Where(f => f.Fecha >= fechaInicio && f.Fecha <= fechaFin)
            .Include(f => f.CodigoCiudadEntregaNavigation)
            .Include(f => f.FacturaDetalles)
            .GroupBy(f => f.CodigoCiudadEntregaNavigation.NombreCiudad)
            .Select(g => new
            {
                Ciudad = g.Key,
                TotalVendido = g.SelectMany(f => f.FacturaDetalles)
                                .Sum(d => d.Cantidad * d.Precio)
            })
            .ToListAsync();

        return Ok(resultado);
    }

    [HttpGet("cliente-articulo")]
    public async Task<IActionResult> ObtenerReporteClienteArticulo([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var facturas = await _context.FacturaCabeceras
            .Where(f => f.Fecha >= fechaInicio && f.Fecha <= fechaFin)
            .Include(f => f.FacturaDetalles)
            .Include(f => f.RucclienteNavigation)
            .Include(f => f.FacturaDetalles)
                .ThenInclude(d => d.CodigoArticuloNavigation)
            .ToListAsync();

        var clientes = facturas
            .Select(f => f.RucclienteNavigation)
            .Distinct()
            .ToList();

        var articulos = facturas
            .SelectMany(f => f.FacturaDetalles)
            .Select(d => d.CodigoArticuloNavigation)
            .Distinct()
            .ToList();

        var resultado = new List<Dictionary<string, object>>();

        foreach (var articulo in articulos)
        {
            var fila = new Dictionary<string, object>
            {
                ["Articulo"] = articulo.NombreArticulo
            };

            foreach (var cliente in clientes)
            {
                var total = facturas
                    .Where(f => f.Ruccliente == cliente.Ruc)
                    .SelectMany(f => f.FacturaDetalles)
                    .Where(d => d.CodigoArticulo == articulo.CodigoArticulo)
                    .Sum(d => d.Cantidad * d.Precio);

                fila[cliente.Ruc] = total;
            }

            resultado.Add(fila);
        }

        return Ok(resultado);
    }

}
