using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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

        // 初始化
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DynamicContants.Instance.OriginalPath == Enums.Origin.Url) {
                OriginUrlRadio.SetValue(RadioButton.IsCheckedProperty, true);
                UrlInput.IsEnabled = true;
                SaveConfig.IsEnabled = true;
                FetchUrl.IsEnabled = false;
            }

            if (DynamicContants.Instance.OriginalPath == Enums.Origin.LocalFile) {
                OriginPathRadio.SetValue(RadioButton.IsCheckedProperty, true);
                UrlInput.IsEnabled = false;
                SaveConfig.IsEnabled = false;
                FetchUrl.IsEnabled = true;
            }
        }


        private void OriginUrlRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (OriginUrlRadio.IsChecked == true)
            {
                UrlInput.IsEnabled = true;
                SaveConfig.IsEnabled = true;
                FetchUrl.IsEnabled = false;
                DynamicContants.Instance.OriginalPath = Enums.Origin.Url;
            }
            else
            {
                UrlInput.IsEnabled = false;
                SaveConfig.IsEnabled = false;
                FetchUrl.IsEnabled = true;
                DynamicContants.Instance.OriginalPath = Enums.Origin.LocalFile;
            }
        }

        private void EnableDebug_Toggled(object sender, RoutedEventArgs e)
        {
            DynamicContants.Instance.DeveloperMode = EnableDebug.IsOn;
        }

        private void EnableContextMenu_Toggled(object sender,RoutedEventArgs e)
        {
            DynamicContants.Instance.DisableRightClick = EnableContextMenu.IsOn;
            EnableDebug.IsEnabled = EnableContextMenu.IsOn;
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Uri(UrlInput.Text);
                DynamicContants.Instance.Url = UrlInput.Text;
            }
            catch (ArgumentNullException) 
            {
                ShowInvalidUriDialog("地址不能是空的。");
                UrlInput.Text = DynamicContants.Instance.Url;
            } catch (UriFormatException)
            {
                ShowInvalidUriDialog("地址错误，请检查。");
                UrlInput.Text = DynamicContants.Instance.Url;
            }
        }

        private async void ShowInvalidUriDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot // 确保对话框显示在正确的根元素上
            };

            await dialog.ShowAsync();
        }
    }
}
