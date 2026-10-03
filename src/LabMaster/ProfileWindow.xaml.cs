using System.Windows;
using LabMaster.Services;
namespace LabMaster;
public partial class ProfileWindow:Window
{
 readonly ProfileService service=new();
 public ProfileWindow(){InitializeComponent();Loaded+=async(_,_)=>Grid.ItemsSource=await service.GetAsync();}
}