namespace Tutorial10.Models.DTOs;

public class PrescriptionDTO
{
    public PatientDTO Patient { get; set; }
    public DoctorDTO Doctor { get; set; }
    public IEnumerable<MedicamentDTO> Medicaments { get; set; } = new List<MedicamentDTO>();
    public string Date { get; set; } 
    public string DueDate { get; set; } 

}