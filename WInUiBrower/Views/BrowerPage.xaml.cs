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
        private static string runtimePath = AppDomain.CurrentDomain.BaseDirectory;
       
        public BrowerPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化完成WebView2后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InitWebView(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.Settings.AreDevToolsEnabled = DynamicContants.Instance.DeveloperMode;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = DynamicContants.Instance.DisableRightClick;

        }

        /// <summary>
        /// 页面加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            BrowerConfig.CoreWebView2Initialized += InitWebView;
            if (DynamicContants.Instance.Url != null && DynamicContants.Instance.Url.Trim() != "")
            {
                BrowerConfig.Source = new Uri(DynamicContants.Instance.Url);
            }
            
        }

    }
}
