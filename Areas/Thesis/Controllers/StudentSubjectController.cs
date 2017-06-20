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

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class StudentSubjectController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly IOpeningReportService _open = new OpeningReportService();
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly IChooseSubjectAdminService _choose = new ChooseSubjectAdminService();
        private readonly IReviewTeacherService _review = new ReviewTeacherService();
        private readonly ISchoolService _school = new SchoolService();
        public ActionResult Search(int? page)
        {
            var stuno = "";
            var stuname = "";
            if (page == null || page < 1)
            {
                page = 1;
                CookieHelper.Del("stuno");
                CookieHelper.Del("stuname");
            }
            else
            {
                stuno = StringHelper.ClearFormat(CookieHelper.GetValue("stuno"));
                stuname = StringHelper.ClearFormat(CookieHelper.GetValue("stuname"));
            }
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            var model = _choose.StudentPage(trea, stuname, stuno, page ?? 1);
            return trea != null ? View(model) : View("404");
        }
        [HttpPost]
        public ActionResult Search(string stuno, string stuname)
        {
            var id = ConfigHelper.TreatiseID;
            if (!string.IsNullOrWhiteSpace(stuno))
                CookieHelper.SetObj("stuno", stuno);
            else
                CookieHelper.Del("stuno");
            if (!string.IsNullOrWhiteSpace(stuname))
                CookieHelper.SetObj("stuname", stuname);
            else
                CookieHelper.Del("stuname");

            var trea = _treatise.GetModel(id);
            var model = _choose.StudentPage(trea, stuname, stuno);
            return trea != null ? View(model) : View("404");
        }
        public ActionResult Check(int? id)
        {
            var model = _open.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！')</script>");
            ViewData["CollegeState"] = SelectListHelper.GetTreatiseOpeningCheckStateList(model.CollegeState ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Check(int? id, OpeningReport model)
        {
            model = _open.GetModel(id ?? 0);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (model?.TreatiseStudent == null)
                return Content("<script>alert('参数错误！请返回后重试！')</script>");
            UpdateModel(model);
            if (model.CollegeState == 2)
            {
                _review.Delete(d => d.TreatiseStudentID == model.TreatiseStudentID);
                model.TreatiseStudent.TeacherID = model.TeacherID;
                if (!string.IsNullOrWhiteSpace(model.Title))
                    model.TreatiseStudent.Title = model.Title;
                model.TreatiseStudent.TreatiseSubjectID = model.TreatiseSubjectID;
                model.TreatiseStudent.CheckState = 3;
                model.TreatiseStudent.ReviewTeachers.Add(
                         new ReviewTeacher()
                         {
                             CreateInfoID = Admin.InfoID,
                             CreateUserType = (int)UserInfoType.Admin,
                             CreateTime = DateTime.Now,
                             TeacherID = model.ReviewID,
                             TreatiseStudentID = model.TreatiseStudentID
                         });
            }
            _open.SaveOrEdit(model);
            if (model.CollegeState == 2)
                _choose.OpeningRefused(trea, model);
            var list = _choose.StudentPage(trea);
            return View("Search", list);
        }
        public ActionResult Change(int? id)
        {
            var model = _tStudent.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！')</script>");
            var subjectlist = _tSubject.GetTreatisePassList(model.TreatiseID ?? 0);
            var subtea = (from c in subjectlist select c.TeacherID).Distinct().ToList();
            var teacher = _teacher.GetAllList().Where(d => subtea.Contains(d.ID));
            ViewBag.TeacherID = teacher.ToSelectList(d => d.UserInfo.TrueName, d => d.ID.ToString(), "请选择", model.TeacherID);
            if (_admin.IsSuperAdmin())
            {
                var teacherlist = _teacher.GetAllList();
                ViewBag.ReviewID = teacherlist.ToSelectList(d => d.UserInfo.TrueName, d => d.ID.ToString(), "请选择");
            }
            else
            {
                var school = _school.GetParendSchoolByType(Admin.SchoolID ?? Admin?.UserInfo?.SchoolID, 1) ?? new School();
                var teacherlist = _teacher.GetSchoolTeacher(school.ID);
                ViewBag.ReviewID = teacherlist.ToSelectList(d => d.UserInfo.TrueName, d => d.ID.ToString(), "请选择");
            }
            subjectlist = subjectlist.Where(d => d.TeacherID == model.TeacherID);
            ViewBag.TreatiseSubjectID = subjectlist.ToSelectList(d => d.Subject.Title, d => d.ID.ToString(), "请先选择导师！", model.TreatiseSubjectID);
            return View(model);
        }
        [HttpPost]
        public ActionResult Change(int? id, TreatiseStudent model, int reviewid)
        {
            var user = _admin.LoginUser();
            model = _tStudent.GetModel(id ?? 0);
            if (model == null) return Content("<script>alert('参数错误！请返回后重试！')</script>");
            UpdateModel(model);
            var treatisesubject = _tSubject.GetModel(model.TreatiseSubjectID ?? 0);
            if (treatisesubject == null) return Content("<script>alert('参数错误！请返回后重试！')</script>");
            model.TreatiseSubjectID = treatisesubject.ID;
            var open = new OpeningReport()
            {
                StudentID = model.StudentID,
                TreatiseStudentID = model.ID,
                TreatiseSubjectID = treatisesubject.ID,
                TeacherID = model.TeacherID,
                TeacherComment = "管理员指定",
                TeacherTime = DateTime.Now,
                CollegeUerID = user.ID,
                CollegeComment = "管理员指定",
                CollegeTime = DateTime.Now,
                CollegeState = 2,
                ReviewID = reviewid,
            };
            _open.SaveOrEdit(open);
            _review.Delete(d => d.TreatiseStudentID == model.ID);
            model.ReviewTeachers.Add(
                        new ReviewTeacher()
                        {
                            CreateInfoID = Admin.InfoID,
                            CreateUserType = (int)UserInfoType.Admin,
                            CreateTime = DateTime.Now,
                            TeacherID = reviewid,
                            TreatiseStudentID = model.ID
                        });
            model.CheckState = 3;
            _tStudent.SaveOrEdit(model);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            _choose.OpeningRefused(trea, open);
            var list = _choose.StudentPage(trea);
            return View("Search", list);
        }
        public ActionResult SubjectComment(int? id)
        {
            var model = _tSubject.GetModel(id ?? 0);
            return View(model);
        }
        public ActionResult Detail(int? id)
        {
            var model = _open.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }

        public ActionResult Import()
        {
            throw new Exception("此功能暂未开放！");
        }
    }
}
