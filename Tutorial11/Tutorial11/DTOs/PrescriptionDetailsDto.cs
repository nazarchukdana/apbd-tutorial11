namespace Tutorial11.DTOs;

public class PrescriptionDetailsDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDetailsDto> Medicaments { get; set; } = new();
    public DoctorDto Doctor { get; set; }
}