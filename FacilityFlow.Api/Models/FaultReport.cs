namespace FacilityFlow.Api.Models
{
    public class FaultReport
    {
        public int Id{ get; set;  }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public string Location { get; set; } = "";

        public string Status { get; set; } = "Open";

        public string Priority { get; set; } = "Medium";

        public int? AssignedTechnicianId { get; set; }

        public int? ReportedById { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    }
}
