using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutorial10.Models;

[Table("prescriptions")]
public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly DueDate { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }

    [ForeignKey(nameof(IdPatient))]
    public Patient Patient { get; set; } = null!;
    [ForeignKey(nameof(IdDoctor))]
    public Doctor Doctor { get; set; } = null!;
    public ICollection<Prescription_Medicament> PrescriptionMedicaments { get; set; } =
        new HashSet<Prescription_Medicament>();
}