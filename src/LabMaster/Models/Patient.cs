namespace LabMaster.Models;

public sealed class Patient
{
    public int PatientId { get; set; }
    public string MRNumber { get; set; } = "";
    public string PatientName { get; set; } = "";
    public string? FatherName { get; set; }
    public string Gender { get; set; } = "Male";
    public DateTime? DateOfBirth { get; set; }
    public int? AgeYears { get; set; }
    public int? AgeMonths { get; set; }
    public int? AgeDays { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? ReferringDoctor { get; set; }
}