using System.Windows;
using System.Windows.Controls;
using LabMaster.Services;
namespace LabMaster;
public partial class ProfileWindow:Window
{
 readonly ProfileService service=new(); ProfileRow? selected;
 public ProfileWindow(){InitializeComponent();Loaded+=async(_,_)=>await LoadAsync();}
 async Task LoadAsync(){try{Grid.ItemsSource=await service.GetAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void Grid_SelectionChanged(object sender,SelectionChangedEventArgs e){selected=Grid.SelectedItem as ProfileRow;await LoadItemsAsync();}
 async Task LoadItemsAsync(){if(selected==null){SelectedProfileText.Text="Select a profile";CountText.Text="";ItemsGrid.ItemsSource=null;return;}SelectedProfileText.Text=$"{selected.ProfileName} ({selected.ProfileCode})";var items=await service.GetItemsAsync(selected.ProfileId);ItemsGrid.ItemsSource=items;CountText.Text=$"{items.Count} test(s) in this profile";}
 async void Add_Click(object sender,RoutedEventArgs e){var w=new AddProfileWindow{Owner=this};if(w.ShowDialog()==true)await LoadAsync();}
 async void Toggle_Click(object sender,RoutedEventArgs e){if(selected==null){MessageBox.Show("Select a profile first.");return;}try{await service.SetActiveAsync(selected.ProfileId,!selected.IsActive);await LoadAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
 async void AddTest_Click(object sender,RoutedEventArgs e){if(selected==null){MessageBox.Show("Select a profile first.");return;}var w=new AddProfileTestWindow(selected.ProfileId){Owner=this};if(w.ShowDialog()==true)await LoadItemsAsync();}
 async void RemoveTest_Click(object sender,RoutedEventArgs e){if(ItemsGrid.SelectedItem is not ProfileTestRow item){MessageBox.Show("Select a test to remove.");return;}if(MessageBox.Show($"Remove {item.TestName} from this profile?","Confirm",MessageBoxButton.YesNo,MessageBoxImage.Question)!=MessageBoxResult.Yes)return;try{await service.RemoveTestAsync(item.ProfileItemId);await LoadItemsAsync();}catch(Exception ex){MessageBox.Show(ex.Message,"Lab Master",MessageBoxButton.OK,MessageBoxImage.Error);}}
}