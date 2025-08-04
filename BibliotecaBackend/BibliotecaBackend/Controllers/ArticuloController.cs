using BibliotecaBackend.Data;
using BibliotecaBackend.DTOs.Articulo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("articulos")]
    public class ArticuloController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public ArticuloController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloDto>>> GetAll()
        {
            var articulos = _context.Articulos
                .Select(a => new ArticuloDto
                {
                    CodigoArticulo = a.CodigoArticulo,
                    NombreArticulo = a.NombreArticulo,
                    SaldoInventario = a.SaldoInventario,
                    Precio = a.Precio
                })
                .ToList();

            return Ok(articulos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ArticuloDto>> GetById(int codigo)
        {
            var articulo = await _context.Articulos.FindAsync(codigo);
            if (articulo == null) return NotFound();

            return Ok(new ArticuloDto
            {
                CodigoArticulo = articulo.CodigoArticulo,
                NombreArticulo = articulo.NombreArticulo,
                SaldoInventario = articulo.SaldoInventario,
                Precio = articulo.Precio
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateArticuloDto dto)
        {
            var existe = await _context.Articulos.AnyAsync(a =>
                a.NombreArticulo.ToLower() == dto.NombreArticulo.ToLower());

            if (existe)
                return BadRequest(new { mensaje = "Ya existe un artículo con ese nombre." });

            var articulo = new Articulo
            {
                NombreArticulo = dto.NombreArticulo,
                SaldoInventario = dto.SaldoInventario,
                Precio = dto.Precio
            };

            _context.Articulos.Add(articulo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { codigo = articulo.CodigoArticulo }, articulo);
        }


        [HttpPut("{codigo}")]
        public async Task<IActionResult> Update(int codigo, UpdateArticuloDto dto)
        {
            var articulo = await _context.Articulos.FindAsync(codigo);
            if (articulo == null) return NotFound();

            articulo.NombreArticulo = dto.NombreArticulo;
            articulo.SaldoInventario = dto.SaldoInventario;
            articulo.Precio = dto.Precio;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(int codigo)
        {
            var articulo = await _context.Articulos.FindAsync(codigo);
            if (articulo == null) return NotFound();

            _context.Articulos.Remove(articulo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
