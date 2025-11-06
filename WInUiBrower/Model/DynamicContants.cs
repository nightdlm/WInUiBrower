using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Windows.Storage;
using WInUiBrower.Enums;
using WinUiBrowser.Utils;

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
        private bool _isForwardSelfBrower = false;
        private ObservableCollection<ServerItem> _items { get; set; } = [];

        public static DynamicContants Instance => _instance;

        public bool IsForwardSelfBrower
        {
            get => _isForwardSelfBrower;
            set
            {
                if (_isForwardSelfBrower != value)
                {
                    _isForwardSelfBrower = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ObservableCollection<ServerItem> Items
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
            var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string configDirectory = Path.Combine(localFolderPath, "config");
            string fullPath = Path.Combine(localFolderPath, "config", "setting.json");

            // 确保配置目录存在
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, jsonString); // 写入文件
        }

        /// <summary>
        /// 从相对路径下的文件加载配置
        /// </summary>
        public static void LoadFromFile()
        {

            // 如果用户配置不存在，尝试从应用包目录加载默认配置
            var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string packageConfigPath = Path.Combine(localFolderPath, "config", "setting.json");

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
            IsForwardSelfBrower = config.IsForwardSelfBrower;
            Items = config.Items ?? [];
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// 从Items列表中移除指定的ServerItem对象（静态方法）
        /// </summary>
        /// <param name="item">要移除的ServerItem对象</param>
        public static void RemoveServerItemStatic(ServerItem item)
        {
            if (_instance._items.Contains(item))
            {
                _instance._items.Remove(item);
                _instance.OnPropertyChanged(nameof(Items));
            }
        }

        /// <summary>
        /// 添加一个ServerItem对象到Items列表（静态方法）
        /// </summary>
        /// <param name="item"></param>
        public static void AddServerItem(ServerItem item)
        {
            _instance._items.Add(item);
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
        public bool IsForwardSelfBrower { get; set; }
        public ObservableCollection<ServerItem> Items { get; set; } = new ObservableCollection<ServerItem>();
    }

    public class ServerItem : INotifyPropertyChanged
    {

        private bool _isEnable = false;
        private string _key = "";
        private string _workingDirectory = "";
        private string _executableFile = "";
        private string _args = "";
        private int _port = 0;
        private bool _delayPortDetect;
        private bool _waitExit;

        private StatusEnums _status = StatusEnums.Stopped;

        public StatusEnums Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string ExecutableFile
        {
            get => _executableFile;
            set
            {
                if (_executableFile != value)
                {
                    _executableFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set
            {
                if (_workingDirectory != value)
                {
                    _workingDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Args
        {
            get => _args;
            set
            {
                if (_args != value)
                {
                    _args = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DelayPortDetect
        {
            get => _delayPortDetect;
            set
            {
                if (_delayPortDetect != value)
                {
                    _delayPortDetect = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool WaitExit
        {
            get => _waitExit;
            set
            {
                if (_waitExit != value)
                {
                    _waitExit = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    public enum StatusEnums
    { 
         Running,
         Stopped,
         Starting
        
    }

}