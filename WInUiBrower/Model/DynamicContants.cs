using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace WInUiBrower.Model
{
    class DynamicContants
    {
        // 配置文件名
        private const string ConfigFileName = "setting.json";

        private DynamicContants() { }

        // 添加私有静态实例字段
        private static readonly SystemInfo _instance = new();

        // 提供公共静态属性访问实例
        public static SystemInfo Instance => _instance;

        /// <summary>
        /// 静态构造函数，在类首次被使用时自动调用
        /// </summary>
        static DynamicContants()
        {
            // 应用启动时加载配置
            _ = LoadAsync();
        }

        /// <summary>
        /// 保存配置到本地文件
        /// </summary>
        public static async Task SaveAsync()
        {
            try
            {
                // 获取本地存储文件夹
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // 创建或打开配置文件
                StorageFile configFile = await localFolder.CreateFileAsync(
                    ConfigFileName,
                    CreationCollisionOption.ReplaceExisting
                );

                // 序列化SystemInfo实例为JSON
                string jsonContent = JsonConvert.SerializeObject(_instance, Newtonsoft.Json.Formatting.Indented);

                // 写入文件
                await FileIO.WriteTextAsync(configFile, jsonContent);
            }
            catch (Exception ex)
            {
                // 记录保存配置时的异常（如果ErrorLog可用）
                System.Diagnostics.Debug.WriteLine($"保存配置失败：{ex.Message}");
            }
        }


        /// <summary>
        /// 从本地文件加载配置
        /// </summary>
        public static async Task LoadAsync()
        {
            try
            {
                // 获取本地存储文件夹
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // 尝试获取配置文件
                StorageFile configFile = await localFolder.TryGetItemAsync(ConfigFileName) as StorageFile;

                if (configFile != null)
                {
                    // 读取文件内容
                    string jsonContent = await FileIO.ReadTextAsync(configFile);

                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        // 反序列化JSON到现有实例（而不是创建新实例）
                        JsonConvert.PopulateObject(jsonContent, _instance);
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录加载配置时的异常
                System.Diagnostics.Debug.WriteLine($"加载配置失败：{ex.Message}");
            }
        }

    }

}