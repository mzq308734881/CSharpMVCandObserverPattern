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
    public class ReviewController : BaseTeacherController
    {
        private readonly IReviewTeacherService _review = new ReviewTeacherService();
        private readonly ITreatiseStudentService _student = new TreatiseStudentService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly IStudentProcessLogService _log = new StudentProcessLogService();
        private readonly ITreatiseProcessEventService _event =new TreatiseProcessEventService();

        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var list = _review.GetTeacherStudentProcessLogs(Teacher.ID);
            ViewBag.EventList= _event.GetList(d => d.IdentityID == 3);
            return View(list);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var list = _review.GetTeacherStudentProcessLogs(Teacher.ID);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.StudentProcess.Student.UserInfo.LoginName.Contains(keyword) || c.StudentProcess.Student.UserInfo.TrueName.Contains(keyword)
                       select c;
            ViewBag.EventList = _event.GetList(d => d.IdentityID == 3);
            return View(list);
        }

        public ActionResult Action(int? id)
        {
            var log = _log.GetModel(id ?? 0);
            if (log == null) return PartialView("404");
            return PartialView(log);
        }
    }
}
