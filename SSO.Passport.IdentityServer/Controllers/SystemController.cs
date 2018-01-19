using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using Masuit.Tools;
using Masuit.Tools.Hardware;
using Masuit.Tools.Logging;
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

        /// <summary>
        /// 获取服务器硬件监控数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHistoryList()
        {
            return Json(new { cpu = CommonHelper.HistoryCpuLoad, mem = CommonHelper.HistoryMemoryUsage, temp = CommonHelper.HistoryCpuTemp, read = CommonHelper.HistoryIORead, write = CommonHelper.HistoryIOWrite, down = CommonHelper.HistoryNetReceive, up = CommonHelper.HistoryNetSend }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 整理系统内存
        /// </summary>
        /// <returns></returns>
        public ActionResult CollectMemory()
        {
            double p = Windows.ClearMemory();
            return ResultData(null, true, "内存整理成功，当前内存使用率：" + p.ToString("N") + "%");
        }

        /// <summary>
        /// 获取网站日志文件
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLogfiles()
        {
            List<string> files = Directory.GetFiles(LogManager.LogDirectory).OrderByDescending(s => s).Select(Path.GetFileName).ToList();
            return ResultData(files);
        }

        /// <summary>
        /// 查看日志
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ActionResult Catlog(string filename)
        {
            if (System.IO.File.Exists(Path.Combine(LogManager.LogDirectory, filename)))
            {
                string text = System.IO.File.ReadAllText(Path.Combine(LogManager.LogDirectory, filename));
                return ResultData(text);
            }
            return ResultData(null, false, "文件不存在！");
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(string filename)
        {
            try
            {
                System.IO.File.Delete(Path.Combine(LogManager.LogDirectory, filename));
                return ResultData(null, message: "文件删除成功!");
            }
            catch
            {
                return ResultData(null, false, "文件删除失败！");
            }
        }

    }
}