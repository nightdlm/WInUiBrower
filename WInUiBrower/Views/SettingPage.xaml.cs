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

        // ��ʼ��
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
            // ��ʾ����ȷ�϶Ի���
            ContentDialog restartDialog = new ContentDialog
            {
                Title = "����Ӧ��",
                Content = "Ӧ���������޸ģ���Ҫ����Ӧ�ò�����Ч���Ƿ�����������",
                PrimaryButtonText = "��������",
                CloseButtonText = "�Ժ�����",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await restartDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // ��������Ӧ��
                RestartApplication();
            }
        }

        private void RestartApplication()
        {
            // ����һ���µĽ���ʵ����ʹ�õ�ǰӦ�õĿ�ִ���ļ�·��
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            // �˳���ǰӦ��ʵ��
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
                ShowInvalidUriDialog("��ַ�����ǿյġ�");
                UrlInput.Text = DynamicContants.Url;
            } catch (UriFormatException)
            {
                ShowInvalidUriDialog("��ַ�������顣");
                UrlInput.Text = DynamicContants.Url;
            }
        }

        private async void ShowInvalidUriDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "ȷ��",
                XamlRoot = this.XamlRoot // ȷ���Ի�����ʾ����ȷ�ĸ�Ԫ����
            };

            await dialog.ShowAsync();
        }
    }
}
