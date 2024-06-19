using System.ComponentModel.DataAnnotations;

namespace API.Models.FormModels
{
    public class PatientSearchModel
    {
        public string? Query { get; set; }

        public string Type { get; set; } = "NIC";
    }
}
