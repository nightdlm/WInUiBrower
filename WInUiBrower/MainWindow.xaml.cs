using Microsoft.UI.Xaml;
using System;
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
        public MainWindow()
        {
            AppWindow.SetIcon(@$"{runtimePath}\Assets\logo.ico");
            InitializeComponent();
            MainWindow_Loaded();
        }

        private void MainWindow_Loaded()
        {
            if(DynamicContants.OriginalPath == Enums.Origin.Url) {
                RootFrame.Navigate(typeof(BrowerPage));
            }
            else {
                RootFrame.Navigate(typeof(BrowerPage));
            }
        }

        private void RootNavigation_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                RootFrame.Navigate(typeof(SettingPage));
                return;
            }
            // 获取选中的NavigationViewItem
            var selectedItem = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;


            // 通过Tag属性进行筛选
            if (selectedItem?.Tag != null)
            {
                string tag = selectedItem.Tag.ToString();
                switch (tag)
                {
                    case "WebServer":
                        RootFrame.Navigate(typeof(BrowerPage));
                        break;
                }
            }
        }
    }
}
