namespace LabMaster.Services;
public static class ResultFlagService
{
 public static string Flag(string? value,decimal? low,decimal? high)
 {
  if(!decimal.TryParse(value,out var number)) return "";
  if(low.HasValue && number<low.Value) return "LOW";
  if(high.HasValue && number>high.Value) return "HIGH";
  return "NORMAL";
 }
}