using System.ComponentModel.DataAnnotations;

namespace Talabat.Dtos
{
    public class RegisterDto
    {
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,15}$",
            ErrorMessage = "Password must have 1 uppercase, 1 lowercase, 1 number, 1 non alphanumeric and at least 8 characters")]

        public required string Password { get; set; }

    }
}
