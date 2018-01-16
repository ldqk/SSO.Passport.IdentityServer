using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Common;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.Hardware;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetBaseInfo()
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
            return Json(new { cpuInfo, ramInfo, osVersion, diskInfo = new { total = total.ToString(), free = free.ToString(), usage = usage.ToString() }, netInfo = new { mac, ips } }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHistoryList()
        {
            return Json(new { cpu = CommonHelper.HistoryCpuLoad, mem = CommonHelper.HistoryMemoryUsage, temp = CommonHelper.HistoryCpuTemp, read = CommonHelper.HistoryIORead, write = CommonHelper.HistoryIOWrite, down = CommonHelper.HistoryNetReceive, up = CommonHelper.HistoryNetSend }, JsonRequestBehavior.AllowGet);
        }
    }
}