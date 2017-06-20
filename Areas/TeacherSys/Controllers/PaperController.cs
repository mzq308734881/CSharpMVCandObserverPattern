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

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class PaperController : BaseTeacherController
    {
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        public ActionResult Index(int? id)
        {
            VerificationViewModel.DeleteCookie();
            var process = _process.GetModel(id ?? 0);
            if (process == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/teachersys/treatise'</script>");
            var teacher = _teacher.LoginTeacher();
            var model = new TreatiseTeacherViewModel(process.Treatise, teacher.ID);
            model.TeacherStudent = _tStudent.GetAllList().Where(d => d.TreatiseID == process.TreatiseID && d.TeacherID == teacher.ID);
            var list = (from c in model.TeacherStudent select c.ID).ToList();
            model.StudentProcessList = _sProcess.GetAllList().Where(d => list.Contains(d.TreatiseStudentID ?? 0)).ToList();
            return View(model);
        }
        public ActionResult Detail(int? id)
        {
            var model = _sProcess.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/teachersys/treatise'</script>");
            return View(model);
        }
        public ActionResult Check(int? id)
        {
            var model = _sProcess.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/teachersys/treatise'</script>");
            return View(model);
        }
        [HttpPost]
        public ActionResult Check(int? id, StudentProcess model)
        {
            model = _sProcess.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/teachersys/treatise'</script>");
            UpdateModel(model);
            if (model.QualityMark > model.TreatiseProcess.QualityMark)
                model.QualityMark = model.TreatiseProcess.QualityMark;
            _sProcess.SaveOrEdit(model);
            return RedirectToAction("index", new { id = model.ProcessID });
        }
    }
}
