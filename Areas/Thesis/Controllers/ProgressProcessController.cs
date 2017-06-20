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
    public class ProgressProcessController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly IStudentProcessService _sprocess = new StudentProcessService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseProcessEventService _event = new TreatiseProcessEventService();
        public ActionResult Search(int? id, int? page)
        {
            var process = _process.GetModel(id ?? 0);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var plist = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList();
            if (process == null)
                process = plist.FirstOrDefault();
            if (process == null) process = new TreatiseProcess();
            var list = _tStudent.GetTreatiseList(process.TreatiseID ?? 0).Where(d => d.Student.State != 0).OrderByDescending(d => d.Student.UserInfo.ID);
            var model = new TreatiseProcessViewModel(trea)
            {
                Process = process,
                ProcessList = plist,
                TreatiseStudentPager = _tStudent.GetPageList(list),
                StudentProcessPager = _sprocess.GetPageList(process.ID, page ?? 1),
                EventList = _event.GetIdentityActionList(process.TreatiseProcessEvents.Select(d => d.EventID).ToList()).Where(d => d.Checked)
            };
            return View(model);
        }
        public ActionResult StudentList(int? id, int? page)
        {
            var process = _process.GetModel(id ?? 0);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null || process == null || trea.ID != process.TreatiseID) return View("404");

            var stuno = "";
            var stuname = "";
            if (page == null || page < 1)
            {
                page = 1;
                CookieHelper.Del("stuno");
                CookieHelper.Del("stuname");
            }
            else
            {
                stuno = StringHelper.ClearFormat(CookieHelper.GetValue("stuno"));
                stuname = StringHelper.ClearFormat(CookieHelper.GetValue("stuname"));
            }
            var list = _tStudent.GetTreatiseList(process.TreatiseID ?? 0);
            if (!string.IsNullOrWhiteSpace(stuno))
                list = from c in list where c.Student.UserInfo.LoginName.Contains(stuno) select c;
            if (!string.IsNullOrWhiteSpace(stuname))
                list = from c in list where c.Student.UserInfo.TrueName.Contains(stuname) select c;
            list = list.Where(d => d.Student.State != 0).OrderByDescending(d => d.Student.UserInfo.ID);
            var model = new TreatiseProcessViewModel(trea)
            {
                Process = process,
                TreatiseStudentPager = _tStudent.GetPageList(list, page ?? 1),
                EventList = _event.GetIdentityActionList(process.TreatiseProcessEvents.Select(d => d.EventID).ToList()).Where(d => d.Checked)
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult StudentList(int? id, string stuno, string stuname)
        {
            var process = _process.GetModel(id ?? 0);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null || process == null || trea.ID != process.TreatiseID) return View("404");
            var list = _tStudent.GetTreatiseList(process.TreatiseID ?? 0);
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
                Process = process,
                TreatiseStudentPager = _tStudent.GetPageList(list),
            };
            return View(model);
        }
    }
}