using System.Windows; using System.Windows.Controls; using LabMaster.Models; using LabMaster.Services;
namespace LabMaster;
public partial class PatientRegistrationWindow : Window {
 readonly PatientService service=new();
 public PatientRegistrationWindow(){InitializeComponent(); Loaded+=async(_,_)=>{try{MrBox.Text=await service.GetNextMrNumberAsync();}catch{MrBox.Text="MR-000001";}};}
 async void Save_Click(object sender,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(NameBox.Text)){MessageBox.Show("Patient name is required.");NameBox.Focus();return;} try{var p=new Patient{MRNumber=MrBox.Text,PatientName=NameBox.Text.Trim(),FatherName=string.IsNullOrWhiteSpace(FatherBox.Text)?null:FatherBox.Text.Trim(),Gender=(GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString()??"Male",Phone=string.IsNullOrWhiteSpace(PhoneBox.Text)?null:PhoneBox.Text.Trim(),ReferringDoctor=string.IsNullOrWhiteSpace(DoctorBox.Text)?null:DoctorBox.Text.Trim()};await service.SaveAsync(p);MessageBox.Show($"Patient saved successfully.\nMR Number: {p.MRNumber}");ClearForm();MrBox.Text=await service.GetNextMrNumberAsync();}catch(Exception ex){MessageBox.Show($"Could not save patient.\n\n{ex.Message}","Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 void Clear_Click(object sender,RoutedEventArgs e)=>ClearForm();
 void ClearForm(){NameBox.Clear();FatherBox.Clear();PhoneBox.Clear();DoctorBox.Clear();GenderBox.SelectedIndex=0;NameBox.Focus();}
}