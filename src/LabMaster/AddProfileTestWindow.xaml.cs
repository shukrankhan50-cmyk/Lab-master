using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class AddProfileTestWindow:Window
{
 readonly ProfileService profiles=new(); readonly TestOrderService tests=new(); readonly int profileId;
 public AddProfileTestWindow(int id){InitializeComponent();profileId=id;Loaded+=async(_,_)=>await LoadAsync("");}
 async Task LoadAsync(string q){try{TestBox.ItemsSource=await tests.GetTestsAsync(q);if(TestBox.Items.Count>0)TestBox.SelectedIndex=0;}catch(Exception ex){StatusText.Text=ex.Message;}}
 async void Search_TextChanged(object sender,System.Windows.Controls.TextChangedEventArgs e){if(IsLoaded)await LoadAsync(SearchBox.Text);}
 async void Save_Click(object sender,RoutedEventArgs e){if(TestBox.SelectedItem is not TestOption test){StatusText.Text="Select a test.";return;}try{var existing=await profiles.GetItemsAsync(profileId);await profiles.AddTestAsync(profileId,test.TestId,existing.Count+1);DialogResult=true;}catch(Exception ex){StatusText.Text=ex.Message;}}
}