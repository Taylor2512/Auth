using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;

namespace Auth.Application.Dto.Request
{
    /// <summary>
    /// Represents a user request.
    /// </summary>
    public record UserRequest
    {
        private string? _userName;

        /// <summary>
        /// Gets or sets the username.
        /// If the username is not set, it defaults to the email address.
        /// </summary>
        public string? UserName
        {
            get => _userName ?? Email;
            set => _userName = value;
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required(ErrorMessage = "El campo 'Email' es requerido.")]
        [EmailAddress(ErrorMessage = "El campo 'Email' debe tener un formato válido de dirección de correo electrónico.")]
        [StringLength(maximumLength: 100, MinimumLength = 8, ErrorMessage = "El campo 'Email' debe tener entre 8 y 100 caracteres.")]
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public required string Password { get; set; }
    }
}
