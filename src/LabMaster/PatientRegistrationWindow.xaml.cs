using System.Windows;
using System.Windows.Controls;
using LabMaster.Models;
using LabMaster.Services;

namespace LabMaster;

public partial class PatientRegistrationWindow : Window
{
    private readonly PatientService service = new();

    public PatientRegistrationWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            try { MrBox.Text = await service.GetNextMrNumberAsync(); }
            catch { MrBox.Text = "MR-000001"; }
        };
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Patient name is required.", "Lab Master");
            NameBox.Focus();
            return;
        }

        try
        {
            var patient = new Patient
            {
                MRNumber = MrBox.Text,
                PatientName = NameBox.Text.Trim(),
                FatherName = NullIfEmpty(FatherBox.Text),
                Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Male",
                DateOfBirth = DobBox.SelectedDate,
                AgeYears = ParseInt(AgeYearsBox.Text),
                AgeMonths = ParseInt(AgeMonthsBox.Text),
                AgeDays = ParseInt(AgeDaysBox.Text),
                Phone = NullIfEmpty(PhoneBox.Text),
                ReferringDoctor = NullIfEmpty(DoctorBox.Text),
                Address = NullIfEmpty(AddressBox.Text)
            };

            await service.SaveAsync(patient);
            MessageBox.Show($"Patient saved successfully.\nMR Number: {patient.MRNumber}", "Lab Master");
            ClearForm();
            MrBox.Text = await service.GetNextMrNumberAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not save patient.\n\n{ex.Message}", "Lab Master",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void ClearForm()
    {
        NameBox.Clear();
        FatherBox.Clear();
        PhoneBox.Clear();
        DoctorBox.Clear();
        AddressBox.Clear();
        AgeYearsBox.Clear();
        AgeMonthsBox.Clear();
        AgeDaysBox.Clear();
        DobBox.SelectedDate = null;
        GenderBox.SelectedIndex = 0;
        NameBox.Focus();
    }

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static int? ParseInt(string value) =>
        int.TryParse(value, out var number) && number >= 0 ? number : null;
}