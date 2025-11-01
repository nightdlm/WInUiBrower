using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ServerManager : Page
{

    public ServerManager()
    {
        InitializeComponent();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StopButton.Visibility = Visibility.Visible;
        StartButton.Visibility = Visibility.Collapsed;


        this.Frame.Navigate(typeof(BrowerPage));
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StartButton.Visibility = Visibility.Visible;
        StopButton.Visibility = Visibility.Collapsed;
    }

}
