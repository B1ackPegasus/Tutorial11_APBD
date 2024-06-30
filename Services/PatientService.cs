using System.Runtime.InteropServices.JavaScript;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Tutorial10.Data;
using Tutorial10.Helpers;
using Tutorial10.Models;
using Tutorial10.Models.DTOs;

namespace Tutorial10.Services;

public class PatientService : IPatientService
{
    private readonly ApplicationContext _applicationContext;

    public PatientService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<bool> AddPatientIfNotExist(PatientDTO patient)
    {
        var isPatientExist = await _applicationContext.Patients.AnyAsync(p => p.IdPatient == patient.IdPatient);
        if (!isPatientExist)
        {
            await _applicationContext.Patients.AddAsync(new Patient()
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = DateOnly.Parse(patient.BirthDate)
            });
            await _applicationContext.SaveChangesAsync();
            return false;
        }

        return true;
    }

    public async Task<bool> CheckMedicaments(IEnumerable<MedicamentDTO> medicaments)
    {
        foreach (var medicament in medicaments)
        {
            if (!await _applicationContext.Medicaments.AnyAsync(m => m.IdMedicament == medicament.IdMedicament))
            {
                return false;
            }
        }

        if (medicaments.Count() > 10)
        {
            return false;
        }

        return true;
    }

    public bool CheckDates(DateOnly Date, DateOnly DueDate)
    {
        if (DueDate >= Date)
        {
            return true;
        }

        return false;
    }

    public async Task<bool> AddPrescription(PrescriptionDTO prescriptionDto)
    {
        
        var prescription = await _applicationContext.Prescriptions.AddAsync(new Prescription()
        {
            Date = DateOnly.Parse(prescriptionDto.Date),
            DueDate = DateOnly.Parse(prescriptionDto.DueDate),
            IdPatient = prescriptionDto.Patient.IdPatient,
            IdDoctor = prescriptionDto.Doctor.IdDoctor
        });
        await _applicationContext.SaveChangesAsync();
        
        foreach (var med in prescriptionDto.Medicaments)
        {
            await _applicationContext.PrescriptionMedicaments.AddAsync(new Prescription_Medicament()
            {
                IdMedicament = med.IdMedicament,
                IdPrescription = prescription.Entity.IdPrescription,
                Details = med.Details,
                Dose = med.Dose
            });
        }  
            
        
        await _applicationContext.SaveChangesAsync();
        return true;
    }

    public async Task<Patient> GetPatient(int id)
    {
        var patient = await _applicationContext.Patients.Where(patient => patient.IdPatient == id).FirstAsync();
        return patient;
    }

    public async Task AddUser(User user)
    {
        await _applicationContext.Users.AddAsync(user);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task<User?> GetUser(LoggingRequest loggingRequest)
    {
        var user = await _applicationContext.Users.Where(u => u.Username == loggingRequest.Username).FirstOrDefaultAsync();
        return user;
    }

    public async Task<string> NewRefreshToken(User user)
    {
        user.RefreshToken = SecurityHelpers.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        await _applicationContext.SaveChangesAsync();
        return user.RefreshToken;
    }

    public async Task<User?> GetUserByRefreshToken(RefreshTokenRequest refreshToken)
    {
        return await _applicationContext.Users.Where(u => u.RefreshToken == refreshToken.RefreshToken).FirstOrDefaultAsync();
    }
}