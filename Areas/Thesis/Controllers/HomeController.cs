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
    public class HomeController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        public ActionResult Index(int? id)
        {
            VerificationViewModel.DeleteCookie();

            if (id == null || id == 0)
                id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id ?? 0);
            var list = _treatise.GetAllList();
            if (trea == null || trea.ID == 0)
                trea = list.FirstOrDefault(d => d.TreatiseStage == 1) ?? list.FirstOrDefault();
            
            if (trea == null) trea = new Treatise();
            var model = new TreatiseConfigViewModel(trea)
            {
                ProcessList = trea.TreatiseProcesses.Where(d => d.State == 1).ToList(),
                TreatiseList = list.ToList(),
                UnCount =
                    _tStudent.GetTreatiseList(trea.ID).Count(d => d.TreatiseSubject == null && !d.OpeningReports.Any()),
                StudentCount = _tStudent.GetTreatiseList(trea.ID).Count(),
                TeacherCount = _tTeacher.GetTreatiseList(trea.ID).Count(),
                SubjectCount = _tSubject.GetTreatisePassList(trea.ID).Count()
            };
            ConfigHelper.SetTreatiseID(trea.ID);
            return View(model);
        }
        public ActionResult Change(int id)
        {
            var model = _treatise.GetModel(id);
            if (model == null)
                return Json(new RootViewModel() { Message = "无法找到对应项目，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
            ConfigHelper.SetTreatiseID(model.ID);
            return Json(new RootViewModel() { State = 1, Message = "等待页面重新载入……" }, JsonRequestBehavior.AllowGet);
        }
    }
}
