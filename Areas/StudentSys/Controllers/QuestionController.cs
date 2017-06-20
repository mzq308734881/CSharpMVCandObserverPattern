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
    public class QuestionController : BaseStudentController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly IQuestionService _question = new QuestionService();
        private readonly IAnnexService _annex = new AnnexService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            var list = _question.GetStudentQuestions(treastu.ID);
            var pager = _question.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(int? page, int? type)
        {
            string keyword = "";
            if (page != 0 && page != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            var list = _question.GetStudentQuestions(treastu.ID);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list where c.Title.Contains(keyword) select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _question.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            var list = _question.GetStudentQuestions(treastu.ID);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            var pager = _question.GetPageList(list);
            return View(pager);
        }
        public ActionResult Add()
        {
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            ViewBag.IsOpen = SelectListHelper.GetIsOrNoSelectList(1);
            return View(treastu);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Question model, string filename, string fileurl)
        {
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            model.TreatiseStudentID = treastu.ID;
            _question.SaveOrEdit(model);
            return RedirectToAction("Search");
        }
        public ActionResult Detail(int id)
        {
            var model = _question.GetModel(id);
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var model = _studentlog.GetModel(id);
            var studnetlist = _studentlog.GetAllList();
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(1);
            ViewData["IsTeacherHelp"] = SelectListHelper.GetIsTeacherHelpList(1);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id, StudentLog model, string filename, string fileurl)
        {
            model = _studentlog.GetModel(id);
            if (model == null) return View("404");
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(fileurl))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = fileurl };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            UpdateModel(model);
            _studentlog.SaveOrEdit(model);
            return RedirectToAction("Search");
        }
        public ActionResult Close(int id)
        {
            var model = _question.GetModel(id);
            if (model == null) return Json("未找到对应提问！", JsonRequestBehavior.AllowGet);
            model.IsClose = 1;
            _question.SaveOrEdit(model);
            return RedirectToAction("Search");
        }
        public ActionResult DeleteHuiFu(int id)
        {
            return Json(_question.Delete(id), JsonRequestBehavior.AllowGet);
        }
    }
}
