using Chailease.SolarEnergy.Model;
using Chailease.SolarEnergy.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chailease.SolarEnergy.Web.Controllers
{
    public class FileController : Controller
    {
        [HttpGet]
        public FileContentResult Image(String uri)
        {
            String contentType = "";
            String fileName = "";
            DownloadFileRDto file;
            try
            {
                FileRepository fileRespository = new FileRepository();
                file = fileRespository.Download(uri);
                fileName = file.FileName;
                contentType = getImageContentType(fileName);
                if (String.IsNullOrEmpty(contentType))
                    throw new Exception("this is not an image file ");
            }
            catch (Exception ex) {
                return null;
            }
            return new FileContentResult(file.File, contentType);
        }
        public String getImageContentType(String filename) {
            String ends = Path.GetExtension(filename);
            String contentType = "";
            #region get type
            switch (ends.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".jpe":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/x-png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".svf":
                    contentType = "image/vnd.svf";
                    break;
            }
            #endregion
            return contentType;
        }
    }
}