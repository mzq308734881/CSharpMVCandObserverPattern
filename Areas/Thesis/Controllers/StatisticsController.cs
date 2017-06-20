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
using WebGrease.Css.Extensions;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class StatisticsController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        public JsonResult TeacherSubject()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var tteacher = _tTeacher.GetTreatiseList(trea.ID).ToList();
            var model = new EchartsViewModel(tteacher.Select(d => new EchartsView() { name = d.Teacher.UserInfo.TrueName, value = d.StudentSum ?? 0 }));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentSubject()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var tsubject = _tSubject.GetTreatisePassList(trea.ID).ToList();
            var model = new EchartsViewModel(tsubject.Select(d => new EchartsView() { name = d.SubjectTitle, value = d.TreatiseStudents.Count(c => c.CheckState == 1) }));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TeacherStudent()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var tteacher = _tTeacher.GetTreatiseList(trea.ID).ToList();
            var tstudent = _tStudent.GetTreatiseList(trea.ID).ToList();
            var model = new EchartsViewModel(tteacher.Select(d => new EchartsView() { name = d.Teacher.UserInfo.TrueName, value = tstudent.Count(c => c.TeacherID == d.TeacherID) }));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProcessStudent()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var process = trea.TreatiseProcesses.OrderBy(d => d.Sort).ToList();
            var list = _sProcess.GetList(d => d.TreatiseProcess.TreatiseID == trea.ID).ToList();
            var model = new EchartsViewModel(process.Select(d => new EchartsView() { name = d.ProcessName, value = list.Count(c => c.ProcessID == d.ID) }));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StudentPoint()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var tteacher = _tTeacher.GetTreatiseList(trea.ID).ToList();
            var tstudent = _tStudent.GetTreatiseList(trea.ID).ToList();
            var model = new EchartsViewModel(tteacher.Select(d => new EchartsView() { name = d.Teacher.UserInfo.TrueName, value = tstudent.Count(c => c.TeacherID == d.TeacherID) }));
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SchoolProcesss(int? schoolid)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var processs = trea.TreatiseProcesses.Where(c => c.State == 1).ToList();
            var students = _tStudent.GetTreatiseList(trea.ID);
    
                var list = from c in processs
                           select
        new SchoolProcessViewModel()
        {
            SchoolName = trea.School.InfoName,
            ID = trea.SchoolID.ToString(),
            ProcessID = c.ID,
            StudentCount = students.Count(d => d.CheckState == 1 && d.StudentProcesses.Any(e => e.ProcessID == c.ID && e.StudentProcessLogs.Any(f => f.CheckState == 1)))
        };

                return PartialView(list);
            


            return PartialView();
        }


        public class SchoolProcessViewModel
        {
            /// <summary>
            /// 学校名称
            /// </summary>
            public string SchoolName { get; set; }
            /// <summary>
            /// 学校ID
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// 流程ID
            /// </summary>
            public int ProcessID { get; set; }
            /// <summary>
            /// 学生人数
            /// </summary>
            public int StudentCount { get; set; }
        }
    }
}
