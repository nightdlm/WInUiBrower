using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinUiBrowser.Utils
{
    internal class ProcessManagerUtil
    {
        // 用于管理所有作业对象的字典
        private static readonly Dictionary<string, IntPtr> jobHandles = new Dictionary<string, IntPtr>();
        private static readonly Dictionary<string, List<Process>> jobProcesses = new Dictionary<string, List<Process>>();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(IntPtr hJob, int JobObjectInfoClass, ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateJobObject(IntPtr hJob, uint uExitCode);

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public UIntPtr Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

        /// <summary>
        /// 启动一个带作业对象的进程，并将输出通过回调函数处理
        /// </summary>
        /// <param name="jobKey">作业对象的标识键</param>
        /// <param name="fileName">要执行的程序文件名</param>
        /// <param name="arguments">程序参数</param>
        /// <param name="outputCallback">用于处理标准输出的回调函数</param>
        /// <param name="errorCallback">用于处理错误输出的回调函数</param>
        public static void StartProcessWithJobObject(string jobKey, string workingDir , string fileName, string arguments,
            Action<string> outputCallback = null, Action<string> errorCallback = null)
        {
            // 如果指定键的作业对象不存在，则创建一个新的
            if (!jobHandles.ContainsKey(jobKey))
            {
                // 创建作业对象
                IntPtr jobHandle = CreateJobObject(IntPtr.Zero, null);

                // 设置作业对象信息，确保在作业对象关闭时终止所有相关进程
                var jobInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
                {
                    BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                    {
                        LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
                    }
                };

                SetInformationJobObject(jobHandle, 9, ref jobInfo, (uint)Marshal.SizeOf(jobInfo));

                // 将作业对象添加到字典中
                jobHandles[jobKey] = jobHandle;
                jobProcesses[jobKey] = new List<Process>();
            }

            // 获取作业对象句柄
            IntPtr currentJobHandle = jobHandles[jobKey];

            // 配置进程启动信息
            var processInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDir,
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = processInfo
            };

            // 订阅输出事件
            //process.OutputDataReceived += (sender, e) =>
            //{
            //    //if (!string.IsNullOrEmpty(e.Data))
            //    //{
            //    //    // 使用WinUI的DispatcherQueue来更新UI
            //    //    Windows.ApplicationModel.Core.CoreApplication.MainView.DispatcherQueue.TryEnqueue(() =>
            //    //    {
            //    //        outputCallback?.Invoke(e.Data);
            //    //    });
            //    //}
            //};

            //process.ErrorDataReceived += (sender, e) =>
            //{
            //    //if (!string.IsNullOrEmpty(e.Data))
            //    //{
            //    //    Windows.ApplicationModel.Core.CoreApplication.MainView.DispatcherQueue.TryEnqueue(() =>
            //    //    {
            //    //        errorCallback?.Invoke(e.Data);
            //    //    });
            //    //}
            //};

            // 启动进程
            process.Start();

            // 将进程添加到对应的列表中
            jobProcesses[jobKey].Add(process);

            // 异步读取输出
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();

            // 将进程分配给作业对象
            AssignProcessToJobObject(currentJobHandle, process.Handle);
        }

        /// <summary>
        /// 关闭指定作业对象及其所有相关进程
        /// </summary>
        /// <param name="jobKey">要关闭的作业对象的标识键</param>
        public static void CloseJobObject(string jobKey)
        {
            if (jobHandles.ContainsKey(jobKey))
            {
                IntPtr jobHandle = jobHandles[jobKey];

                // 关闭作业对象句柄，这将终止所有相关进程
                CloseHandle(jobHandle);

                // 从字典中移除
                jobHandles.Remove(jobKey);

                // 清理进程列表
                if (jobProcesses.ContainsKey(jobKey))
                {
                    jobProcesses.Remove(jobKey);
                }
            }
        }

        /// <summary>
        /// 强制终止指定作业对象及其所有相关进程
        /// </summary>
        /// <param name="jobKey">要终止的作业对象的标识键</param>
        public static void TerminateJobObject(string jobKey)
        {
            if (jobHandles.ContainsKey(jobKey))
            {
                IntPtr jobHandle = jobHandles[jobKey];

                // 强制终止作业对象中的所有进程
                TerminateJobObject(jobHandle, 0);

                // 关闭作业对象句柄
                CloseHandle(jobHandle);

                // 从字典中移除
                jobHandles.Remove(jobKey);

                // 清理进程列表
                if (jobProcesses.ContainsKey(jobKey))
                {
                    jobProcesses.Remove(jobKey);
                }
            }
        }

        /// <summary>
        /// 检查指定作业对象中的进程是否仍在运行
        /// </summary>
        /// <param name="jobKey">作业对象的标识键</param>
        /// <returns>如果作业对象存在且其中至少有一个进程仍在运行则返回true，否则返回false</returns>
        public static bool IsJobRunning(string jobKey)
        {
            // 检查作业对象是否存在
            if (!jobHandles.ContainsKey(jobKey) || !jobProcesses.ContainsKey(jobKey))
            {
                return false;
            }

            // 获取该作业对象关联的进程列表
            List<Process> processes = jobProcesses[jobKey];

            // 检查是否有任何进程仍在运行
            foreach (Process process in processes.ToList())
            {
                try
                {
                    // 如果进程已经退出，移除它
                    if (process.HasExited)
                    {
                        processes.Remove(process);
                    }
                }
                catch
                {
                    // 如果无法访问进程信息，假设它已退出并移除它
                    processes.Remove(process);
                }
            }

            // 如果仍有进程在运行，返回true
            return processes.Count > 0;
        }

        /// <summary>
        /// 异步等待指定作业对象中的所有进程正常退出
        /// </summary>
        /// <param name="jobKey">作业对象的标识键</param>
        /// <returns>Task表示等待操作</returns>
        public static async Task WaitForJobExitAsync(string jobKey)
        {
            await Task.Run(() => WaitForJobExit(jobKey));
        }

        /// <summary>
        /// 等待指定作业对象中的所有进程正常退出
        /// </summary>
        /// <param name="jobKey">作业对象的标识键</param>
        /// <param name="timeoutMilliseconds">超时时间（毫秒），-1表示无限等待</param>
        /// <returns>如果所有进程正常退出返回true，超时返回false</returns>
        private static bool WaitForJobExit(string jobKey, int timeoutMilliseconds = -1)
        {
            if (!jobHandles.ContainsKey(jobKey) || !jobProcesses.ContainsKey(jobKey))
            {
                return true; // 作业不存在，认为已退出
            }

            List<Process> processes = jobProcesses[jobKey];

            if (processes.Count == 0)
            {
                return true; // 没有进程，认为已退出
            }

            // 等待所有进程退出
            var processArray = processes.ToArray();
            foreach (var process in processArray)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        if (timeoutMilliseconds >= 0)
                        {
                            return process.WaitForExit(timeoutMilliseconds);
                        }
                        else
                        {
                            process.WaitForExit();
                        }
                    }
                }
                catch
                {
                    // 忽略无法等待的进程
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// 关闭所有作业对象及其相关进程
        /// </summary>
        public static void CloseAllJobObjects()
        {
            foreach (var kvp in jobHandles)
            {
                CloseHandle(kvp.Value);
            }

            jobHandles.Clear();
            jobProcesses.Clear();
        }

        /// <summary>
        /// 检查指定端口是否被占用
        /// </summary>
        /// <param name="port">要检查的端口号</param>
        /// <returns>如果端口被占用返回true，否则返回false</returns>
        public static bool IsPortInUse(int port)
        {
            try
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endpoints = ipProperties.GetActiveTcpListeners();
                return endpoints.Any(endpoint => endpoint.Port == port);
            }
            catch
            {
                // 如果无法获取端口信息，假设端口未被占用
                return false;
            }
        }

        /// <summary>
        /// 等待指定端口被占用（阻塞等待服务启动）
        /// </summary>
        /// <param name="port">要等待的端口号</param>
        /// <param name="timeoutMilliseconds">超时时间（毫秒），-1表示无限等待</param>
        /// <param name="checkInterval">检查间隔（毫秒），默认500ms</param>
        /// <returns>如果端口在超时时间内被占用返回true，超时返回false</returns>
        public static bool WaitForPortOccupied(int port, int timeoutMilliseconds = -1, int checkInterval = 500)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                // 检查端口是否被占用
                if (IsPortInUse(port))
                {
                    return true;
                }

                // 检查是否超时
                if (timeoutMilliseconds >= 0 && stopwatch.ElapsedMilliseconds >= timeoutMilliseconds)
                {
                    return false;
                }

                // 等待一段时间后再次检查
                System.Threading.Thread.Sleep(Math.Min(checkInterval, 1000));
            }
        }

        /// <summary>
        /// 异步等待指定端口被占用（等待服务启动）
        /// </summary>
        /// <param name="port">要等待的端口号</param>
        /// <param name="timeoutMilliseconds">超时时间（毫秒），-1表示无限等待</param>
        /// <param name="checkInterval">检查间隔（毫秒），默认500ms</param>
        /// <returns>Task表示等待操作，如果端口在超时时间内被占用返回true，超时返回false</returns>
        public static async Task<bool> WaitForPortOccupiedAsync(int port, int timeoutMilliseconds = -1, int checkInterval = 500)
        {
            return await Task.Run(() => WaitForPortOccupied(port, timeoutMilliseconds, checkInterval));
        }
    }
}