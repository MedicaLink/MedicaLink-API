namespace API.Models;

public class Vaccination
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int HospitalId { get; set; }

    public int VaccineBrandId { get; set; }

    public DateOnly DateOfVaccination { get; set; }

    public string? Dose { get; set; }

    public Patient? Patient { get; set; }

    public Hospital? Hospital { get; set; }

    public VaccineBrand? VaccineBrand { get; set; }
}