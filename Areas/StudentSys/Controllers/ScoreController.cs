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

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class ScoreController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        public ActionResult Index()
        {
            var tlist = _treatise.GetStudentTreatises(ConfigHelper.StudentID);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var stupro = _sProcess.GetTreatiseProcess(trea.ID, ConfigHelper.StudentID).ToList();
            var model = new TreatiseStudentViewModel(trea)
            {
                StudentProcessList = stupro,
                ProcessList = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList(),
            };
            return View(model);
        }
        public ActionResult Search()
        {
            var tlist = _treatise.GetStudentTreatises(ConfigHelper.StudentID);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var stupro = _sProcess.GetTreatiseProcess(trea.ID, ConfigHelper.StudentID).ToList();
            var model = new TreatiseStudentViewModel(trea)
            {
                StudentProcessList = stupro,
                ProcessList = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList(),
            };
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var pro = _process.GetModel(id);
            if (pro == null || pro.TreatiseID != ConfigHelper.TreatiseID) return View("404");
            var stupro =
                _sProcess.GetList(d => d.ProcessID == id && d.StudentID == ConfigHelper.StudentID).FirstOrDefault() ?? new StudentProcess() { TimeMark = 0, QualityMark = 0, TotalMark = 0, ScoreComment = "<p style='color:red'>暂未评分！</p>" };
            var model = new TreatiseStudentViewModel(pro.Treatise)
            {
                Process = pro,
                StudentProcess = stupro,
            };
            return View(model);
        }
    }
}
