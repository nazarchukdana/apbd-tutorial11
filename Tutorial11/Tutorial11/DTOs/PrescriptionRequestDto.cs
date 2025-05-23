namespace Tutorial11.DTOs;

public class PrescriptionRequestDto
{
    public PatientDto Patient { get; set; }
    public int  IdDoctor { get; set; }
    public List<PrescriptionMedicamentDto> Medicaments { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}