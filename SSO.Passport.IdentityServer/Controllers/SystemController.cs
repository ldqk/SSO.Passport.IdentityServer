using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using Masuit.Tools;
using Masuit.Tools.Hardware;
using Masuit.Tools.Win32;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class SystemController : BaseController
    {
        public async Task<ActionResult> GetBaseInfo()
        {
            List<CpuInfo> cpuInfo = SystemInfo.GetCpuInfo();
            RamInfo ramInfo = SystemInfo.GetRamInfo();
            string osVersion = SystemInfo.GetOsVersion();
            var total = new StringBuilder();
            var free = new StringBuilder();
            var usage = new StringBuilder();
            SystemInfo.DiskTotalSpace().ForEach(kv => { total.Append(kv.Key + kv.Value + " | "); });
            SystemInfo.DiskFree().ForEach(kv => free.Append(kv.Key + kv.Value + " | "));
            SystemInfo.DiskUsage().ForEach(kv => usage.Append(kv.Key + kv.Value.ToString("P") + " | "));
            IList<string> mac = SystemInfo.GetMacAddress();
            IList<string> ips = SystemInfo.GetIPAddress();
            var span = DateTime.Now - CommonHelper.StartupTime;
            var boot = DateTime.Now - SystemInfo.BootTime();

            return Content(await new { runningTime = $"{span.Days}天{span.Hours}小时{span.Minutes}分钟", bootTime = $"{boot.Days}天{boot.Hours}小时{boot.Minutes}分钟", cpuInfo, ramInfo, osVersion, diskInfo = new { total = total.ToString(), free = free.ToString(), usage = usage.ToString() }, netInfo = new { mac, ips } }.ToJsonStringAsync(), "application/json");
        }

        public ActionResult GetHistoryList()
        {
            return Json(new { cpu = CommonHelper.HistoryCpuLoad, mem = CommonHelper.HistoryMemoryUsage, temp = CommonHelper.HistoryCpuTemp, read = CommonHelper.HistoryIORead, write = CommonHelper.HistoryIOWrite, down = CommonHelper.HistoryNetReceive, up = CommonHelper.HistoryNetSend }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CollectMemory()
        {
            double p = Windows.ClearMemory();
            return ResultData(null, true, "内存整理成功，当前内存使用率：" + p.ToString("N") + "%");
        }
    }
}