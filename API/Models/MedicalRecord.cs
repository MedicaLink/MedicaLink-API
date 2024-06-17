namespace API.Models;

public class MedicalRecord
{
    public int Id { get; set; }

    public int PatientId { get; set; } 

    public int AdminId { get; set; }    

    public string? RecordType { get; set; }

    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    public string? FilePath { get; set; }

    public Patient? Patient { get; set; }

    public Admin? Admin { get; set; }
}