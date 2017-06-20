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
    public class TreatiseController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ISchoolService _school = new SchoolService();
        public ActionResult Index()
        {
            VerificationViewModel.DeleteCookie();
            var pager = _treatise.GetPageList();
            return View("search", pager);
        }
        public ActionResult Search(string keyword, int? id)
        {
            var list = _treatise.GetList(d => d.State == 1);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                CookieHelper.SetObj("keyword", keyword);
                list = from c in list.Where(d => d.TreatiseName.Contains(keyword)) select c;
            }
            list = list.OrderByDescending(d => d.ID);
            return View("search", _treatise.GetPageList(list, id ?? 1));
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _treatise.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.TreatiseName.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _treatise.GetPageList(list);
            return View(pager);
        }

        public ActionResult EditSort(int id, int tid, int pid)
        {
            var treatise = _treatise.GetModel(tid);
            var paper = treatise.TreatiseProcesses.Where(d => d.Sort == id && d.ID != pid);
            return Json(paper != null ? "OK" : "", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            var model = new Treatise();
            var user = _admin.LoginUser();
            ViewBag.IsReport = SelectListHelper.TreatiseIsReportList(0);
            ViewBag.ShowTeacher = SelectListHelper.TreatiseShowTeacherList(1);
            ViewBag.StageMode = SelectListHelper.TreatiseStageModeList(0);
            ViewBag.ChooseTeacher = SelectListHelper.TreatiseChooseTeacherList(0);
            ViewBag.SchoolID = _school.GetAllList().ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业");
            ViewBag.CheckOrder = SelectListHelper.TreatiseCheckOrderList(model.CheckOrder);
            ViewBag.AdminChoose = SelectListHelper.GetIsAdminList(model.AdminChoose);
            var year = DateTime.Now.Year;
            var list = new List<int>() { year - 1, year, year + 1 };
            ViewBag.ApplicableYear = list.ToSelectList(d => d.ToString(), d => d.ToString(), "请选择");
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Treatise model, FormCollection fc)
        {
            var user = _admin.LoginUser();
            if (model.SchoolID == 0)
                ViewData.ModelState.AddModelError("SchoolID", "请选择项目专业！");
            if (model.TimeGrades + model.QualityGrades > 100)
                ViewData.ModelState.AddModelError("TimeGrades", "时间分与质量分的总和不能大于100！");
            if (model.PassMark > model.TimeGrades + model.QualityGrades)
                ViewData.ModelState.AddModelError("PassMark", "及格分不能大于总分！");
            if (!ViewData.ModelState.IsValid)
            {
                ViewBag.IsReport = SelectListHelper.TreatiseIsReportList(0);
                ViewBag.ShowTeacher = SelectListHelper.TreatiseShowTeacherList(1);
                ViewBag.StageMode = SelectListHelper.TreatiseStageModeList(0);
                ViewBag.ChooseTeacher = SelectListHelper.TreatiseChooseTeacherList(0);
                ViewBag.SchoolID = _school.GetAllList().ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业");
                ViewBag.CheckOrder = SelectListHelper.TreatiseCheckOrderList(model.CheckOrder);
                var year = DateTime.Now.Year;
                var list = new List<int>() { year - 1, year, year + 1 };
                ViewBag.ApplicableYear = list.ToSelectList(d => d.ToString(), d => d.ToString(), "请选择", StringHelper.StringToZero(model.ApplicableYear));
                return View(model);
            }
            model.CreateUserID = user.ID;
            _treatise.SaveOrEdit(model);
            ConfigHelper.SetTreatiseID(model.ID);
            return Redirect(@Url.Action("Index", "Home"));
        }

        public ActionResult Edit(int id)
        {
            var user = _admin.LoginUser();
            var model = _treatise.GetModel(id);
            if (model == null) return RedirectToAction("add");
            if (model.TreatiseStage > 0)
                return View("detail", model);
            ViewBag.IsReport = SelectListHelper.TreatiseIsReportList(model.IsReport);
            ViewBag.ShowTeacher = SelectListHelper.TreatiseShowTeacherList(model.ShowTeacher);
            ViewBag.StageMode = SelectListHelper.TreatiseStageModeList(model.StageMode);
            ViewBag.ChooseTeacher = SelectListHelper.TreatiseChooseTeacherList(model.ChooseTeacher);
            ViewBag.SchoolID = _school.GetAllList().ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            var year = DateTime.Now.Year;
            var list = new List<int>() { year - 1, year, year + 1 };
            ViewBag.ApplicableYear = list.ToSelectList(d => d.ToString(), d => d.ToString(), "请选择");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Treatise model, FormCollection fc)
        {
            var user = _admin.LoginUser();
            model = _treatise.GetModel(id);
            UpdateModel(model);
            if (model.SchoolID == 0)
                ViewData.ModelState.AddModelError("SchoolID", "请选择项目专业！");
            if (model.TimeGrades + model.QualityGrades > 100)
                ViewData.ModelState.AddModelError("TimeGrades", "时间分与质量分的总和不能大于100！");
            if (model.PassMark > model.TimeGrades + model.QualityGrades)
                ViewData.ModelState.AddModelError("PassMark", "及格分不能大于总分！");
            if (!ViewData.ModelState.IsValid)
            {
                ViewBag.IsReport = SelectListHelper.TreatiseIsReportList(model.IsReport);
                ViewBag.ShowTeacher = SelectListHelper.TreatiseShowTeacherList(model.ShowTeacher);
                ViewBag.StageMode = SelectListHelper.TreatiseStageModeList(model.StageMode);
                ViewBag.ChooseTeacher = SelectListHelper.TreatiseChooseTeacherList(model.ChooseTeacher);
                ViewBag.SchoolID = _school.GetAllList().ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
                return View(model);
            }
            _treatise.SaveOrEdit(model);
            return RedirectToAction("index");
        }

        public ActionResult Detail(int id)
        {
            var model = _treatise.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _treatise.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Set()
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            var schoollist = _school.GetList(d => d.InfoType == 4);
            ViewBag.ShowTeacher = SelectListHelper.TreatiseShowTeacherList(model.ShowTeacher);
            ViewBag.IsReport = SelectListHelper.TreatiseIsReportList(model.IsReport);
            ViewBag.StageMode = SelectListHelper.TreatiseStageModeList(model.StageMode);
            ViewBag.ChooseTeacher = SelectListHelper.TreatiseChooseTeacherList(model.ChooseTeacher);
            ViewBag.SchoolID = schoollist.ToSelectList(d => d.InfoName, d => d.ID.ToString(), "请选择专业", model.SchoolID ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Set(Treatise model, FormCollection fc)
        {
            model = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (model == null)
                return Content("<script>alert('未找到对应项目！！');</script>");
            if (model.TreatiseStage != 0)
                return Content("<script>alert('当前项目已开启或已结束，不能再进行配置修改！！');</script>");
            UpdateModel(model);
            var start = fc["Start"];
            if (!string.IsNullOrWhiteSpace(start))
                model.TreatiseStage = 1;
            _treatise.SaveOrEdit(model);
            return View("End", model);
        }

    }
}
