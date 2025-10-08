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
        /// ��ʼ�����WebView2��ִ��
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

            Debug.WriteLine("��������: " + request.Uri);

            // ���� HttpRequestMessage ����������ͷ
            var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToUpper()), finalUrl);

            // ����ԭʼ�������ݣ�������ڣ�
            if (request.Content != null)
            {
                httpRequest.Content = new StreamContent(request.Content.AsStream());
                Debug.WriteLine("����Type: " + httpRequest.Content.GetType());
            }

            // ����ԭʼ����ͷ
            foreach (var header in request.Headers)
            {
                if (!httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    // ������������ͷ����Ҫ��ӵ� Content.Headers
                    if (httpRequest.Content != null)
                    {
                        httpRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            Debug.WriteLine("����ͷ: " + httpRequest.Headers);

            // ʹ��HttpClient��������
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
                        // δ֧�ֵķ������ش�����Ӧ
                        var unsupportedResponse = sender.Environment.CreateWebResourceResponse(
                            null,
                            405,
                            "Method Not Allowed",
                            "");
                        args.Response = unsupportedResponse;
                        return;
                }

                // ��ȡ��Ӧ����
                var buffer = response.Content.ReadAsStreamAsync().Result;
                var stream = buffer.AsRandomAccessStream();

                // ������������Ӧͷ�ַ���������������Ӧͷ
                var headerBuilder = new System.Text.StringBuilder();

                // �����Ӧͷ��
                foreach (var header in response.Headers)
                {
                    foreach (var value in header.Value)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {value}");
                    }
                }

                // �������ͷ��
                foreach (var header in response.Content.Headers)
                {
                    foreach (var value in header.Value)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {value}");
                    }
                }

                // ʹ�û����� CreateWebResourceResponse ����������Ӧ
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
        /// ҳ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            BrowerConfig.CoreWebView2Initialized += InitWebView;
        }
    }
}
