using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class SubjectController : BaseTeacherController
    {
        //
        // GET: /PRMS/Student/
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly IAnnexService _annex = new AnnexService();
        public ActionResult Index()
        {
            VerificationViewModel.DeleteCookie();
            var teacher = _teacher.LoginTeacher();
            var list = _subject.GetTeacherPassSubjects(teacher.ID);
            return View(_subject.GetPageList(list));
        }
        public ActionResult Search(int? page)
        {
            var teacher = _teacher.LoginTeacher();
            var list = _subject.GetTeacherPassSubjects(teacher.ID);
            CookieHelper.SetObj("pageIndex", page.ToString());
            string keyword = CookieHelper.GetValue("keyword");
            if (!string.IsNullOrEmpty(keyword))
                list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) select c;
            list = list.OrderByDescending(d => d.ID);
            var pager = _subject.GetPageList(list, page);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keywords)
        {
            var teacher = _teacher.LoginTeacher();
            var list = _subject.GetTeacherPassSubjects(teacher.ID);
            CookieHelper.Del("pageIndex");
            var keyword = StringHelper.ClearFormat(keywords);
            CookieHelper.SetObj("keyword", keyword);

            if (!string.IsNullOrEmpty(keyword))
                list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) select c;
            CookieHelper.SetObj("keyword", keyword);
            list = list.OrderByDescending(d => d.ID);
            var pager = _subject.GetPageList(list);
            return View(pager);
        }
        public ActionResult Add()
        {
            var model = new Subject();
            var teacher = _teacher.LoginTeacher();
            var schoollist = _school.GetTeacherSchoolByType(4, teacher);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Subject model, string filename, string filepath)
        {
            var teacher = _teacher.LoginTeacher();
            model.CreateInfoID = teacher.InfoID;
            model.TeacherID = teacher.ID;
            if (teacher.SchoolID != null)
                model.SchoolID = teacher.SchoolID;
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(filepath))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            _subject.SaveTeacher(model);
            return RedirectToAction("search");
        }
        public ActionResult Edit(int id)
        {
            var teacher = _teacher.LoginTeacher();
            if (teacher.ID == 0)
                return Content("<script>alert('身份验证失败！');window.location.reload();");
            var model = _subject.GetModel(id);
            if (teacher.ID != model.TeacherID)
                return Content("<script>alert('课题导师不匹配！');window.location.reload();");
            var schoollist = _school.GetTeacherSchoolByType(4, teacher);
            ViewBag.Annex = _annex.GetModel(model.AnnexID ?? 0);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Subject model, string filename, string filepath)
        {
            model = _subject.GetModel(id);
            UpdateModel(model);
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(filepath))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            _subject.SaveTeacher(model);
            return RedirectToAction("search");
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
