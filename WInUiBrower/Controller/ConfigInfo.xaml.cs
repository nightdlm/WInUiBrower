using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WInUiBrower.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WInUiBrower.Controller
{
    public sealed partial class ConfigInfo : UserControl
    {
        public ConfigInfo()
        {
            InitializeComponent();
        }


        private async void WorkingDirectoryButton_Click(object sender, RoutedEventArgs e)
        {

            var window = new Window();
            // ...
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
            var file = await folderPicker.PickSingleFolderAsync();

            if (file != null)
            {
                if( DataContext is ServerItem Se)
                Se.WorkingDirectory = file.Path;
            }

        }
        private async void FileButton_Click(object sender, RoutedEventArgs e)
        {

            var window = new Window();
            // ...
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".exe");
            fileOpenPicker.FileTypeFilter.Add(".bat");
            WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);
            var file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                if( DataContext is ServerItem Se)
                Se.ExecutableFile = file.Path;
            }

        }

        private async void DeleteServerItem_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is ServerItem Se)
            {
                // 创建内容对话框
                ContentDialog deleteDialog = new ContentDialog
                {
                    Title = "确认删除",
                    Content = "确定要删除这个服务器配置吗？此操作不可撤销。",
                    PrimaryButtonText = "删除",
                    CloseButtonText = "取消",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };

                // 显示对话框并等待用户选择
                ContentDialogResult result = await deleteDialog.ShowAsync();

                // 如果用户确认删除，则执行删除操作
                if (result == ContentDialogResult.Primary)
                {
                    DynamicContants.Instance.Items.Remove(Se);
                }
            }
        }
    }
}
