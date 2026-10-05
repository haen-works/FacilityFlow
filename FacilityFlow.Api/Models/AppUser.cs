using System.Text.Json.Serialization;

namespace FacilityFlow.Api.Models
{
    public class AppUser
    {
        public int Id{ get; set; }

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Role { get; set; } = "Employee";

        [JsonIgnore]
        public string PasswordHash { get; set; }= "";
    }
}