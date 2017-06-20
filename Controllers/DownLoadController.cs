using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Service;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using LanShanPRMS.IService;

namespace LanShanPRMS.Controllers
{
    public class DownLoadController : Controller
    {
        private readonly IAnnexService _annex = new AnnexService();
        private readonly IToolService _tool = new ToolService();
        public ActionResult Tool(int id)
        {
            var tool = _tool.GetModel(id);
            if (tool == null || string.IsNullOrWhiteSpace(tool.FileUrl))
            {
                ViewBag.Message = "未找到对应文件地址";
                return View("404");
            }
            var path = ConfigHelper.ServerPath + tool.FileUrl;
            if (!System.IO.File.Exists(path))
            {
                ViewBag.Message = "未找到对应文件";
                return View("404");
            }
            tool.Hits += 1;
            _tool.SaveOrEdit(tool);
            path = Path.Combine(HttpRuntime.AppDomainAppPath, tool.FileUrl);
            var ex = Path.GetExtension(path);
            if (tool.FileName.EndsWith(ex))
                return File(path, "application/octet-stream", Path.GetFileName(tool.FileName ?? DateTime.Now.ToString("yyyyMMdd")));
            return File(path, "application/octet-stream", Path.GetFileName(tool.FileName ?? DateTime.Now.ToString("yyyyMMdd")) + ex);
        }
        public ActionResult Annex(int id)
        {
            var tool = _annex.GetModel(id);
            if (tool == null || string.IsNullOrWhiteSpace(tool.AnnexPath))
            {
                ViewBag.Message = "未找到对应文件地址";
                return View("404");
            }
            var path = ConfigHelper.ServerPath + tool.AnnexPath;
            if (!System.IO.File.Exists(path))
            {
                ViewBag.Message = "未找到对应文件";
                return View("404");
            }
            path = Path.Combine(HttpRuntime.AppDomainAppPath, tool.AnnexPath);
            var ex = Path.GetExtension(path);
            if (tool.AnnexName.EndsWith(ex))
                return File(path, "application/octet-stream", Path.GetFileName(tool.AnnexName ?? DateTime.Now.ToString("yyyyMMdd")));
            return File(path, "application/octet-stream", Path.GetFileName(tool.AnnexName ?? DateTime.Now.ToString("yyyyMMdd")) + ex);

        }
    }
}
