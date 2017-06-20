using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class StudentCheckController : BaseTeacherController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IOpeningReportService _open = new OpeningReportService();
        private readonly IChooseSubjectTeacherService _choose = new ChooseSubjectTeacherService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly IReviewTeacherService _review = new ReviewTeacherService();
        public ActionResult Search()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID) ?? new Treatise();
            var model = _choose.StudentList(trea, ConfigHelper.TeacherID);
            return View(model);
        }
        public ActionResult Check(int id)
        {
            var model = _open.GetModel(id);
            if (ConfigHelper.TeacherID != model.TeacherID)
                return Content("<script>alert('无法审核其他导师的选题！');window.location.reload();");
            var school = _school.GetParendSchoolByType(Teacher.SchoolID ?? Teacher?.UserInfo?.SchoolID, 1) ?? new School();
            var teacherlist = _teacher.GetSchoolTeacher(school.ID);
            ViewBag.ReviewID = teacherlist.ToSelectList(d => d.UserInfo.TrueName, d => d.ID.ToString(), "请选择");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Check(int id, OpeningReport model)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            model = _open.GetModel(id);
            if (model.TreatiseStudent == null || (model.TreatiseStudent.CheckState != 1 && model.TreatiseStudent.CheckState != 0 && model.TreatiseStudent.CheckState != 2))
                return Content("<script>alert('课题已审核或暂未提交！');window.location.reload();");
            UpdateModel(model);
            model.TeacherTime = DateTime.Now;
            switch (model.CollegeState)
            {
                case 1:
                    if (trea.AdminChoose == 1)
                    {
                        model.TreatiseStudent.CheckState = 2;
                    }
                    else
                    {
                        _review.Delete(d => d.TreatiseStudentID == model.TreatiseStudentID);
                        model.CollegeState = 2;
                        if (!string.IsNullOrWhiteSpace(model.Title))
                            model.TreatiseStudent.Title = model.Title;
                        model.TreatiseStudent.TreatiseSubjectID = model.TreatiseSubjectID;
                        model.TreatiseStudent.CheckState = 3;
                        model.TreatiseStudent.TeacherID = model.TeacherID;
                        model.TreatiseStudent.ReviewTeachers.Add(
                           new ReviewTeacher()
                           {
                               CreateInfoID = Teacher.InfoID,
                               CreateUserType = (int)UserInfoType.Teacher,
                               CreateTime = DateTime.Now,
                               TeacherID = model.ReviewID,
                               TreatiseStudentID = model.TreatiseStudentID
                           });
                    }
                    break;
                case 3:
                    if (model.TreatiseStudent.OpeningReports.All(d => d.CollegeState == 3))
                        model.TreatiseStudent.CheckState = 4;
                    break;
            }
            _open.SaveOrEdit(model);
            _choose.OpeningCheck(trea, model);
            return RedirectToAction("search");
        }

        public ActionResult Detail(int id)
        {
            var model = _open.GetModel(id);
            return View(model);
        }
    }
}
