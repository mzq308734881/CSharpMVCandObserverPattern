using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    public class QuestionnaireController : Controller
    {
        private readonly IQuestionnaireService _questionnaire=new QuestionnaireService();
        public ActionResult Index(int?page)
        {
            var list = _questionnaire.GetPageList(page??1);
            return View(list);
        }
        public ActionResult Search(int? page)
        {
            var keyword = "";
            if (page != 0 && page != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _questionnaire.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _questionnaire.GetPageList(list, page ?? 1);
            return View("index", pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _questionnaire.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _questionnaire.GetPageList(list);
            return View("index", pager);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add(Questionnaire model)
        {
            _questionnaire.SaveOrEdit(model);
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int?id)
        {
            var model = _questionnaire.GetModel(id??0);
            return model!=null? View(model): View("404");
        }
        [HttpPost]
        public ActionResult Edit(int?id,Questionnaire model)
        {
             model = _questionnaire.GetModel(id ?? 0);
            if (model == null) return View("404");
            UpdateModel(model);
            _questionnaire.SaveOrEdit(model);
            return RedirectToAction("Index");
        }
        public ActionResult Detial(int? id)
        {
            var model = _questionnaire.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }
    }
}
