using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class SubjectChooseController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly IOpeningReportService _opening = new OpeningReportService();
        private readonly IChooseSubjectStudentService _choose = new ChooseSubjectStudentService();
        public ActionResult ChooseTeacher()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();</script>");
            var tstudent = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (tstudent == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();</script>");
            var model = _choose.ChooseTeacher(trea, tstudent);
            if (model.State != 1&&model.State!=4)
                return Content("<script>alert('您已完成选题或选题审核中！');window.location.reload();</script>");
            if (model.ChooseTeacher) return View(model);
            model = _choose.ChooseSubject(trea, tstudent, null);
            return View("ChooseSubject", model);
        }
        public ActionResult ChooseSubject(int? id)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();</script>");
            var tstudent = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (tstudent == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();</script>");
            var model = _choose.ChooseSubject(trea, tstudent, id);
            return View(model);
        }
        public ActionResult Subject(int id)
        {
            var model = _tSubject.GetModel(id);
            return View(model);
        }
        public ActionResult SubmitSubject(int id)
        {
            var model = _tSubject.GetModel(id);
            var trea = _treatise.GetModel(model.TreatiseID ?? 0);
            var studentid = ConfigHelper.StudentID;
            var tstudent = _tStudent.GetModel(trea.ID, studentid);
            var state = _choose.StudentChooseState(trea, tstudent);
            if (state != 1&& state != 4)
                return Content("<script>alert('您已完成选题或选题审核中！请勿重复选题！');window.location.reload();</script>");

            var opening = _choose.NewOpeningReport(trea, tstudent, model, studentid);
            if (trea.IsReport == 1)
            {
                opening.TreatiseSubject = model;
                return View("OpeningReport", opening);
            }
            else
            {
                _opening.SaveOrEdit(opening);
                tstudent.CheckState = 1;
                _tStudent.SaveOrEdit(tstudent);
            }
            return Content("<script>alert('提交成功！等待导师审核！');window.location.reload();</script>");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult OpeningReport(int id, OpeningReport open)
        {
            var model = _tSubject.GetModel(id);
            var studentid = ConfigHelper.StudentID;
            var trea = _treatise.GetModel(model.TreatiseID ?? 0);
            var tstudent = _tStudent.GetModel(trea.ID, studentid);
            var state = _choose.StudentChooseState(trea, tstudent);
            if (state != 1 && state != 4)
                return Content("<script>alert('您已完成选题或选题审核中！请勿重复选题！');window.location.reload();</script>");
            _choose.SubmitOpeningReport(trea,tstudent,model,open,studentid);
            return Content("<script>alert('提交成功！等待导师审核！');window.location.reload();</script>");
        }
    }
}
