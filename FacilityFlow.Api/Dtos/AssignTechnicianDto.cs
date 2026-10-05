using System.ComponentModel.DataAnnotations;

namespace FacilityFlow.Api.Dtos
{
            public class AssingTechnicianDto
            {
                [Range(1, int.MaxValue,
                    ErrorMessage = "Teknik personel ID'si pozitif olmalıdır.")]

                    public int TechnicianId { get; set; }   
            }
}