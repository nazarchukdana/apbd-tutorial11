using Tutorial11.DTOs;

namespace Tutorial11.Services;

public interface IDbService
{
    Task<int> AddPrescription(PrescriptionRequestDto prescription);
    Task<PrescriptionDto> GetPrescriptionByIdAsync(int id);
    Task<PatientDetailsDto> GetPatientDetailsByIdAsync(int id);
}