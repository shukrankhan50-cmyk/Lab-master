using LabMaster.Models;

namespace LabMaster.Services;

public static class CurrentUserContext
{
    public static CurrentUser? User { get; private set; }
    public static string UserName => User?.UserName ?? "System";
    public static string Role => User?.RoleName ?? "";
    public static void Set(CurrentUser user) => User = user;
}

public record CurrentUser(int UserId, string UserName, string DisplayName, string RoleName, bool RequiresPasswordChange = false);
