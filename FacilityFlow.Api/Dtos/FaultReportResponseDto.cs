namespace FacilityFlow.Api.Dtos
{
    public class FaultReportResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string Location { get; set; } = "";

        public string Status { get; set; } = "";

        public string Priority { get; set; } = "";

        public int? AssignedTechnicianId { get; set; }

        public int? ReportedById { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}