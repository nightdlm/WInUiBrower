using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WInUiBrower.Model;
using WinUiBrowser.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ServerManager : Page
{

    private static bool isRunning = false;

    public ServerManager()
    {
        InitializeComponent();
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StopButton.Visibility = Visibility.Visible;
        StartButton.Visibility = Visibility.Collapsed;

        // 统计有多少个需要启动的进程服务
        int count = 0;
        foreach (ServerItem item in DynamicContants.Instance.Items)
        {
            if (item.IsEnable)
            {
                count++;
            }
        }

        MainProgressBar.Value = 0;
        List<ServerItem> successfullyStartedItems = new List<ServerItem>(); // 记录成功启动的服务
        List<ServerItem> jobKeys = [];

        try
        {
            foreach (ServerItem item in DynamicContants.Instance.Items)
            {
                if (item.IsEnable)
                {
                    if (ProcessManagerUtil.IsJobRunning(item.Key)) continue;

                    try
                    {
                        ProcessManagerUtil.StartProcessWithJobObject(item.Key, item.WorkingDirectory, item.ExecutableFile, " " + item.Args);

                        // 记录成功启动的服务
                        successfullyStartedItems.Add(item);

                        if (item.WaitExit)
                        {
                            await ProcessManagerUtil.WaitForJobExitAsync(item.Key);
                        }

                        if (!item.DelayPortDetect)
                        {
                            await ProcessManagerUtil.WaitForPortOccupiedAsync(item.Port);
                            MainProgressBar.Value = MainProgressBar.Value + 100 / count;
                        }
                        else
                        {
                            jobKeys.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 启动某个服务时发生异常，停止所有已启动的服务
                        ProcessManagerUtil.CloseAllJobObjects();
                        throw new Exception($"启动服务 {item.Key} 时发生错误: {ex.Message}", ex);
                    }
                }
            }

            // 处理延迟端口检测的服务
            while (jobKeys.Count > 0)
            {
                for (int i = jobKeys.Count - 1; i >= 0; i--)
                {
                    ServerItem item = jobKeys[i];

                    // 检查进程是否仍在运行
                    if (!ProcessManagerUtil.IsJobRunning(item.Key))
                    {
                        // 进程已退出，可能是异常退出，停止所有已启动的服务
                        ProcessManagerUtil.CloseAllJobObjects();
                        throw new Exception($"服务 {item.Key} 在启动过程中意外退出");
                    }

                    if (item.Port == 0) {
                        jobKeys.RemoveAt(i);
                        MainProgressBar.Value = MainProgressBar.Value + 100 / count;
                        continue;
                    }

                    if (ProcessManagerUtil.IsPortInUse(item.Port))
                    {
                        // 端口已被占用，说明进程已启动成功
                        jobKeys.RemoveAt(i);
                        MainProgressBar.Value = MainProgressBar.Value + 100 / count;
                    }
                }
                await Task.Delay(1000);
            }

            MainProgressBar.Value = 100;
            isRunning = true;
            if (DynamicContants.Instance.IsForwardSelfBrower)
            {
                this.Frame.Navigate(typeof(BrowerPage));
            }
        }
        catch (Exception ex)
        {
            // 发生异常时确保UI状态正确
            StartButton.Visibility = Visibility.Visible;
            StopButton.Visibility = Visibility.Collapsed;
            MainProgressBar.Value = 0;

            // 显示错误对话框告知用户
            var dialog = new ContentDialog
            {
                Title = "服务启动失败",
                Content = ex.Message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot // 确保对话框正确显示
            };

            await dialog.ShowAsync();
        }
    }


    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        ProcessManagerUtil.CloseAllJobObjects();
        StartButton.Visibility = Visibility.Visible;
        StopButton.Visibility = Visibility.Collapsed;
        isRunning = false;
    }

    private void Page_Loading(FrameworkElement sender, object args)
    {
        StopButton.Visibility = isRunning ? Visibility.Visible : Visibility.Collapsed;
        StartButton.Visibility = isRunning ? Visibility.Collapsed : Visibility.Visible;
    }
}
