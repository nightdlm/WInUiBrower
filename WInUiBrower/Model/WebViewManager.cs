using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;

namespace WInUiBrower.Model
{
    internal class WebViewManager
    {
        private static WebView2 _webViewInstance;
        private static bool _isMappingInitialized = false;

        public static WebView2 GetWebView()
        {
            if (_webViewInstance == null)
            {
                _webViewInstance = new WebView2();
                // 初始化 WebView 相关设置
            }
            return _webViewInstance;
        }

        public static void InitializeWebViewMapping(WebView2 webView)
        {
            if (!_isMappingInitialized)
            {
                // 记录时间
                DateTime startTime = DateTime.Now;
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "app.local",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages"),
                    CoreWebView2HostResourceAccessKind.Allow);
                _isMappingInitialized = true;
                Console.WriteLine($"映射时间：{DateTime.Now - startTime}");
            }
        }
    }
}
