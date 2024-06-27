using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(patient =>
        {
            patient.Property(p => p.RegisteredDate)
                .HasDefaultValue(DateTime.Now);
        });
    }

    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<VaccineBrand> VaccineBrands { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
}
