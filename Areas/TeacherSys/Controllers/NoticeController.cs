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
    public class NoticeController : BaseTeacherController
    {
        private readonly INoticeService _notice = new NoticeService();
        private readonly ITeacherService _teacher = new TeacherService();
        public ActionResult Index()
        {
            var model = new NoticeViewModel();
            var list = _notice.GetSchoolNotices(Teacher.SchoolID ?? Teacher.UserInfo.SchoolID ?? 0, 0);
            var notice = list.FirstOrDefault();
            model.Rules = new Pager<Notice>(list);
            list = _notice.GetSchoolNotices(Teacher.SchoolID ?? Teacher.UserInfo.SchoolID ?? 0, 2);
            model.Notices = new Pager<Notice>(list);
            if (notice == null)
                notice = list.FirstOrDefault();
            if (notice != null)
            {
                model.Notice = notice;
                _notice.ReadNotice(notice.ID, ConfigHelper.TeacherID, UserInfoType.Teacher);
            }
            return View(model);
        }
        public ActionResult Search(int type, int? id)
        {
            var list = _notice.GetSchoolNotices(Teacher.SchoolID ?? Teacher.UserInfo.SchoolID ?? 0, type);
            var pager = new Pager<Notice>(list, id ?? 1);
            ViewBag.Type = type;
            return View(pager);
        }
        public ActionResult Detail(int id)
        {
            var model = _notice.ReadNotice(id, ConfigHelper.TeacherID, UserInfoType.Teacher);
            return View(model);
        }
        public ActionResult DetailRules(int id)
        {
            var model = _notice.ReadNotice(id, ConfigHelper.TeacherID, UserInfoType.Teacher);
            return View(model);
        }

    }
}
