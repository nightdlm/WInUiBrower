using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WInUiBrower.Model;
using WinUiBrowser.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StatusMonitorPage : Page
{

    public StatusMonitorPage()
    {
        InitializeComponent();
        this.DataContext = DynamicContants.Instance;

        this.Loaded += (s, e) =>
        {
            // 创建定时任务,定时检查服务状态并更新
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        };
    }


    private void Start_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Button startButton = sender as Button;

        // 获取按钮所在行的数据上下文（即绑定的实例对象）
        var item = startButton?.DataContext as ServerItem;

        if (item != null)
        {
            if (!ProcessManagerUtil.IsJobRunning(item.Key)) {
                ProcessManagerUtil.StartProcessWithJobObject(item.Key, item.WorkingDirectory, item.ExecutableFile, " " + item.Args);
            }
        }
    }

    private void ReStart_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Stop_Click(sender,e);
        Start_Click(sender,e);
    }

    private void Stop_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Button startButton = sender as Button;

        // 获取按钮所在行的数据上下文（即绑定的实例对象）
        var item = startButton?.DataContext as ServerItem;

        if (ProcessManagerUtil.IsJobRunning(item.Key))
        {
            ProcessManagerUtil.CloseJobObject(item.Key);
        }

    }

    // 创建定时任务,定时检查服务状态并更新
    private void Timer_Tick(object sender, object e)
    {
        foreach (var item in DynamicContants.Instance.Items)
        {
            if (item.IsEnable)
            {
                if (ProcessManagerUtil.IsJobRunning(item.Key))
                {
                    if (item.Port != 0) {
                        if (ProcessManagerUtil.IsPortInUse(item.Port))
                        {
                            item.Status = StatusEnums.Running;
                        }
                        else
                        {
                            item.Status = StatusEnums.Starting;
                        }
                    } else {
                        item.Status = StatusEnums.Running;
                    }
                    
                }
                else
                {
                    item.Status = StatusEnums.Stopped;
                }
            }
        }
    }

}
