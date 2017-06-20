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
    public class PlanController : BaseStudentController
    {
        private readonly IWorkPlanService _plan = new WorkPlanService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        public ActionResult Index()
        {
            var list = _plan.GetUserPlans(ConfigHelper.InfoID);
            var page = _plan.GetPageList(list);
            return View(page);
        }
        public ActionResult Search(int? page)
        {
            var keyword = "";
            var list = _plan.GetUserPlans(ConfigHelper.InfoID);
            if (page != 0 && page != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _plan.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _plan.GetUserPlans(ConfigHelper.InfoID);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list where c.Title.Contains(keyword) || c.Comment.Contains(keyword) select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            var pager = _plan.GetPageList(list);
            return View(pager);
        }

        public ActionResult Detail(int id)
        {
            var model = _plan.GetModel(id);
            return View(model);
        }

        public ActionResult Process()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var model = new TreatiseStudentViewModel(trea)
            {
                ProcessList = trea.TreatiseProcesses.Where(d => d.State == 1).OrderBy(d => d.Sort).ToList(),
            };
            return View(model);
        }

        public ActionResult Add()
        {
            var model = new WorkPlan();
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(WorkPlan model)
        {
            model.UserType = (int)UserInfoType.Student;
            model.CreateInfoID = ConfigHelper.InfoID;
            model.IsRemind = 0;
            _plan.SaveOrEdit(model);
            return RedirectToAction("search");
        }
        public ActionResult Close(int? id)
        {
            var model = _plan.GetModel(id ?? 0);
            if (model == null) return RedirectToAction("search");
            model.IsRemind = 1;
            _plan.SaveOrEdit(model);
            return RedirectToAction("search");
        }
    }
}
