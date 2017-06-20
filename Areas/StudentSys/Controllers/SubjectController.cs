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
    public class SubjectController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly IOpeningReportService _opening = new OpeningReportService();
        private readonly IChooseSubjectStudentService _choose = new ChooseSubjectStudentService();
        public ActionResult Index()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/'</script>");
            var treastu = _tStudent.GetTreatiseStudent(trea.ID, ConfigHelper.StudentID);
            if (treastu == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/'</script>");
            var model = new StudentChooseViewModel(trea) { State = _choose.StudentChooseState(trea, treastu) };
            return View(model);
        }
        public ActionResult Search()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/'</script>");
            var treastu = _tStudent.GetTreatiseStudent(trea.ID, ConfigHelper.StudentID);
            if (treastu == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/'</script>");
            var model = new StudentChooseViewModel(trea) { State = _choose.StudentChooseState(trea, treastu) };
            return View(model);

        }
        public ActionResult SubjectList()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/'</script>");
            var model = new StudentChooseViewModel(trea);
            var list = _opening.GetAllList().Where(d => d.StudentID == ConfigHelper.StudentID && d.TreatiseStudent.TreatiseID == trea.ID);
            model.OpeningList = model.CheckOrder == 2 ? list.OrderByDescending(d => d.Sort).ToList() : list.OrderBy(d => d.Sort).ToList();
            return View(model);
        }
        public ActionResult Subject(int? id)
        {
            var model = _tSubject.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            return View(model);
        }
        public ActionResult Opening(int? id)
        {
            var tsubject = _tSubject.GetModel(id ?? 0);
            if (tsubject == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            var model = new OpeningReport() { TreatiseSubjectID = tsubject.ID, TreatiseSubject = tsubject };
            return View(model);
        }
    }
}
