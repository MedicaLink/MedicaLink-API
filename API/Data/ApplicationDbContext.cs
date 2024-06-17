using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<VaccineBrand> VaccineBrands { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
}
