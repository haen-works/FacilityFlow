using System.ComponentModel.DataAnnotations;

namespace FacilityFlow.Api.Dtos
{
        public class CreateFaultReportDto
        {
            [Required]
            [StringLength(150)]
            public string Title { get; set; } = "";

            [Required]
            [StringLength(2000)]
            public string Description { get; set; } = "";

            [Required]
            [StringLength(200)]
            public string Location { get; set; } = "";

            [Required]
            [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Öncelik Low, Medium, High, veya Critical olmalıdır.")]

            public string Priority{ get; set; } = "Medium";
            
        } 
}