using System.ComponentModel.DataAnnotations;

namespace API.Models.FormModels;

public class VaccinationModel
{
    [Required(ErrorMessage = "PatientId is required.")]
    public int PatientId { get; set; }
    public int HospitalId { get; set; }
    public int VaccineBrandId { get; set; }
    public DateOnly DateOfVaccination { get; set; }
    public string? Dose { get; set; }
    
    
}