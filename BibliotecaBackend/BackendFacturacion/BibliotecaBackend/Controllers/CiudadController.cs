using BibliotecaBackend.Data;
using BibliotecaBackend.DTOs.Ciudad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCiudadDto dto)
        {
            var existe = await _context.CiudadEntregas.AnyAsync(c =>
                c.NombreCiudad.ToLower() == dto.NombreCiudad.ToLower());

            if (existe)
                return BadRequest(new { mensaje = "Ya existe una ciudad con ese nombre." });

            var ciudad = new CiudadEntrega
            {
                NombreCiudad = dto.NombreCiudad
            };

            _context.CiudadEntregas.Add(ciudad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = ciudad.CodigoCiudad }, ciudad);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCiudadDto dto)
        {
            var ciudad = await _context.CiudadEntregas.FindAsync(id);
            if (ciudad == null) return NotFound();

            ciudad.NombreCiudad = dto.NombreCiudad;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ciudad = await _context.CiudadEntregas
                .Include(c => c.FacturaCabeceras)
                .FirstOrDefaultAsync(c => c.CodigoCiudad == id);

            if (ciudad == null) return NotFound();

            if (ciudad.FacturaCabeceras.Any())
                return BadRequest(new { mensaje = "No se puede eliminar la ciudad porque está asociada a facturas." });

            _context.CiudadEntregas.Remove(ciudad);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
