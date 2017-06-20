using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class SubjectController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly IAnnexService _annex = new AnnexService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var list = _subject.GetUserSchoolSubjects(ConfigHelper.UserID);
            var pager = _subject.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(string keyword, int? id)
        {
            var list = _subject.GetUserSchoolSubjects(ConfigHelper.UserID);
            if (id != 0 && id != null)
            {
                keyword = CookieHelper.GetValue("keyword");
            }
            else
            {
                CookieHelper.Del("keyword");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list.Where(d => d.Title.Contains(keyword)) select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
            {
                CookieHelper.Del("keyword");    
            }
            list = list.OrderByDescending(d => d.ID);
            return View("index", _subject.GetPageList(list, id ?? 1));
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var list = _subject.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _subject.GetPageList(list);
            return View("index", pager);
        }
        public ActionResult Add()
        {
            var model = new Subject();
            var teacherlist = new List<Teacher>();
            var schoollist = _school.GetHaveTeacherSchool();
            ViewBag.SubjectState = SelectListHelper.SubjectStateList(model.SubjectState);
            ViewBag.TeacherID = teacherlist.ToSelectList(d => d.UserInfo.TrueName,
                   d => d.ID.ToString(),
                   "请先选择专业", model.TeacherID ?? 0);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Subject model, FormCollection fc, string filename, string filepath)
        {
            var user = _admin.LoginUser();
            if (model.SchoolID == 0) model.SchoolID = null;
            model.CheckContent = "管理员"+user.UserInfo.TrueName+"创建";
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(filepath))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            model.CheckTime = DateTime.Now;
            model.CheckUserID = user.ID;
            model.CreateInfoID = user.InfoID;
            model.SubjectState = 1;
            _subject.SaveAdmin(model);
            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var model = _subject.GetModel(id);
            var teacherlist = _teacher.GetSchoolTeacher(model.SchoolID??0);
            var schoollist = _school.GetHaveTeacherSchool();
            ViewBag.SubjectState = SelectListHelper.SubjectStateList(model.SubjectState);
            ViewBag.TeacherID = teacherlist.ToSelectList(d => d.UserInfo.TrueName,
                   d => d.ID.ToString(),
                   "请选择导师", model.TeacherID ?? 0);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            if (model.AnnexID != 0 && model.AnnexID != null)
                ViewBag.Annex = _annex.GetModel(model.AnnexID ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Subject model, FormCollection fc)
        {
            var user = _admin.LoginUser();
            model = _subject.GetModel(id);
            UpdateModel(model);
            if (model.SchoolID == 0) model.SchoolID = null;
            model.CheckTime = DateTime.Now;
            model.CheckUserID = user.ID;
            var name = Request.Form["FileName"];
            var path = Request.Form["FileUrl"];
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path))
            {
                var annex = new Annex() { AnnexName = name, AnnexPath = path };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            _subject.SaveAdmin(model);
            return RedirectToAction("index");
        }

        public ActionResult Check(int id)
        {
            var model = _subject.GetModel(id);
            var teacherlist = _teacher.GetSchoolTeacher(model.SchoolID ?? 0);
            var schoollist = _school.GetHaveTeacherSchool();
            ViewBag.SubjectState = SelectListHelper.SubjectStateList(model.SubjectState);
            ViewBag.TeacherID = teacherlist.ToSelectList(d => d.UserInfo.TrueName,d => d.ID.ToString(),"请选择导师", model.TeacherID ?? 0);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            if (model.AnnexID != 0 && model.AnnexID != null)
                ViewBag.Annex = _annex.GetModel(model.AnnexID ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Check(int id, Subject model, FormCollection fc)
        {
            var user = _admin.LoginUser();
            model = _subject.GetModel(id);
            UpdateModel(model);
            if (model.SchoolID == 0) model.SchoolID = null;
            model.CheckTime =DateTime.Now;
            model.CheckUserID = user.ID;
            var name = Request.Form["FileName"];
            var path = Request.Form["FileUrl"];
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path))
            {
                var annex = new Annex() { AnnexName = name, AnnexPath = path };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            _subject.SaveAdmin(model);
            return RedirectToAction("index");
        }
        public ActionResult Detail(int id)
        {
            var model = _subject.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _subject.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}
