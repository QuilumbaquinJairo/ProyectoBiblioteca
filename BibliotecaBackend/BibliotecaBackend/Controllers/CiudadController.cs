using BibliotecaBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ciudades")]
    public class CiudadController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public CiudadController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ciudades = _context.CiudadEntregas
                .OrderBy(c => c.NombreCiudad)
                .Select(c => new
                {
                    c.CodigoCiudad,
                    c.NombreCiudad
                })
                .ToList();

            return Ok(ciudades);
        }
    }
}
