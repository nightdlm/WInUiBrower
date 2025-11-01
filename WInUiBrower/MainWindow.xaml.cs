using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
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
        private static string runtimePath = AppDomain.CurrentDomain.BaseDirectory;

        // ��ȡ������̨����
        public static string[] args = Environment.GetCommandLineArgs();

        public MainWindow()
        {
            //AppWindow.SetIcon(@$"{runtimePath}\Assets\logo.ico");
            InitializeComponent();
            MainWindow_Loaded();
            this.Closed += (s, e) =>
            {
                DynamicContants.SaveToFile();
            };
        }


        private void CenterWindow()
        {
            // ��ȡ���ھ��
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // ��ȡ AppWindow
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            // ��ȡ����ʾ����Ϣ
            var displayArea = Microsoft.UI.Windowing.DisplayArea.Primary;

            // �������λ��
            var centerX = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            var centerY = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;

            // ���ô���λ��
            appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
        }

        private void MainWindow_Loaded()
        {
            CenterWindow();
            this.Title = DynamicContants.AppName;
            

            if (args.Contains("--console")) {
                RootNavigation.IsPaneVisible = true;
            } else {
                RootNavigation.IsPaneVisible = false;
            }
            RootFrame.Navigate(typeof(ServerManager));

        }

        private void RootNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                RootFrame.Navigate(typeof(SettingPage));
                return;
            }
            // ��ȡѡ�е�NavigationViewItem
            var selectedItem = args.SelectedItem as NavigationViewItem;


            // ͨ��Tag���Խ���ɸѡ
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
                }
            }
        }

    }
}
