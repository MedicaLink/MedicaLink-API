namespace API.Models;

public class VaccineBrand
{   
    public int Id { get; set; }

    public string? BrandName { get; set; }

    public int VaccineId { get; set; }

    public Vaccine? Vaccine { get; set; }

    public List<Vaccination> Vaccinations { get; set; } = [];
}