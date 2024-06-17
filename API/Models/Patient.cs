namespace API.Models;

public class Patient
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public BloodGroup BloodGroup  { get; set; }

    public float Height { get; set; }

    public float Weight { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? Nic { get; set; }

    public string? Address { get; set; }

    public Gender Gender { get; set; }

    public string? ContactNumber { get; set; }

    public string? Password { get; set; }

    public int RegisteredBy { get; set; }

    public List<Vaccination> Vaccinations { get; set; } = [];

    public List<MedicalRecord> MedicalRecords { get; set; } = [];
}


public enum BloodGroup{
    A,
    B,
    AB,
    O
}

public enum Gender{
    Male,
    Female
}