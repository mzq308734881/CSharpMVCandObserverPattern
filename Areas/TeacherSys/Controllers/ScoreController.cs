using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class ScoreController : BaseTeacherController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly IStudentProcessLogService _processLog = new StudentProcessLogService();
        private readonly IStudentService _student = new StudentService();
        private readonly IAnnexService _annex = new AnnexService();
        public ActionResult Index()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var plist = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList();
            var studentlist = _tStudent.GetTreatiseTeacherPass(trea.ID, ConfigHelper.TeacherID);
            var model = new TreatiseTeacherViewModel(trea, ConfigHelper.TeacherID)
            {
                TeacherStudent = studentlist,
                ProcessList = plist,
            };
            return View(model);
        }
        public ActionResult Search()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var plist = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList();
            var studentlist = _tStudent.GetTreatiseTeacherPass(trea.ID, ConfigHelper.TeacherID);
            var model = new TreatiseTeacherViewModel(trea, ConfigHelper.TeacherID)
            {
                TeacherStudent = studentlist,
                ProcessList = plist,
            };
            return PartialView(model);
        }

        public ActionResult Detail(int id)
        {
            var model = _sProcess.GetModel(id);
            return PartialView(model);
        }
        public ActionResult Log(int id)
        {
            var log = _processLog.GetModel(id);
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID) ?? new TreatiseStudent();
            var student = _student.GetModel(ConfigHelper.StudentID) ?? new Student();
            var model = new StudentProcessViewModel()
            {
                ProcessLog = log,
                TreatiseStudent = treastu,
                TreatiseSubject = treastu.TreatiseSubject ?? new TreatiseSubject(),
                UserInfo = student.UserInfo ?? new UserInfo(),
                Annex = _annex.GetModel(log.AnnexID ?? 0),
                CheckAnnex = _annex.GetModel(log.CheckAnnexID ?? 0),
            };
            return View(model);
        }
    }
}
