using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUiBrowser.Helpers
{
    /// <summary>
    /// 错误日志管理器，负责记录日志到本地文件
    /// </summary>
    public static class ErrorLog
    {
        // 日志文件路径（保存在应用沙盒的LocalFolder中，MSIX兼容）
        private static readonly string _logFileName = "error_log.txt";
        private static readonly StorageFolder _logFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// 记录错误日志（包含详细信息）
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="customMessage">自定义错误描述（可选）</param>
        public static async Task LogErrorAsync(Exception ex, string customMessage = "")
        {
            if (ex == null) return;

            // 构建详细日志内容
            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] 错误发生");
            if (!string.IsNullOrEmpty(customMessage))
                logBuilder.AppendLine($"自定义描述：{customMessage}");
            logBuilder.AppendLine($"异常类型：{ex.GetType().FullName}");
            logBuilder.AppendLine($"错误消息：{ex.Message}");
            logBuilder.AppendLine($"堆栈跟踪：{ex.StackTrace}");

            // 记录内部异常（如果有）
            if (ex.InnerException != null)
            {
                logBuilder.AppendLine($"内部异常：{ex.InnerException.Message}");
                logBuilder.AppendLine($"内部堆栈：{ex.InnerException.StackTrace}");
            }

            logBuilder.AppendLine("---------------------------------------------------");
            logBuilder.AppendLine();

            // 写入日志文件
            await WriteLogToFileAsync(logBuilder.ToString());
        }

        /// <summary>
        /// 写入日志到本地文件（追加模式）
        /// </summary>
        private static async Task WriteLogToFileAsync(string logContent)
        {
            try
            {
                // 获取或创建日志文件
                var logFile = await _logFolder.CreateFileAsync(
                    _logFileName,
                    CreationCollisionOption.OpenIfExists
                );

                // 以追加模式写入（避免覆盖历史日志）
                await FileIO.AppendTextAsync(logFile, logContent);
            }
            catch (Exception ex)
            {
                // 极端情况下日志写入失败，至少在调试输出中提示
                System.Diagnostics.Debug.WriteLine($"日志写入失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取日志文件路径（用于调试或让用户上传日志）
        /// </summary>
        public static async Task<string> GetLogFilePathAsync()
        {
            var logFile = await _logFolder.TryGetItemAsync(_logFileName) as StorageFile;
            return logFile?.Path;
        }

        /// <summary>
        /// 清空日志文件（可选功能）
        /// </summary>
        public static async Task ClearLogAsync()
        {
            var logFile = await _logFolder.TryGetItemAsync(_logFileName) as StorageFile;
            if (logFile != null)
            {
                await FileIO.WriteTextAsync(logFile, string.Empty);
            }
        }
    }
}