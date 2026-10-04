namespace LabMaster.Services;

public static class PermissionService
{
    public static bool Can(string role, string permission)
    {
        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            return true;

        return permission switch
        {
            "Patients" => role is "Reception" or "Lab Manager" or "Technician",
            "Orders" => role is "Reception" or "Lab Manager" or "Technician",
            "Billing" => role is "Reception" or "Lab Manager",
            "Results" => role is "Technician" or "Lab Manager",
            "Reports" => role is "Technician" or "Lab Manager",
            "QC" => role is "Technician" or "Lab Manager",
            "Inventory" => role is "Technician" or "Lab Manager",
            "MasterData" => role is "Lab Manager",
            "Finance" => role is "Lab Manager",
            "Administration" => role is "Lab Manager",
            _ => false
        };
    }
}