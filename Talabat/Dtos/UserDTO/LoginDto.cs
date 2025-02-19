using System.ComponentModel.DataAnnotations;

namespace Talabat.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
