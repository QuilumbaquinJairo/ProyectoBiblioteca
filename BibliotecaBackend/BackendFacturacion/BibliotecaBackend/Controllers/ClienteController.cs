using BibliotecaBackend.Data;
using BibliotecaBackend.DTOs.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly BibliotecaDbContext _context;

        public ClienteController(BibliotecaDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetClientes([FromQuery] string? ruc)
        {
            if (!string.IsNullOrEmpty(ruc))
            {
                var cliente = await _context.Clientes.FindAsync(ruc);
                if (cliente == null)
                    return NotFound(new { mensaje = "Cliente no encontrado." });

                return Ok(cliente);
            }

            var clientes = await _context.Clientes.ToListAsync();
            return Ok(clientes);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] ClienteRequestDto dto)
        {
            if (_context.Clientes.Any(c => c.Ruc == dto.Ruc))
                return BadRequest(new { mensaje = "Ya existe un cliente con ese RUC." });

            var cliente = new Cliente
            {
                Ruc = dto.Ruc,
                Nombre = dto.Nombre,
                Direccion = dto.Direccion
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{ruc}")]
        public async Task<IActionResult> ActualizarCliente(string ruc, [FromBody] ClienteUpdateDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(ruc);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            cliente.Nombre = dto.Nombre;
            cliente.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return Ok(cliente);
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{ruc}")]
        public async Task<IActionResult> EliminarCliente(string ruc)
        {
            var cliente = await _context.Clientes.FindAsync(ruc);
            if (cliente == null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
