using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

    public string? ProfileImage {  get; set; }

    public DateTime RegisteredDate { get; set; }

    [ForeignKey(nameof(Admin))]
    public int RegisteredBy { get; set; }

    public Admin Admin { get; set; }

    public List<Vaccination> Vaccinations { get; set; } = [];

    public List<MedicalRecord> MedicalRecords { get; set; } = [];

    public int getAge()
    {
        DateTime currentDate = DateTime.Now;
        DateTime dob = new DateTime(DateOfBirth.Year,DateOfBirth.Month,DateOfBirth.Day);

        Console.WriteLine(currentDate);

        for(int i = 1; ; i++)
        {
            if(dob.AddYears(i) > currentDate)
            {
                return i - 1;
            }
        }
    }
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