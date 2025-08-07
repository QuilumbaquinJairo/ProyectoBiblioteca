using System.Security.Claims;
using BibliotecaBackend.Models;
using BibliotecaBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaBackend.Controllers
{
    
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegistrarAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message;
                return BadRequest(new { mensaje = ex.Message, detalle = innerMessage });
                
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetPerfil()
        {
            var usuario = new
            {
                NombreUsuario = User.FindFirstValue(ClaimTypes.Name),
                Correo = User.FindFirstValue(ClaimTypes.Email),
                Rol = User.FindFirstValue(ClaimTypes.Role)
            };

            return Ok(usuario);
        }

        [AllowAnonymous]
        [HttpGet("helpchat/disponible")]
        public IActionResult VerificarDisponibilidad([FromServices] ChatWebSocketHandler chatService)
        {
            var disponible = chatService.HayAgentesConectados();
            return Ok(new { disponible });
        }



    }

}
