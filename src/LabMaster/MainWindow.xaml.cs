using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class MainWindow : Window
{
    readonly DashboardService dashboard = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await RefreshDashboardAsync();
    }

    async Task RefreshDashboardAsync()
    {
        try
        {
            var s = await dashboard.GetAsync();
            PatientsCount.Text = s.TodayPatients.ToString();
            PendingCount.Text = s.PendingTests.ToString();
            ReadyCount.Text = s.ReportsReady.ToString();
            CollectionCount.Text = $"Rs. {s.TodayCollection:N0}";
        }
        catch { }
    }

    bool Allowed(string permission)
    {
        if (PermissionService.Can(CurrentUserContext.Role, permission)) return true;
        MessageBox.Show("You do not have permission to open this module.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private async void Dashboard_Click(object sender, RoutedEventArgs e) => await RefreshDashboardAsync();
    private void PatientRegistration_Click(object sender, RoutedEventArgs e) { if (Allowed("Patients")) new PatientRegistrationWindow { Owner = this }.ShowDialog(); }
    private void PatientSearch_Click(object sender, RoutedEventArgs e) { if (Allowed("Patients")) new PatientSearchWindow { Owner = this }.ShowDialog(); }
    private void AcceptTest_Click(object sender, RoutedEventArgs e) { if (!Allowed("Orders")) return; new AcceptTestWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Billing_Click(object sender, RoutedEventArgs e) { if (!Allowed("Billing")) return; new BillingWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void ResultEntry_Click(object sender, RoutedEventArgs e) { if (!Allowed("Results")) return; new ResultEntryWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Reports_Click(object sender, RoutedEventArgs e) { if (!Allowed("Reports")) return; new ReportWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Departments_Click(object sender, RoutedEventArgs e) { if (Allowed("MasterData")) new DepartmentWindow { Owner = this }.ShowDialog(); }
    private void Profile_Click(object sender, RoutedEventArgs e) { if (Allowed("MasterData")) new ProfileWindow { Owner = this }.ShowDialog(); }
    private void TestMaster_Click(object sender, RoutedEventArgs e) { if (Allowed("MasterData")) new TestMasterWindow { Owner = this }.ShowDialog(); }
    private void User_Click(object sender, RoutedEventArgs e) { if (Allowed("Administration")) new UserWindow { Owner = this }.ShowDialog(); }
    private void Backup_Click(object sender, RoutedEventArgs e) { if (Allowed("Administration")) new BackupWindow { Owner = this }.ShowDialog(); }
    private void Expense_Click(object sender, RoutedEventArgs e) { if (!Allowed("Finance")) return; new ExpenseWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Financial_Click(object sender, RoutedEventArgs e) { if (Allowed("Finance")) new FinancialReportWindow { Owner = this }.ShowDialog(); }
    private void Settings_Click(object sender, RoutedEventArgs e) { if (Allowed("Administration")) new SettingsWindow { Owner = this }.ShowDialog(); }
    private void Audit_Click(object sender, RoutedEventArgs e) { if (Allowed("Administration")) new AuditWindow { Owner = this }.ShowDialog(); }
 private void QC_Click(object sender,RoutedEventArgs e){if(Allowed("QC"))new QCWindow{Owner=this}.ShowDialog();}
 private void Analyzer_Click(object sender,RoutedEventArgs e){if(Allowed("MasterData"))new AnalyzerWindow{Owner=this}.ShowDialog();}
    private void Inventory_Click(object sender,RoutedEventArgs e){if(Allowed("Inventory"))new InventoryWindow{Owner=this}.ShowDialog();}
}