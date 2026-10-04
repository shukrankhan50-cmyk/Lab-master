using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;

namespace LabMaster;

public partial class ReportWindow : Window
{
    readonly ReportService service = new();
    readonly ReportSettingsService settingsService = new();
    ReportOrder? selected;
    ReportData? report;

    public ReportWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadAsync();
    }

    async Task LoadAsync()
    {
        try
        {
            OrdersGrid.ItemsSource = await service.GetEnteredOrdersAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void OrdersGrid_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        try
        {
            selected = OrdersGrid.SelectedItem as ReportOrder;
            if (selected == null) return;

            await service.EnsureReportNumberAsync(selected.OrderId);
            report = await service.GetReportAsync(selected.OrderId);
            if (report == null) return;

            var settings = await settingsService.GetAsync();
            LabNameText.Text = settings.LaboratoryName;
            LabAddressText.Text = settings.Address ?? "";
            LabPhoneText.Text = settings.Phone ?? "";
            FooterText.Text = settings.ReportFooter ?? "";

            PatientInfo.Text =
                $"Patient: {report.PatientName}\n" +
                $"MR: {report.MRNumber}    Gender: {report.Gender}" +
                (string.IsNullOrWhiteSpace(report.Phone) ? "" : $"    Phone: {report.Phone}");

            ReportInfo.Text =
                $"Report: {report.ReportNumber}\n" +
                $"Order: {report.OrderNumber}\n" +
                $"Date: {report.OrderDate:dd-MMM-yyyy HH:mm}";

            ReportGrid.ItemsSource = report.Results;

            CommentText.Text = string.Join(
                Environment.NewLine,
                report.Results
                    .Where(x => !string.IsNullOrWhiteSpace(x.Comment))
                    .Select(x => x.Comment));

            VerifiedText.Text = report.VerifiedBy == null
                ? "Pending Verification"
                : $"{report.VerifiedBy}  |  {report.VerifiedAt:dd-MMM-yyyy HH:mm}";

            StatusText.Text = report.VerifiedBy == null ? "PENDING" : "VERIFIED";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    async void Verify_Click(object s, RoutedEventArgs e)
    {
        if (selected == null)
        {
            MessageBox.Show("Select a report first.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await service.VerifyAsync(selected.OrderId, CurrentUserContext.UserName);
            report = await service.GetReportAsync(selected.OrderId);
            if (report != null)
            {
                VerifiedText.Text = $"{report.VerifiedBy}  |  {report.VerifiedAt:dd-MMM-yyyy HH:mm}";
                StatusText.Text = "VERIFIED";
            }

            MessageBox.Show("Report verified successfully.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Print_Click(object s, RoutedEventArgs e)
    {
        if (report == null)
        {
            MessageBox.Show("Select a report first.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() != true) return;

        dlg.PrintVisual(ReportPanel, $"Lab Report - {report.ReportNumber ?? report.MRNumber}");
    }
}
