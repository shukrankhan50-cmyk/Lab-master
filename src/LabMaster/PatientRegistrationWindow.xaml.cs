using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LabMaster.Models;
using LabMaster.Services;

namespace LabMaster;

public partial class PatientRegistrationWindow : Window
{
    private readonly PatientService service = new();

    public PatientRegistrationWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await PrepareNextMrNumberAsync();
    }

    private async Task PrepareNextMrNumberAsync()
    {
        SaveButton.IsEnabled = false;
        try
        {
            MrBox.Text = await service.GetNextMrNumberAsync();
            StatusText.Text = "Enter the patient details, then select Save Patient.";
            StatusText.Foreground = (Brush)FindResource("MutedTextBrush");
            SaveButton.IsEnabled = true;
        }
        catch
        {
            MrBox.Clear();
            SetStatus("A new MR number could not be loaded. Check the SQL Server connection before saving.", true);
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            SetStatus("Patient name is required.", true);
            NameBox.Focus();
            return;
        }

        if (DobBox.SelectedDate is DateTime dob && dob.Date > DateTime.Today)
        {
            SetStatus("Date of birth cannot be in the future.", true);
            DobBox.Focus();
            return;
        }

        if (!TryReadAge(out var years, out var months, out var days))
        {
            SetStatus("Enter age using numbers: years 0–130, months 0–11, and days 0–31.", true);
            AgeYearsBox.Focus();
            return;
        }

        SaveButton.IsEnabled = false;
        try
        {
            var patient = new Patient
            {
                MRNumber = MrBox.Text,
                PatientName = NameBox.Text.Trim(),
                FatherName = NullIfEmpty(FatherBox.Text),
                Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Male",
                DateOfBirth = DobBox.SelectedDate,
                AgeYears = years,
                AgeMonths = months,
                AgeDays = days,
                Phone = NullIfEmpty(PhoneBox.Text),
                ReferringDoctor = NullIfEmpty(DoctorBox.Text),
                Address = NullIfEmpty(AddressBox.Text)
            };

            await service.SaveAsync(patient);
            MessageBox.Show($"Patient saved successfully.\nMR Number: {patient.MRNumber}", "Lab Master",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ClearForm();
            MrBox.Clear();
            await PrepareNextMrNumberAsync();
        }
        catch (Exception ex)
        {
            SaveButton.IsEnabled = true;
            SetStatus("The patient record could not be saved. Review the message and try again.", true);
            MessageBox.Show($"Could not save patient.\n\n{ex.Message}", "Lab Master",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
        SetStatus("Form cleared. The current MR number is still available.", false);
    }

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

    private void SetStatus(string message, bool isError)
    {
        StatusText.Text = message;
        StatusText.Foreground = isError ? Brushes.IndianRed : (Brush)FindResource("MutedTextBrush");
    }

    private bool TryReadAge(out int? years, out int? months, out int? days)
    {
        years = months = days = null;
        return TryParseAgePart(AgeYearsBox.Text, 130, out years)
            && TryParseAgePart(AgeMonthsBox.Text, 11, out months)
            && TryParseAgePart(AgeDaysBox.Text, 31, out days);
    }

    private static bool TryParseAgePart(string value, int maximum, out int? parsed)
    {
        parsed = null;
        if (string.IsNullOrWhiteSpace(value)) return true;
        if (!int.TryParse(value, out var number) || number < 0 || number > maximum) return false;
        parsed = number;
        return true;
    }

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}