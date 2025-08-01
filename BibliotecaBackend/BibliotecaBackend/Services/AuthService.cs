using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BibliotecaBackend.Data;
using BibliotecaBackend.DTOs.Auth;
using BibliotecaBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly BibliotecaDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public AuthService(BibliotecaDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponse> RegistrarAsync(RegisterRequest request)
        {
            if (_context.Usuarios.Any(u => u.Correo == request.Correo))
                throw new Exception("El correo ya está registrado.");

            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Correo = request.Correo,
                Rol = request.Rol
            };

            usuario.ContrasenaHash = _hasher.HashPassword(usuario, request.Contrasenia);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return GenerarToken(usuario);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == request.Correo);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, request.Contrasenia);
            if (resultado == PasswordVerificationResult.Failed)
                throw new Exception("Contraseña incorrecta.");

            return GenerarToken(usuario);
        }

        private AuthResponse GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                NombreUsuario = usuario.NombreUsuario,
                Correo = usuario.Correo,
                Rol = usuario.Rol
            };
        }
    }
}
