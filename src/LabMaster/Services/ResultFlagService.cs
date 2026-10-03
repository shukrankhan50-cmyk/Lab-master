namespace LabMaster.Services;
public static class ResultFlagService
{
 public static string Flag(string? value,string? range)
 {
  if(string.IsNullOrWhiteSpace(value)) return "";
  if(decimal.TryParse(value,out _)) return "";
  return "";
 }
}