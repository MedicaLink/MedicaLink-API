using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Admin
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public AdminType Type { get; set; }

    public string? Password { get; set; }

    [ForeignKey("HospitalId")]
    public int HospitalId { get; set; }

    public Hospital? Hospital { get; set; }

    public List<MedicalRecord> MedicalRecords { get; set; } = [];
}

public enum AdminType {
    SuperAdmin,
    Admin
}