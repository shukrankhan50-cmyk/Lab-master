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

    private async void Dashboard_Click(object sender, RoutedEventArgs e) => await RefreshDashboardAsync();
    private void PatientRegistration_Click(object sender, RoutedEventArgs e) => new PatientRegistrationWindow { Owner = this }.ShowDialog();
    private void PatientSearch_Click(object sender, RoutedEventArgs e) => new PatientSearchWindow { Owner = this }.ShowDialog();
    private void AcceptTest_Click(object sender, RoutedEventArgs e) { new AcceptTestWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Billing_Click(object sender, RoutedEventArgs e) { new BillingWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void ResultEntry_Click(object sender, RoutedEventArgs e) { new ResultEntryWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Reports_Click(object sender, RoutedEventArgs e) { new ReportWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Departments_Click(object sender, RoutedEventArgs e) => new DepartmentWindow { Owner = this }.ShowDialog();
    private void Profile_Click(object sender, RoutedEventArgs e) => new ProfileWindow { Owner = this }.ShowDialog();
    private void TestMaster_Click(object sender, RoutedEventArgs e) => new TestMasterWindow { Owner = this }.ShowDialog();
    private void User_Click(object sender, RoutedEventArgs e) => new UserWindow { Owner = this }.ShowDialog();
    private void Backup_Click(object sender, RoutedEventArgs e) => new BackupWindow { Owner = this }.ShowDialog();
    private void Expense_Click(object sender, RoutedEventArgs e) { new ExpenseWindow { Owner = this }.ShowDialog(); _ = RefreshDashboardAsync(); }
    private void Financial_Click(object sender, RoutedEventArgs e) => new FinancialReportWindow { Owner = this }.ShowDialog();
    private void Settings_Click(object sender, RoutedEventArgs e) => new SettingsWindow { Owner = this }.ShowDialog();
    private void Audit_Click(object sender, RoutedEventArgs e) => new AuditWindow { Owner = this }.ShowDialog();
 private void QC_Click(object sender,RoutedEventArgs e)=>new QCWindow{Owner=this}.ShowDialog();
 private void Inventory_Click(object sender,RoutedEventArgs e)=>new InventoryWindow{Owner=this}.ShowDialog();
}