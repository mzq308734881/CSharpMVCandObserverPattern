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
    public class TreatiseController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IStudentService _student = new StudentService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly IStudentProcessService _studentProcess = new StudentProcessService();
        public ActionResult Index()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return View("404");

            var model = new TreatiseStudentViewModel(trea)
            {
                ProcessList = trea.TreatiseProcesses.Where(d=>d.State==1).ToList()
            };
            var tstudent = _tStudent.GetModel(trea.ID, Student.ID);
            if (tstudent == null) return View("404");
            model.StudentProcessList = _studentProcess.GetTreatiseProcess(trea.ID, Student.ID).ToList();
            model.TreatiseStudent = tstudent;
            return View(model);
        }
        public ActionResult Subject(int? id)
        {
            var model = _tStudent.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            return View(model);
        }
    }
}
