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

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class ProgressStudentController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly TreatiseProcessEventService _event=new TreatiseProcessEventService();
        public ActionResult Search(int? page)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var plist = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList();
            var studentlist = _tStudent.GetTreatiseList(trea.ID);
            var model = new TreatiseProcessViewModel(trea)
            {
                TreatiseStudentPager = _tStudent.GetPageList(studentlist, page ?? 1),
                ProcessList = plist,
            };
            ViewBag.EventList = _event.GetAllList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Search(string stuno, string stuname)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var plist = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList();
            var list = _tStudent.GetTreatiseList(trea.ID);
            if (!string.IsNullOrWhiteSpace(stuno))
            {
                CookieHelper.SetObj("stuno", stuno);
                list = from c in list where c.Student.UserInfo.LoginName.Contains(stuno) select c;
            }
            else
                CookieHelper.Del("stuno");
            if (!string.IsNullOrWhiteSpace(stuname))
            {
                CookieHelper.SetObj("stuname", stuname);
                list = from c in list where c.Student.UserInfo.TrueName.Contains(stuname) select c;
            }
            else
                CookieHelper.Del("stuname");
            list = list.OrderByDescending(d => d.Student.UserInfo.ID);
            var model = new TreatiseProcessViewModel(trea)
            {
                TreatiseStudentPager = _tStudent.GetPageList(list),
                ProcessList = plist,
            };
            return View(model);
        }
    }
}