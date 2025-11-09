using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WInUiBrower.Controller;
using WInUiBrower.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerConfig : Page
    {
        public ServerConfig()
        {
            InitializeComponent();
            this.DataContext = DynamicContants.Instance;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private async void AppBarButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new InputDialog();
            dialog.XamlRoot = this.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // 创建新的 ServerItem 实例
                var newItem = new ServerItem
                {
                    Key = dialog.Key,
                    WorkingDirectory = dialog.WorkingDirectory,
                    ExecutableFile = dialog.ExecutableFile,
                    IsEnable = true, // 默认启用
                    Args = "",       // 默认空参数
                    Port = 0,     // 默认端口
                    DelayPortDetect = false,
                    WaitExit = true
                };

                DynamicContants.Instance.Items.Add(newItem);
            }
        }
    }
}
