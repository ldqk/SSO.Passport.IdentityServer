using System;
using Common;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Hardware;
using Masuit.Tools.Logging;
using Masuit.Tools.Win32;
using SSO.Passport.IdentityServer.Hubs;

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
                    double time = DateTime.Now.GetTotalMilliseconds();// - 28800000;
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

                    if (mem > 90)
                    {
                        Windows.ClearMemorySilent();
                    }
                    //缓存历史数据
                    if (CommonHelper.HistoryCpuLoad.Count < 50 || (time / 10000).ToInt32() % 12 == 0)
                    {
                        CommonHelper.HistoryCpuLoad.Add(new object[] { time, load });
                        CommonHelper.HistoryCpuTemp.Add(new object[] { time, temperature });
                        CommonHelper.HistoryMemoryUsage.Add(new object[] { time, mem });
                        CommonHelper.HistoryIORead.Add(new object[] { time, read });
                        CommonHelper.HistoryIOWrite.Add(new object[] { time, write });
                        CommonHelper.HistoryNetReceive.Add(new object[] { time, up });
                        CommonHelper.HistoryNetSend.Add(new object[] { time, down });
                        if (CommonHelper.HistoryCpuLoad.Count > 720)
                        {
                            CommonHelper.HistoryCpuLoad.RemoveAt(0);
                            CommonHelper.HistoryMemoryUsage.RemoveAt(0);
                            CommonHelper.HistoryCpuTemp.RemoveAt(0);
                        }
                        if (CommonHelper.HistoryIORead.Count > 720)
                        {
                            CommonHelper.HistoryIORead.RemoveAt(0);
                            CommonHelper.HistoryIOWrite.RemoveAt(0);
                            CommonHelper.HistoryNetReceive.RemoveAt(0);
                            CommonHelper.HistoryNetSend.RemoveAt(0);
                        }
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