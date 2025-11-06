using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using WInUiBrower.Model;
using WInUiBrower.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        // 获取到控制台参数
        public static string[] args = Environment.GetCommandLineArgs();

        public MainWindow()
        {
            InitializeComponent();

            MainWindow_Loaded();

            this.Closed += (s, e) =>
            {
                 DynamicContants.SaveToFile();
            };
        }


        private void CenterWindow()
        {
            // 获取窗口句柄
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // 获取 AppWindow
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            // 获取主显示器信息
            var displayArea = Microsoft.UI.Windowing.DisplayArea.Primary;

            // 计算居中位置
            var centerX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

            // 设置窗口位置
            appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
        }

        private void MainWindow_Loaded()
        {
            // 设置自定义标题栏
            ExtendsContentIntoTitleBar = true;
            CenterWindow();

            // 获取环境变量来决定是否显示导航面板
            var consoleEnvVar = Environment.GetEnvironmentVariable("CONSOLE_MODE");
            if (!string.IsNullOrEmpty(consoleEnvVar) && consoleEnvVar.ToLower() == "true") { 
                RootNavigation.IsPaneVisible = true;
            } else {
                RootNavigation.IsPaneVisible = false;
            }
            var serverManagerItem = RootNavigation.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(i => i.Tag?.ToString() == "ServerManager");
            RootNavigation.SelectedItem = serverManagerItem;

        }

        private void RootNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                RootFrame.Navigate(typeof(SettingPage));
                return;
            }
            // 获取选中的NavigationViewItem
            var selectedItem = args.SelectedItem as NavigationViewItem;


            // 通过Tag属性进行筛选
            if (selectedItem?.Tag != null)
            {
                string tag = selectedItem?.Tag as string ?? string.Empty;
                switch (tag)
                {
                    case "WebServer":
                        RootFrame.Navigate(typeof(BrowerPage));
                        break;
                    case "ServerManager":
                        RootFrame.Navigate(typeof(ServerManager));
                        break;
                    case "StatusMonitor":
                        RootFrame.Navigate(typeof(StatusMonitorPage));
                        break;
                    case "ServerConfig":
                        RootFrame.Navigate(typeof(ServerConfig));
                        break;
                }
            }
        }

    }
}
