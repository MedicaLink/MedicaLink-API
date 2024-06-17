namespace API.Models;

public class Vaccine
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<VaccineBrand> VaccineBrands { get; set; } = [];
}