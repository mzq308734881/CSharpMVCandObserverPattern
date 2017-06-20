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

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class ReviewController : BaseAdminController
    {
        private readonly IReviewTeacherService _review = new ReviewTeacherService();

        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var list = _review.GetAdminrStudentProcessLogs();
            return View(list);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var list = _review.GetAdminrStudentProcessLogs();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.StudentProcess.Student.UserInfo.LoginName.Contains(keyword) || c.StudentProcess.Student.UserInfo.TrueName.Contains(keyword)
                       select c;
            return View("index",list);
        }

    }
}
