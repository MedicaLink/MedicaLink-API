using System;
using API.Models;
using Bogus;
using Microsoft.AspNetCore.Identity;

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
            var faker = new Faker<Hospital>()
                .RuleFor(h => h.Name, f => f.Name.FullName())
                .RuleFor(h => h.Address, f => f.Address.FullAddress())
                .RuleFor(h => h.Type, f => f.Company.CompanyName())
                .RuleFor(h => h.Branch, f => f.Commerce.Department());

            var hospitals = faker.Generate(10);
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

        if (!_context.Patients.Any())
        {
            var hospitals = _context.Hospitals.ToList();
            var referenceNumbers = new HashSet<string>(); // Keeps track of added reference numbers

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
                .RuleFor(p => p.RegisteredBy, f => f.PickRandom(hospitals).Id)
                .RuleFor(p => p.Password, f => _passwordHasher.HashPassword(null,"password"));

            var patients = faker.Generate(10);
            _context.Patients.AddRange(patients);
            _context.SaveChanges();
        }
    }
}
