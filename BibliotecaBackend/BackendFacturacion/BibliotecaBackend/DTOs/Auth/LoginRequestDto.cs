﻿using System.ComponentModel.DataAnnotations;

namespace BibliotecaBackend.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        public bool RecordarMe { get; set; } = false;
    }
}
