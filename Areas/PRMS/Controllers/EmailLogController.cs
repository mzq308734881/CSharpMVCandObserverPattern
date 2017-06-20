using System;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class EmailLogController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly IEmailLogService _emaillog = new EmailLogService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var pager = _emaillog.GetPageList();
            return View(pager);
        }
        public ActionResult Search(int? id)
        {
            var keyword = "";
            if (id != 0 && id != null)
            {
                keyword = CookieHelper.GetValue("keyword");
            }
            else
            {
                CookieHelper.Del("keyword");
            }
            var list = _emaillog.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _emaillog.GetPageList(list, id ?? 1);
            return View("index", pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword, string toolType)
        {
            var list = _emaillog.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _emaillog.GetPageList(list);
            return View("index", pager);
        }

        public ActionResult Detail(int id)
        {
            var model = _emaillog.GetModel(id);
            return View(model);
        }
    }
}
