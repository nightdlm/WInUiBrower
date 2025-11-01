using System;
using System.IO;
using System.Text.Json;
using System.Reflection;
using System.Collections.Generic;
using WInUiBrower.Enums;

namespace WInUiBrower.Model
{

    class DynamicContants
    {

        public static string AppName = "WinUiBrower";

        public static Origin OriginalPath = Origin.Url;

        // 源url地址
        public static string Url = "https://www.baidu.com";

        // fetch地址
        public static string FetchUrl = "http://localhost:8080";

        // 是否启用开发者模式
        public static bool DeveloperMode = false;

        // 禁用右键点击
        public static bool DisableRightClick = false;

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
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                settings[field.Name] = field.GetValue(null);
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
                ReadSettingsFromFile(packageConfigPath);
            }
        }

        private static void ReadSettingsFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

                if (settings != null)
                {
                    if (settings.TryGetValue("AppName", out JsonElement appNameElement))
                        AppName = appNameElement.GetString() ?? AppName;

                    if (settings.TryGetValue("OriginalPath", out JsonElement originalPathElement))
                        OriginalPath = (Origin)originalPathElement.GetInt32();

                    if (settings.TryGetValue("Url", out JsonElement urlElement))
                        Url = urlElement.GetString() ?? Url;

                    if (settings.TryGetValue("FetchUrl", out JsonElement fetchUrlElement))
                        FetchUrl = fetchUrlElement.GetString() ?? FetchUrl;

                    if (settings.TryGetValue("DeveloperMode", out JsonElement developerModeElement))
                        DeveloperMode = developerModeElement.GetBoolean();

                    if (settings.TryGetValue("DisableRightClick", out JsonElement disableRightClickElement))
                        DisableRightClick = disableRightClickElement.GetBoolean();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"读取配置文件失败: {ex.Message}");
            }
        }
    }
}