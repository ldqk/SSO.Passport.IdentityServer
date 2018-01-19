using System;
using System.IO;
using System.Web.Mvc;
using Masuit.Tools.Logging;
using Masuit.Tools.Media;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UploadController : BaseController
    {
        public ActionResult DecodeDataUri(string data)
        {
            var dir = "/upload/images";
            var filename = Guid.NewGuid() + ".jpg";
            string path = Path.Combine(dir, filename);
            try
            {
                string physicsDir = Request.MapPath(dir);
                if (!Directory.Exists(physicsDir))
                {
                    Directory.CreateDirectory(physicsDir);
                }
                data.SaveDataUriAsImageFile().Save(Request.MapPath(path), System.Drawing.Imaging.ImageFormat.Jpeg);
                return ResultData(dir + "/" + filename);
            }
            catch (Exception e)
            {
                LogManager.Error(e);
                return ResultData(null, false, "转码失败！");
            }
        }
    }
}