using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WInUiBrower.Enums;

namespace WInUiBrower.Model
{
    class DynamicContants : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private static readonly DynamicContants _instance = new DynamicContants();

        private string _appName = "";
        private Origin _originalPath = Origin.Url;
        private string _url = "";
        private string _fetchUrl = "";
        private bool _developerMode = false;
        private bool _disableRightClick = false;
        private List<ServerItem> _items = [];

        public static DynamicContants Instance => _instance;

        public string AppName
        {
            get => _appName;
            set
            {
                if (_appName != value)
                {
                    _appName = value;
                    OnPropertyChanged();
                }
            }
        }

        public Origin OriginalPath
        {
            get => _originalPath;
            set
            {
                if (_originalPath != value)
                {
                    _originalPath = value;
                    OnPropertyChanged();
                }
            }
        }

        // 源url地址
        public string Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged();
                }
            }
        }

        // fetch地址
        public string FetchUrl
        {
            get => _fetchUrl;
            set
            {
                if (_fetchUrl != value)
                {
                    _fetchUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        // 是否启用开发者模式
        public bool DeveloperMode
        {
            get => _developerMode;
            set
            {
                if (_developerMode != value)
                {
                    _developerMode = value;
                    OnPropertyChanged();
                }
            }
        }

        // 禁用右键点击
        public bool DisableRightClick
        {
            get => _disableRightClick;
            set
            {
                if (_disableRightClick != value)
                {
                    _disableRightClick = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<ServerItem> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 静态构造函数，在类首次被使用时自动调用
        /// </summary>
        static DynamicContants()
        {
            LoadFromFile();
        }

        /// <summary>
        /// 保存配置到相对路径下的文件
        /// </summary>
        public static void SaveToFile()
        {
            var settings = new Dictionary<string, object>();
            Type type = typeof(DynamicContants);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                settings[property.Name] = property.GetValue(_instance);
            }

            // 使用应用程序目录下的路径，与LoadFromFile保持一致
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, "config", "setting.json");

            string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, jsonString); // 写入文件
        }

        /// <summary>
        /// 从相对路径下的文件加载配置
        /// </summary>
        public static void LoadFromFile()
        {
            // 如果用户配置不存在，尝试从应用包目录加载默认配置
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string packageConfigPath = Path.Combine(baseDir, "config", "setting.json");

            if (File.Exists(packageConfigPath))
            {
                try
                {
                    string jsonString = File.ReadAllText(packageConfigPath);
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var config = JsonSerializer.Deserialize<Appconfig>(jsonString);
                        // 假设你有一个 UpdateFromConfig(Appconfig config) 的方法来更新静态字段
                        _instance.UpdateFromConfig(config);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"读取配置文件失败: {ex.Message}");
                }
            }
        }

        private void UpdateFromConfig(Appconfig config)
        {
            AppName = config.AppName;
            OriginalPath = config.OriginalPath;
            Url = config.Url;
            FetchUrl = config.FetchUrl;
            DeveloperMode = config.DeveloperMode;
            DisableRightClick = config.DisableRightClick;
            Items = config.Items ?? [];
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class Appconfig
    {
        public string AppName { get; set; } = "";
        public Origin OriginalPath { get; set; }
        public string Url { get; set; } = "";
        public string FetchUrl { get; set; } = "";
        public bool DeveloperMode { get; set; }
        public bool DisableRightClick { get; set; }
        public List<ServerItem> Items { get; set; } = [];
    }

    public class ServerItem
    {

        public string Key { get; set; } = "";

        public string WorkingDirectory { get; set; } = "";

        public string Args { get; set; } = "";

        public int Port { get; set; }

        public bool DelayPortDetect { get; set; }

        public bool WaitExit { get; set; }
    }

}