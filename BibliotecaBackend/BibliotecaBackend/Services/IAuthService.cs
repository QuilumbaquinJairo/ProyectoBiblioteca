using BibliotecaBackend.DTOs.Auth;
using BibliotecaBackend.Models;

namespace BibliotecaBackend.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegistrarAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }

}
