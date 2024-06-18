namespace API.Models;
public class Hospital
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Address { get; set; }

    public string? Branch { get; set; }

    public List<Vaccination> Vaccinations { get; set; } = [];

    public List<Admin> Admins { get; set; } = [];
}