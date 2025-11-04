// InputDialog.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace WInUiBrower.Controller
{
    public sealed partial class InputDialog : ContentDialog
    {
        public string Key => KeyTextBox.Text;
        public string WorkingDirectory => WorkingDirectoryTextBox.Text;

        public InputDialog()
        {
            this.InitializeComponent();
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
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
                WorkingDirectoryTextBox.Text = file.Name;
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

            // 设置确定按钮的启用状态
            IsPrimaryButtonEnabled = isKeyValid && isWorkingDirectoryValid;
        }
    }
}