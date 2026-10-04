using System.Globalization;
using System.Windows;
using LabMaster.Services;

namespace LabMaster;

public partial class AddAnalyzerMaintenanceWindow : Window
{
    readonly AnalyzerService service = new();
    readonly int analyzerId;

    public AddAnalyzerMaintenanceWindow(int id, string name)
    {
        InitializeComponent();
        analyzerId = id;
        TitleText.Text = $"Add Maintenance — {name}";
        DateBox.SelectedDate = DateTime.Today;
        TypeBox.SelectedIndex = 0;
        PerformedByBox.Text = CurrentUserContext.UserName;
    }

    async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (TypeBox.SelectedItem is not System.Windows.Controls.ComboBoxItem typeItem)
        {
            MessageBox.Show("Select maintenance type.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        decimal? cost = null;
        if (!string.IsNullOrWhiteSpace(CostBox.Text))
        {
            if (!decimal.TryParse(CostBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) || parsed < 0)
            {
                MessageBox.Show("Enter a valid non-negative cost.", "Lab Master", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            cost = parsed;
        }

        try
        {
            await service.AddMaintenanceAsync(
                analyzerId,
                DateBox.SelectedDate ?? DateTime.Today,
                typeItem.Content?.ToString() ?? "Other",
                string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
                string.IsNullOrWhiteSpace(PerformedByBox.Text) ? null : PerformedByBox.Text.Trim(),
                NextDueBox.SelectedDate,
                cost,
                string.IsNullOrWhiteSpace(CertificateBox.Text) ? null : CertificateBox.Text.Trim(),
                string.IsNullOrWhiteSpace(ResultBox.Text) ? null : ResultBox.Text.Trim(),
                string.IsNullOrWhiteSpace(NotesBox.Text) ? null : NotesBox.Text.Trim());

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lab Master", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}