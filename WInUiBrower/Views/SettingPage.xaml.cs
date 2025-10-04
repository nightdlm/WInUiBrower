using Microsoft.UI.Xaml.Controls;
using WInUiBrower.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        // ≥ı ºªØ
        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DynamicContants.OriginalPath == Enums.Origin.Url) {
                OriginUrlRadio.SetValue(RadioButton.IsCheckedProperty, true);
                UrlInput.IsEnabled = true;
            }

            if (DynamicContants.OriginalPath == Enums.Origin.LocalFile) {
                OriginPathRadio.SetValue(RadioButton.IsCheckedProperty, true);
                UrlInput.IsEnabled = false;
            }
        }

        private void OriginUrlRadio_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (OriginUrlRadio.IsChecked == true)
            {
                UrlInput.IsEnabled = true;
                DynamicContants.OriginalPath = Enums.Origin.Url;
            }
            else
            {
                UrlInput.IsEnabled = false;
                DynamicContants.OriginalPath = Enums.Origin.LocalFile;
            }
        }
    }
}
