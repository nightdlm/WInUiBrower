using Microsoft.UI.Xaml.Controls;
using WInUiBrower.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StatusMonitorPage : Page
{
    public StatusMonitorPage()
    {
        InitializeComponent();
        this.DataContext = DynamicContants.Instance;
    }
}
