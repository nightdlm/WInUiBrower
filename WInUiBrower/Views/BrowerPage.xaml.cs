using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media.Protection.PlayReady;
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
            sender.CoreWebView2.Settings.AreDevToolsEnabled = DynamicContants.DeveloperMode;
            sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = DynamicContants.DisableRightClick;

            if (DynamicContants.OriginalPath == Enums.Origin.LocalFile) 
            {
                sender.Source = new Uri("ms-appdata:///Pages/index.html");
            }
            else if (DynamicContants.OriginalPath == Enums.Origin.Url)
            {
                sender.Source = new Uri(DynamicContants.Url);
            }
        }

        private void CoreWebView2_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            var request = args.Request;

            var waitHandleUrl = request.Uri.Replace("file:///", "");
            waitHandleUrl = waitHandleUrl[waitHandleUrl.IndexOf('/')..];
            var finalUrl = new Uri(new Uri(DynamicContants.FetchUrl), waitHandleUrl);

            Debug.WriteLine("拦截请求: " + request.Uri);

            // 创建 HttpRequestMessage 并设置请求头
            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToUpper()), finalUrl);

            // 复制原始请求内容（如果存在）
            if (request.Content != null)
            {
                httpRequest.Content = new StreamContent(request.Content.AsStream());
                Debug.WriteLine("请求Type: " + httpRequest.Content.GetType());
            }

            // 复制原始请求头
            foreach (var header in request.Headers)
            {
                if (!httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    // 如果是内容相关头，需要添加到 Content.Headers
                    if (httpRequest.Content != null)
                    {
                        httpRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            Debug.WriteLine("请求头: " + httpRequest.Headers);

            // 使用HttpClient处理请求
            using (var client = new HttpClient())
            {
                HttpResponseMessage? response = null;

                switch (request.Method.ToUpper())
                {
                    case "GET":
                        response = client.SendAsync(httpRequest).Result;
                        break;
                    case "POST":
                        response = client.SendAsync(httpRequest).Result;
                        break;
                    case "PUT":
                        response = client.SendAsync(httpRequest).Result;
                        break;
                    case "DELETE":
                        response = client.SendAsync(httpRequest).Result;
                        break;
                    default:
                        // 未支持的方法返回错误响应
                        var unsupportedResponse = sender.Environment.CreateWebResourceResponse(
                            null,
                            405,
                            "Method Not Allowed",
                            "");
                        args.Response = unsupportedResponse;
                        return;
                }

                // 读取响应内容
                var buffer = response.Content.ReadAsStreamAsync().Result;
                var stream = buffer.AsRandomAccessStream();

                // 构建完整的响应头字符串，包含所有响应头
                var headerBuilder = new System.Text.StringBuilder();

                // 添加响应头部
                foreach (var header in response.Headers)
                {
                    foreach (var value in header.Value)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {value}");
                    }
                }

                // 添加内容头部
                foreach (var header in response.Content.Headers)
                {
                    foreach (var value in header.Value)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {value}");
                    }
                }

                // 使用环境的 CreateWebResourceResponse 方法创建响应
                var webResponse = sender.Environment.CreateWebResourceResponse(
                    stream,
                    ((int)response.StatusCode),
                    "OK",
                    headerBuilder.ToString()
                );

                args.Response = webResponse;
                Debug.WriteLine("success");

            }

        }


        /// <summary>
        /// 页面加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            BrowerConfig.CoreWebView2Initialized += InitWebView;
        }
    }
}
