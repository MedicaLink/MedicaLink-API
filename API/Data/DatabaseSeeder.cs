﻿using System;
using API.Models;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace API.Data;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    private readonly IPasswordHasher<Admin> _passwordHasher;

    public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher<Admin> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public void Seed()
    {
        if(!_context.Hospitals.Any())
        {
            var images = new string[] { "h1.jpg", "h2.png", "h3.webp", "h4.webp", "h5.webp", "h6.jpg", "h7.jpg", "h8.jpg", "h9.jpg", "h10.jpg" };

            var faker = new Faker<Hospital>()
                .RuleFor(h => h.Name, f => f.Name.FullName())
                .RuleFor(h => h.Address, f => f.Address.FullAddress())
                .RuleFor(h => h.Type, f => f.Company.CompanyName())
                .RuleFor(h => h.Branch, f => f.Commerce.Department());

            var hospitals = faker.Generate(10);
            hospitals.ForEach(async h =>
            {
                h.LogoImage = $"/public/hospitals/{images[hospitals.IndexOf(h)]}";
            });

            _context.Hospitals.AddRange(hospitals);
            _context.SaveChanges();
        }

        if (!_context.Admins.Any())
        {
            var hospitals = _context.Hospitals.ToList();

            var faker = new Faker<Admin>()
                .RuleFor(a => a.Name, f => f.Name.FullName())
                .RuleFor(a => a.Email, f => f.Internet.Email())
                .RuleFor(a => a.Type, f => f.PickRandom<AdminType>())
                .RuleFor(a => a.HospitalId, f => f.PickRandom(hospitals).Id)
                .RuleFor(a => a.Password, f => _passwordHasher.HashPassword(null, "password"));

            var admins = faker.Generate(10);
            _context.Admins.AddRange(admins);
            _context.SaveChanges();
        }

        if (!_context.Doctors.Any())
        {
            var admins = _context.Admins.Where(a => a.Type == AdminType.Admin).ToList();

            var faker = new Faker<Doctor>()
                .RuleFor(d => d.Name, f => f.Name.FullName())
                .RuleFor(d => d.DateOfBirth, f => f.Date.BetweenDateOnly(new DateOnly(1950, 12, 31), new DateOnly(2024, 5, 30)))
                .RuleFor(d => d.Title, f => f.Person.Company.Bs);

            var doctors = new List<Doctor>();
            admins.ForEach(a =>
            {
                var doctor = faker.Generate(1)[0];
                doctor.AdminId = a.Id;
                doctors.Add(doctor);
            });

            _context.Doctors.AddRange(doctors);
            _context.SaveChanges();
        }

        if (!_context.Patients.Any())
        {
            var admins = _context.Admins.ToList();
            var referenceNumbers = new HashSet<string>(); // Keeps track of added reference numbers
            var images = new string[] { "p1.jpg", "p2.jpg", "p3.jpg", "p4.jpg", "p5.jpg", "p6.jpg", "p7.jpg", "p8.jpg", "p9.jpg", "p10.webp" };

            var faker = new Faker<Patient>()
                .RuleFor(p => p.Nic, f =>
                {
                    string number;
                    do
                    {
                        number = f.Random.Replace("############");
                    } while (!referenceNumbers.Add(number));
                    return number;
                })
                .RuleFor(p => p.Name, f => f.Name.FullName())
                .RuleFor(p => p.Address, f => f.Address.FullAddress())
                .RuleFor(p => p.BloodGroup, f => f.PickRandom<BloodGroup>())
                .RuleFor(p => p.DateOfBirth, f => f.Date.BetweenDateOnly(new DateOnly(1950, 12, 31), new DateOnly(2024, 5, 30)))
                .RuleFor(p => p.Height, f => f.Random.Float(4, 6.10f))
                .RuleFor(p => p.Weight, f => f.Random.Float(15, 100))
                .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
                .RuleFor(p => p.ContactNumber, f => f.Person.Phone)
                .RuleFor(p => p.RegisteredBy, f => f.PickRandom(admins).Id)
                .RuleFor(p => p.Password, f => _passwordHasher.HashPassword(null,"password"));

            var patients = faker.Generate(10);
            patients.ForEach(async p =>
            {
                p.ProfileImage = $"/public/profile/{images[patients.IndexOf(p)]}";
            });

            _context.Patients.AddRange(patients);
            _context.SaveChanges();
        }

        if (!_context.Vaccines.Any())
        {
            var faker = new Faker<Vaccine>()
                .RuleFor(v => v.Name, f => f.Name.FullName());

            var vaccines = faker.Generate(10);
            _context.Vaccines.AddRange(vaccines);
            _context.SaveChanges();
        }

        if (!_context.VaccineBrands.Any())
        {
            var vaccines = _context.Vaccines.ToList();

            var faker = new Faker<VaccineBrand>()
                .RuleFor(v => v.BrandName, f => f.Name.FirstName())
                .RuleFor(v => v.VaccineId, f => f.PickRandom(vaccines).Id);

            var vaccineBrands = faker.Generate(10);
            _context.VaccineBrands.AddRange(vaccineBrands);
            _context.SaveChanges();
        }

        if (!_context.Vaccinations.Any())
        {
            var patients = _context.Patients.ToList();
            var hospitals = _context.Hospitals.ToList();
            var vaccineBrands = _context.VaccineBrands.ToList();

            var faker = new Faker<Vaccination>()
                .RuleFor(v => v.PatientId, f => f.PickRandom(patients).Id)
                .RuleFor(v => v.HospitalId, f => f.PickRandom(hospitals).Id)
                .RuleFor(v => v.VaccineBrandId, f => f.PickRandom(vaccineBrands).Id)
                .RuleFor(v => v.DateOfVaccination,
                    f => f.Date.BetweenDateOnly(new DateOnly(2020, 01, 01), new DateOnly(2024, 12, 31)))
                .RuleFor(v => v.Dose, f => f.Name.Suffix());

            var vaccinations = faker.Generate(60);
            _context.Vaccinations.AddRange(vaccinations);
            _context.SaveChanges();
        }

        if (!_context.MedicalRecords.Any())
        {
            var patients = _context.Patients.ToList();
            var admins = _context.Admins.ToList();

            var faker = new Faker<MedicalRecord>()
                .RuleFor(m => m.PatientId, f => f.PickRandom(patients).Id)
                .RuleFor(m => m.AdminId, f => f.PickRandom(admins).Id)
                .RuleFor(m => m.RecordType, f => f.Name.FirstName())
                .RuleFor(m => m.Date,
                    f => f.Date.BetweenDateOnly(new DateOnly(2020, 01, 01), new DateOnly(2024, 12, 31)))
                .RuleFor(m => m.FilePath, f => f.Image.PicsumUrl(50, 50));

            var medicalRecords = faker.Generate(60);
            _context.MedicalRecords.AddRange(medicalRecords);
            _context.SaveChanges();
        }
    }

    private async Task<string> GetRandomImage(int width = 200, int height = 300)
    {
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync($"https://picsum.photos/{width}/{height}");
        string uri = (response != null)? response.RequestMessage.RequestUri.ToString() : "";

        return uri;
    }
}
