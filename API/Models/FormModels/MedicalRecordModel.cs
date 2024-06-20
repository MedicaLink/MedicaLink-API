namespace API.Models.FormModels
{
    public class MedicalRecordModel
    {
        public int PatientId { get; set; }
    }

    public class MedicalRecordSearchModel
    {
        public int PatientId { get; set; }

        public string? Query { get; set; }

        public string? Type { get; set; }

    }
}
