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
    public sealed partial class BrowerPage : Page
    {
        public BrowerPage()
        {
            InitializeComponent();
            BrowerConfig.CoreWebView2Initialized += InitWebView;
        }

        private void InitWebView(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.AreDevToolsEnabled = DynamicContants.DeveloperMode;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = DynamicContants.DisableRightClick;
        }

        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DynamicContants.OriginalPath == Enums.Origin.Url) {
                BrowerConfig.Source = new Uri(DynamicContants.Url);
            }
        }
    }
}
