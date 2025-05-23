using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tutorial11.Models;

[PrimaryKey(nameof(IdMedicament), nameof(IdPrescription))]
[Table("Prescription_Medicament")]
public class PrescriptionMedicament
{
    
    [Required]
    [ForeignKey(nameof(Medicament))]
    public int IdMedicament { get; set; }

    [Required]
    [ForeignKey(nameof(Prescription))]
    public int IdPrescription { get; set; }

    public int? Dose { get; set; }

    [Required]
    [MaxLength(100)]
    public string Details { get; set; }

    public Medicament Medicament { get; set; }
    public Prescription Prescription { get; set; }
    
}