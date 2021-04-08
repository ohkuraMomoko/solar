using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace Chailease.SolarEnergy.Web.Commons
{
    public class UploadHelpers
    {
        private readonly string basePath;
        public UploadHelpers()
        {
            basePath = ConfigurationManager.AppSettings["WebsiteRootUploadUrl"]?.ToString();
        }

        public string Register(HttpPostedFileBase File)
        {
            return Save(File, "RealRegister");
        }

        public string Default(HttpPostedFileBase File)
        {
            return Save(File, "Default");
        }

        public string Save(HttpPostedFileBase File, string Folder)
        {
            if (File.ContentLength > 0)
            {
                var path = Path.Combine(basePath, Folder);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var imgName = "MVC_" + Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);
                File.SaveAs(Path.Combine(path, imgName));
                return imgName;
            }
            return string.Empty;
        }
    }
}