using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class UserWindow:Window
{
 readonly UserService service=new();
 public UserWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{UsersGrid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message);}}
 async void Add_Click(object s,RoutedEventArgs e){var w=new AddUserWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
 async void Toggle_Click(object s,RoutedEventArgs e){if(UsersGrid.SelectedItem is not UserRow u)return;try{await service.SetActiveAsync(u.UserId,!u.IsActive);await LoadAsync();}catch(Exception ex){MessageBox.Show(ex.Message);}}
 async void Password_Click(object s,RoutedEventArgs e){if(UsersGrid.SelectedItem is not UserRow u){MessageBox.Show("Select a user.");return;}var w=new ChangePasswordWindow(u){Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
}