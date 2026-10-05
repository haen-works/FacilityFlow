using System.ComponentModel.DataAnnotations;

namespace FacilityFlow.Api.Dtos
{
    public class UpdateFaultReportStatusDto
    {
        [Required]
        [RegularExpression(
            "^(Open|InProgress|Resolved)$",
            ErrorMessage = "Durum Open, InProgress veya Resolved olmalıdır.")]
        public string Status { get; set; } = "";
    }
}