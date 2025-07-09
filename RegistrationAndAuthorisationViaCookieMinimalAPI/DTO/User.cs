using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.DTO
{
    public record class User
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string? Email { get; set; }

        [RegularExpression(@"^[A-Za-z!@#$%^&*()_\-+=\[{\]};:'"",<.>/?\\|`~]+$")]
        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
