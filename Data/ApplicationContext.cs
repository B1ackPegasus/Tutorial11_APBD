using Microsoft.EntityFrameworkCore;
using Tutorial10.Models;

namespace Tutorial10.Data;

public class ApplicationContext : DbContext
{
    protected ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Prescription_Medicament> PrescriptionMedicaments { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Patient>().HasData(new List<Patient>()
            {
                new() { IdPatient = 1, FirstName =
                    "John", LastName =
                    "Doe", BirthDate = new DateOnly(1987, 10, 12)},
                new() { IdPatient = 2, FirstName =
                    "Ann", LastName =
                    "Smith", BirthDate = new DateOnly(1989, 3, 1)},
                new() { IdPatient = 3, FirstName =
                    "Jack", LastName =
                    "Taylor", BirthDate = new DateOnly(1990, 11, 3)}
            });
        
        modelBuilder.Entity<Doctor>(e =>
        {
            e.HasData(new List<Doctor>()
            {
                new() { IdDoctor = 1, FirstName =
                    "John", LastName =
                    "Kowalski", Email = "jkowalski@gmail.com"},
                new() { IdDoctor = 2, FirstName =
                    "Johnny", LastName =
                    "Bond", Email = "bond@gmail.com"},
                new() { IdDoctor = 3, FirstName =
                    "Kyle", LastName =
                    "Mickiewicz", Email = "mick@gmail.com"}
            });
        });
        
        modelBuilder.Entity<Medicament>(e =>
        {
            e.HasData(new List<Medicament>()
            {
                new() { IdMedicament = 1, Name =
                    "Nimesil", Description = 
                    "Good against pain", Type = "Safe"},
                new() { IdMedicament = 2, Name =
                        "Strepsils", Description = 
                        "Good against sore throat", Type = "Safe"},
                new() { IdMedicament = 3, Name =
                        "Vitamin C", Description = 
                        "Good", Type = "Safe"}
            });
        });
        
        modelBuilder.Entity<Prescription>(e =>
        {
            e.HasData(new List<Prescription>()
            {
                new() { IdPrescription = 1, Date = 
                    new DateOnly(2020, 12,12), DueDate = 
                    new DateOnly(2022, 1, 1), IdPatient = 1,
                    IdDoctor = 1 },
                new() {IdPrescription = 2, Date = 
                        new DateOnly(2020, 11,11), DueDate = 
                        new DateOnly(2022, 1, 1), IdPatient = 2,
                    IdDoctor = 3},
                new() { IdPrescription = 3, Date = 
                        new DateOnly(2021, 10,12), DueDate = 
                        new DateOnly(2032, 1, 10), IdPatient = 3,
                    IdDoctor = 1}
            });
        });
        
        modelBuilder.Entity<Prescription_Medicament>(e =>
        {
            e.HasData(new List<Prescription_Medicament>()
            {
                new() { IdMedicament = 3, IdPrescription = 1, Dose = 1, Details = "once a day"},
                new() {IdMedicament = 2, IdPrescription = 1, Details = "every hour for a week"},
                new() { IdMedicament = 1, IdPrescription = 2, Details = "in case of pain"},
                new() { IdMedicament = 1, IdPrescription = 3, Details = "in case of pain"}
            });
        });
        
    }
}