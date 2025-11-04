using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            var fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".exe");
            fileOpenPicker.FileTypeFilter.Add(".bat");
            WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);
            var file = await fileOpenPicker.PickSingleFileAsync();

            if (file != null)
            {
                if( DataContext is ServerItem Se)
                Se.WorkingDirectory = file.Name;
            }

        }

    }
}
