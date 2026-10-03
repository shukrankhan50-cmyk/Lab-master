namespace LabMaster.Models;
public sealed class PatientSearchResult
{
 public int PatientId { get; set; }
 public string MRNumber { get; set; } = "";
 public string PatientName { get; set; } = "";
 public string Gender { get; set; } = "";
 public string? Phone { get; set; }
}