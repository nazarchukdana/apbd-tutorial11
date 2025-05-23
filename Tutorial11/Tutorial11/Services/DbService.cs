using Microsoft.EntityFrameworkCore;
using Tutorial11.Data;
using Tutorial11.DTOs;
using Tutorial11.Exceptions;
using Tutorial11.Models;

namespace Tutorial11.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<int> AddPrescription(PrescriptionRequestDto dto)
    {
        if (dto.DueDate < dto.Date)
            throw new BadRequestException("Due date must be greater than or equal to Date");
        if (dto.Medicaments.Count > 10)
            throw new BadRequestException("A prescription can include a maximum of 10 medications.");
        if (await _context.Doctors.FindAsync(dto.IdDoctor) == null)
            throw new BadRequestException("Doctor with such id does not exists");
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p =>
                p.IdPatient == dto.Patient.IdPatient &&
                p.FirstName == dto.Patient.FirstName &&
                p.LastName == dto.Patient.LastName &&
                p.Birthdate.Date == dto.Patient.BirthDate.Date);
        if (patient == null)
        {
            if(dto.Patient.IdPatient  > 0)
                throw new BadRequestException("Provided patient Id does not match patient details");
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName = dto.Patient.LastName,
                Birthdate = dto.Patient.BirthDate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var medicamentIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicaments = await _context.Medicaments.Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament).ToListAsync();
        if(existingMedicaments.Count != dto.Medicaments.Count)
            throw new NotFoundException("One or more medicament do not exist");
        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            IdPatient = patient.IdPatient,
            IdDoctor = dto.IdDoctor
        };
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        foreach (var med in dto.Medicaments)
        {
            var pm = new PrescriptionMedicament
            {
                IdMedicament = med.IdMedicament,
                IdPrescription = prescription.IdPrescription,
                Dose = med.Dose,
                Details = med.Description
            };
            _context.PrescriptionMedicaments.Add(pm);
        }
        await _context.SaveChangesAsync();
        return prescription.IdPrescription;
    }

    public async Task<PrescriptionDto> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await _context.Prescriptions.FirstOrDefaultAsync(p => p.IdPrescription == id);
        if(prescription == null)
            throw new NotFoundException("Prescription not found");
        return new PrescriptionDto
        {
            IdPrescription = prescription.IdPrescription,
            Date = prescription.Date,
            DueDate = prescription.DueDate,
            IdDoctor = prescription.IdDoctor,
            IdPatient = prescription.IdPatient,
        };
    }

    public async Task<PatientDetailsDto> GetPatientDetailsByIdAsync(int id)
    {
        var patient = await _context.Patients.Where(p => p.IdPatient == id)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync();
        if(patient == null) throw new NotFoundException("Patient not found");
        return new PatientDetailsDto
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(pr => pr.DueDate)
                .Select(pr => new PrescriptionDetailsDto
                {
                    IdPrescription = pr.IdPrescription,
                    Date = pr.Date,
                    DueDate = pr.DueDate,
                    Doctor = new DoctorDto
                    {
                        IdDoctor = pr.Doctor.IdDoctor,
                        FirstName = pr.Doctor.FirstName,
                    },
                    Medicaments = pr.PrescriptionMedicaments.Select(pm => new MedicamentDetailsDto
                    {
                        IdMedicament = pm.IdMedicament,
                        Name = pm.Medicament.Name,
                        Dose = pm.Dose,
                        Description = pm.Medicament.Description,
                    }).ToList()
                }).ToList()
        };
    }
}