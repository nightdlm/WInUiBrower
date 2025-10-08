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
            if (DynamicContants.OriginalPath == Enums.Origin.Url) {
                OriginUrlRadio.SetValue(RadioButton.IsCheckedProperty, true);
                UrlInput.IsEnabled = true;
                SaveConfig.IsEnabled = true;
                FetchUrl.IsEnabled = false;
            }

            if (DynamicContants.OriginalPath == Enums.Origin.LocalFile) {
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
                DynamicContants.OriginalPath = Enums.Origin.Url;
            }
            else
            {
                UrlInput.IsEnabled = false;
                SaveConfig.IsEnabled = false;
                FetchUrl.IsEnabled = true;
                DynamicContants.OriginalPath = Enums.Origin.LocalFile;
            }
        }

        private void EnableDebug_Toggled(object sender, RoutedEventArgs e)
        {
            DynamicContants.DeveloperMode = EnableDebug.IsOn;
        }

        private void EnableContextMenu_Toggled(object sender,RoutedEventArgs e)
        {
            DynamicContants.DisableRightClick = EnableContextMenu.IsOn;
            EnableDebug.IsEnabled = EnableContextMenu.IsOn;
        }

        private async void AppNameInput_LostFocus(object sender,RoutedEventArgs e)
        {
            if (AppNameInput.Text == DynamicContants.AppName)
            {
                return;
            }

            DynamicContants.AppName = AppNameInput.Text;
            // 显示重启确认对话框
            ContentDialog restartDialog = new ContentDialog
            {
                Title = "重启应用",
                Content = "应用名称已修改，需要重启应用才能生效。是否立即重启？",
                PrimaryButtonText = "立即重启",
                CloseButtonText = "稍后重启",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await restartDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // 立即重启应用
                RestartApplication();
            }
        }

        private void RestartApplication()
        {
            // 启动一个新的进程实例，使用当前应用的可执行文件路径
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            // 退出当前应用实例
            Application.Current.Exit();
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Uri(UrlInput.Text);
                DynamicContants.Url = UrlInput.Text;
            }
            catch (ArgumentNullException) 
            {
                ShowInvalidUriDialog("地址不能是空的。");
                UrlInput.Text = DynamicContants.Url;
            } catch (UriFormatException)
            {
                ShowInvalidUriDialog("地址错误，请检查。");
                UrlInput.Text = DynamicContants.Url;
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
