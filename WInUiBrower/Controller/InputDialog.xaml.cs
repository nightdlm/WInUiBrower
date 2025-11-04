// InputDialog.xaml.cs
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WInUiBrower.Model;

namespace WInUiBrower.Controller
{
    public sealed partial class InputDialog : ContentDialog
    {
        public string Key => KeyTextBox.Text;
        public string WorkingDirectory => WorkingDirectoryTextBox.Text;
        public string ExecutableFile => ExecutableFileTextBox.Text;

        public InputDialog()
        {
            this.InitializeComponent();
        }

        private async void BrowseExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".exe");
            fileOpenPicker.FileTypeFilter.Add(".bat");

            var window = new Window(); // 假设你在 App 类中有对主窗口的引用
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

            var file = await fileOpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                ExecutableFileTextBox.Text = file.Path;
            }
        }

        private async void BrowseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();

            var window = new Window(); // 假设你在 App 类中有对主窗口的引用
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                WorkingDirectoryTextBox.Text = folder.Path;
            }
        }


        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePrimaryButtonState();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePrimaryButtonState();
        }

        private void UpdatePrimaryButtonState()
        {
            // 检查 KeyTextBox 和 WorkingDirectoryTextBox 是否都有内容
            bool isKeyValid = !string.IsNullOrWhiteSpace(KeyTextBox.Text);
            bool isWorkingDirectoryValid = !string.IsNullOrWhiteSpace(WorkingDirectoryTextBox.Text);
            bool iFileValid = !string.IsNullOrWhiteSpace(ExecutableFileTextBox.Text);

            // 如果是KeyTextBox的name的对象才检查这个
            if (KeyTextBox.Name == "KeyTextBox")
            {
                // 拿到所有的key
                foreach (var item in DynamicContants.Instance.Items)
                {
                    if (item.Key == KeyTextBox.Text)
                    {
                        KeyTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                        IsPrimaryButtonEnabled = false;
                        return;
                    }
                }
            }

            // 恢复默认颜色
            KeyTextBox.BorderBrush = new SolidColorBrush(Colors.Transparent);

            // 设置确定按钮的启用状态
            IsPrimaryButtonEnabled = isKeyValid && isWorkingDirectoryValid && iFileValid;
        }
    }
}