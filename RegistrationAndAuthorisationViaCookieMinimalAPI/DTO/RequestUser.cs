using System.ComponentModel.DataAnnotations;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.DTO
{
    public class RequestUser
    {
        [Required, StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        public string? Password { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
