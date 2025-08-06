using ContabilidadBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContabilidadBackend.Controllers
{
    
    [ApiController]
    [Route("asientos")]
    public class AsientoContableController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public AsientoContableController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? tipo, [FromQuery] int? referencia)
        {
            var query = _context.AsientoContables.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(a => a.TipoOperacion == tipo);

            if (referencia.HasValue)
                query = query.Where(a => a.Referencia == referencia.Value);

            var result = query
                .OrderByDescending(a => a.Fecha)
                .Select(a => new
                {
                    a.IdAsientoContable,
                    a.Fecha,
                    a.TipoOperacion,
                    a.Referencia,
                    a.CuentaDebito,
                    a.CuentaCredito,
                    a.Monto
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var asiento = _context.AsientoContables
                .FirstOrDefault(a => a.IdAsientoContable == id);

            if (asiento == null)
                return NotFound();

            return Ok(new
            {
                asiento.IdAsientoContable,
                asiento.Fecha,
                asiento.TipoOperacion,
                asiento.Referencia,
                asiento.CuentaDebito,
                asiento.CuentaCredito,
                asiento.Monto
            });
        }
    }
}
