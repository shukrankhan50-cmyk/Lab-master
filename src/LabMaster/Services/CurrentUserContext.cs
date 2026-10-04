using LabMaster.Models;
namespace LabMaster.Services;
public static class CurrentUserContext
{
 public static CurrentUser? User { get; private set; }
 public static string UserName => User?.UserName ?? "System";
 public static string Role => User?.RoleName ?? "";
 public static void Set(CurrentUser user)=>User=user;
}