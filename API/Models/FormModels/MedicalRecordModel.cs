namespace API.Models.FormModels
{
    public class MedicalRecordsModel
    {
        public int PatientId { get; set; }
        //public int AdminId { get; set; }
        public string? RecordType { get; set; }
        public string? Description { get; set; }
        public DateOnly Date { get; set; }
        //public string? FilePath { get; set; }
    }

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
