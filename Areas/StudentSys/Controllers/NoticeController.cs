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
    public class NoticeController : BaseStudentController
    {
        private readonly INoticeService _notice = new NoticeService();
        private readonly IStudentService _student = new StudentService();
        public ActionResult Index()
        {
            var model = new NoticeViewModel();
            var list = _notice.GetSchoolNotices(Student.SchoolID?? Student.UserInfo.SchoolID ?? 0,0);
            var notice = list.FirstOrDefault();
            model.Rules = new Pager<Notice>(list);
            list = _notice.GetSchoolNotices(Student.SchoolID ?? Student.UserInfo.SchoolID ?? 0, 2);
            model.Notices = new Pager<Notice>(list);
            if (notice == null)
                notice = list.FirstOrDefault();
            if (notice != null)
            {
                model.Notice = notice;
                _notice.ReadNotice(notice.ID, ConfigHelper.StudentID, UserInfoType.Student);
            }
            return View(model);
        }
        public ActionResult Search(int type, int? id)
        {
            var list = _notice.GetSchoolNotices(Student.SchoolID ?? Student.UserInfo.SchoolID ?? 0, type);
            var pager = new Pager<Notice>(list, id ?? 1);
            ViewBag.Type = type;
            return View(pager);
        }

        public ActionResult Detail(int id)
        {
            var model = _notice.ReadNotice(id, ConfigHelper.StudentID, UserInfoType.Student);
            return View(model);
        }
    }
}
