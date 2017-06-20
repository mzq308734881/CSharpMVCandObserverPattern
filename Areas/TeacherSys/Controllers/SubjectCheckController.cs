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
    public class SubjectCheckController : BaseTeacherController
    {
        //
        // GET: /PRMS/Student/
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ITeacherService _teacher = new TeacherService();
        
        public ActionResult Index(int? id)
        {
            VerificationViewModel.DeleteCookie();
            var teacher = _teacher.LoginTeacher();
            var list = _subject.GetTeacherUnPassSubjects(teacher.ID);
            return View(_subject.GetPageList(list, id, 10));
        }
        
        public ActionResult Search(int? page)
        {
            var list = _subject.GetTeacherUnPassSubjects(ConfigHelper.TeacherID);

            string keyword = "";
            if (page != null)
                keyword = StringHelper.ClearFormat(CookieHelper.GetValue("keyword"));
            else
                CookieHelper.Del("keyword");
            if (string.IsNullOrWhiteSpace(keyword))
                return View("search", _subject.GetPageList(list, page ?? 1));

            list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) orderby c.SubjectState select c;
            return View("search", _subject.GetPageList(list, page ?? 1));
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var list = _subject.GetTeacherUnPassSubjects(ConfigHelper.TeacherID);
            if (string.IsNullOrWhiteSpace(keyword))
                return View("search", _subject.GetPageList(list));

            keyword = StringHelper.ClearFormat(keyword);
            CookieHelper.SetObj("keyword", keyword);

            list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) orderby c.SubjectState select c;
            var pager = _subject.GetPageList(list);
            return View("search", pager);
        }
        //public ActionResult Add()
        //{
        //    var model = new Subject();
        //    var teacher = _teacher.LoginTeacher();
        //    if (teacher.ID == 0)
        //        return Content("<script>alert('身份验证失败！');window.location.reload();");
        //    var schoollist = _school.GetTeacherSchoolByType(4);
        //    ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
        //    return View(model);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult Add(Subject model, FormCollection fc)
        //{
        //    var teacher = _teacher.LoginTeacher();
        //    model.SchoolID = teacher.SchoolID ?? 0;
        //    model.CreateInfoID = teacher.InfoID;
        //    model.TeacherID = teacher.InfoID;
        //    _subject.SaveTeacher(model);
        //    return RedirectToAction("search");
        //}

        //public ActionResult Edit(int id)
        //{
        //    var teacher = _teacher.LoginTeacher();
        //    if (teacher.ID == 0)
        //        return Content("<script>alert('身份验证失败！');window.location.reload();");
        //    var model = _subject.GetModel(id);
        //    if (teacher.ID != model.TeacherID)
        //        return Content("<script>alert('课题导师不匹配！');window.location.reload();");
        //    var schoollist = _school.GetTeacherSchoolByType(4);
        //    ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
        //    return View(model);
        //}
        //[HttpPost, ValidateInput(false)]
        //public ActionResult Edit(int id, Subject model, FormCollection fc)
        //{
        //    model = _subject.GetModel(id);
        //    UpdateModel(model);
        //    _subject.SaveTeacher(model);
        //    return RedirectToAction("search");
        //}

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
