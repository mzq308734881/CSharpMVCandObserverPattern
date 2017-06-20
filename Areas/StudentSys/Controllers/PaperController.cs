using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class PaperController : BaseStudentController
    {
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly IStudentProcessLogService _processLog = new StudentProcessLogService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();

        public ActionResult Index(int? id)
        {
            var process = _process.GetModel(id ?? 0);
            var student =_admin.LoginStudent();
            if (process == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            var list = _process.GetAllList().Where(d => d.Treatise.TreatiseStudents.Any(c => c.StudentID == student.ID));
            if (!list.Any(d => d.ID == process.ID))
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            if (process.IsOpen != 1)
                return Content("<script>alert('当前流程尚未开启！');window.location.href ='/studentsys/treatise'</script>");
            var model = _sProcess.GetModel(id ?? 0, student.ID);
            var tstudent = _tStudent.GetModel(process.TreatiseID ?? 0, student.ID);
            if (model == null)
            {
                model = new StudentProcess() { StudentID = student.ID, TreatiseStudentID = tstudent.ID, ProcessID = process.ID };
                _sProcess.SaveOrEdit(model);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(StudentProcessLog model)
        {
            if (model != null)
            {
                model.IsBest = 0;
                _processLog.SaveOrEdit(model);
            }

            var sprocess = _sProcess.GetModel(model?.StudentProcessID ?? 0);
            return View("index", sprocess);
        }
        public ActionResult Edit(int? id)
        {
            var model = _processLog.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');'</script>");
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int? id, StudentProcessLog model)
        {
            model = _processLog.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');'</script>");
            UpdateModel(model);
            _processLog.SaveOrEdit(model);
            return View("index", model.StudentProcess);
        }
        public ActionResult Detail(int? id)
        {
            var model = _processLog.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');'</script>");
            return View(model);

        }
        public ActionResult SubmitIt(int? id)
        {
            var model = _sProcess.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');'</script>");
            model.SubmitTime = DateTime.Now;
            model.IsOnTime = model.TreatiseProcess.EndTime > model.SubmitTime ? 1 : 0;
            model.TimeMark = model.IsOnTime == 1 ? model.TreatiseProcess.TimeMark : 0;
            _sProcess.SaveOrEdit(model);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}
