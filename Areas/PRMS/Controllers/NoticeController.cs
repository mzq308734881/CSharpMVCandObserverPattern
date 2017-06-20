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
    public class NoticeController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ISchoolService _school = new SchoolService();
        private readonly INoticeService _notice = new NoticeService();
        private readonly IAnnexService _annex = new AnnexService();

        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var pager = _notice.GetPageList();
            return View(pager);
        }

        public ActionResult Search(int? id)
        {
            var keyword = "";
            if (id != 0 && id != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _notice.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _notice.GetPageList(list, id ?? 1);
            return View("index", pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _notice.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _notice.GetPageList(list);
            return View("index", pager);
        }

        public ActionResult Add()
        {
            var model = new Notice();
            ViewBag.Type = SelectListHelper.GetNoticeList(0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Notice model, string filename, string fileurl)
        {
            if (model.SchoolID == 0) model.SchoolID = null;
            if (_admin.IsSuperAdmin(Admin.InfoID))
                model.SchoolID = 1;
            else
            {
                if (Admin.School != null)
                    model.SchoolID = Admin.SchoolID;
                else
                    return Content("<script>alert('当前用户权限不足！请返回后重试！');window.location.href ='/prms/notice'</script>");
            }
            model.CreateDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(fileurl))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = fileurl };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            model.CreateUserID = Admin.ID;
            _notice.SaveOrEdit(model);
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int id)
        {
            var model = _notice.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _notice.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var model = _notice.GetModel(id);
            ViewBag.SchoolID = _school.GetAllList().ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业");
            ViewBag.Type = SelectListHelper.GetNoticeList(model.Type);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Notice model, string filename, string fileurl)
        {
            model = _notice.GetModel(id);
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(fileurl))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = fileurl };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            if (model == null) return View("404");
            UpdateModel(model);
            _notice.SaveOrEdit(model);
            return RedirectToAction("index");
        }
    }
}
