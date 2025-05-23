using Microsoft.EntityFrameworkCore;
using Tutorial11.Models;

namespace Tutorial11.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }

    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
        {
            new Doctor { IdDoctor = 1, FirstName = "James", LastName = "Bond", Email = "james.bond@example.com" },
            new Doctor { IdDoctor = 2, FirstName = "John", LastName = "Watson", Email = "john.watson@example.com" }
        });
        modelBuilder.Entity<Patient>().HasData(
            new Patient { IdPatient = 1, FirstName = "Alice", LastName = "Smith", Birthdate = new DateTime(1985, 5, 20) },
            new Patient { IdPatient = 2, FirstName = "Bob", LastName = "Johnson", Birthdate = new DateTime(1990, 8, 15) }
        );
        modelBuilder.Entity<Medicament>().HasData(
            new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Pain reliever", Type = "Tablet" },
            new Medicament { IdMedicament = 2, Name = "Ibuprofen", Description = "Anti-inflammatory", Type = "Capsule" }
        );
        modelBuilder.Entity<Prescription>().HasData(
            new Prescription
            {
                IdPrescription = 1,
                Date = new DateTime(2023, 1, 10),
                DueDate = new DateTime(2023, 1, 20),
                IdPatient = 1,
                IdDoctor = 1
            },
            new Prescription
            {
                IdPrescription = 2,
                Date = new DateTime(2023, 2, 5),
                DueDate = new DateTime(2023, 2, 15),
                IdPatient = 2,
                IdDoctor = 2
            }
        );
        modelBuilder.Entity<PrescriptionMedicament>().HasData(
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 1,
                Dose = 2,
                Details = "Take after meals"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 1,
                IdMedicament = 2,
                Dose = 1,
                Details = "Before sleep"
            },
            new PrescriptionMedicament
            {
                IdPrescription = 2,
                IdMedicament = 2,
                Dose = null,
                Details = "Optional use"
            }
        );

    }

}