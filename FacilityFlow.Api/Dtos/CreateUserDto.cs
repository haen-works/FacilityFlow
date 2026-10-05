using System.ComponentModel.DataAnnotations;

namespace FacilityFlow.Api.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = "";

        [Required]
        [RegularExpression("^(Employee|Technician|Admin)$", ErrorMessage = "Rol Employee, Technician veya Admin olmalıdır.")]

        public string Role { get; set; } = "Employee";

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = "";
    }
}