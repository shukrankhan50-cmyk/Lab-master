namespace LabMaster.Models;
public sealed class TestMasterItem
{
 public int TestId {get;set;}
 public string TestCode {get;set;}="";
 public string TestName {get;set;}="";
 public int DepartmentId {get;set;}
 public string DepartmentName {get;set;}="";
 public string? SampleType {get;set;}
 public string? Unit {get;set;}
 public string? ReferenceRange {get;set;}
 public decimal Price {get;set;}
 public bool IsActive {get;set;}
}