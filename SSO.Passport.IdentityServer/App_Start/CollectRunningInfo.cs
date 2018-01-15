using System;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Hardware;
using Masuit.Tools.Logging;
using SSO.Passport.IdentityServer.Hubs;
using static Common.CommonHelper;

namespace SSO.Passport.IdentityServer
{
    /// <summary>
    /// 收集系统运行状态
    /// </summary>
    public class CollectRunningInfo
    {
        public static void Start()
        {

            MyHub.PushData(a =>
            {
                try
                {
                    double time = DateTime.Now.GetTotalMilliseconds();
                    float load = SystemInfo.CpuLoad;
                    double temperature = SystemInfo.GetCPUTemperature();
                    double mem = (1 - SystemInfo.MemoryAvailable.To<double>() / SystemInfo.PhysicalMemory.To<double>()) * 100;
                    a.receiveLoad($"[{time},{load},{mem},{temperature}]");//CPU

                    var read = SystemInfo.GetDiskData(DiskData.Read) / 1024;
                    var write = SystemInfo.GetDiskData(DiskData.Write) / 1024;
                    a.receiveReadWrite($"[{time},{read},{write}]");//磁盘IO

                    var up = SystemInfo.GetNetData(NetData.Received) / 1024;
                    var down = SystemInfo.GetNetData(NetData.Sent) / 1024;
                    a.receiveUpDown($"[{time},{down},{up}]");//网络上下载

                    //缓存历史数据
                    HistoryCpuLoad.Add(new object[] { time, load });
                    HistoryCpuTemp.Add(new object[] { time, temperature });
                    HistoryMemoryUsage.Add(new object[] { time, mem });
                    HistoryIORead.Add(new object[] { time, read });
                    HistoryIOWrite.Add(new object[] { time, write });
                    HistoryNetReceive.Add(new object[] { time, up });
                    HistoryNetSend.Add(new object[] { time, down });
                    if (HistoryCpuLoad.Count > 80)
                    {
                        HistoryCpuLoad.RemoveAt(0);
                        HistoryMemoryUsage.RemoveAt(0);
                        HistoryCpuTemp.RemoveAt(0);
                    }
                    if (HistoryIORead.Count > 50)
                    {
                        HistoryIORead.RemoveAt(0);
                        HistoryIOWrite.RemoveAt(0);
                        HistoryNetReceive.RemoveAt(0);
                        HistoryNetSend.RemoveAt(0);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Error(e);
                }
            });
        }
    }
}