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
    public class StudentLogController : BaseStudentController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        private readonly ITreatiseStudentService _treastu = new TreatiseStudentService();
        private readonly IStudentLogCommentService _stucomment = new StudentLogCommentService();
        private readonly IAnnexService _annex = new AnnexService();
        
        private TreatiseStudent TreaStu => _treastu.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var list = _studentlog.GetStudentLog(ConfigHelper.StudentID);
            ViewData["Reply"] = SelectListHelper.GetReplyList(0);
            ViewBag.InTreatise = TreaStu != null;
            var pager = _studentlog.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(int? page)
        {
            var keyword = "";
            int? reply = 0;
            if (page != 0 && page != null)
            {
                keyword = CookieHelper.GetValue("keyword");
                reply = StringHelper.StringToInt(CookieHelper.GetValue("reply"));
            }
            else
            {
                CookieHelper.Del("keyword");
                CookieHelper.Del("reply");
            }
            ViewBag.InTreatise = TreaStu != null;
            ViewBag.Reply = SelectListHelper.GetReplyList(reply);
            var list = _studentlog.GetStudentLog(ConfigHelper.StudentID);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            if (reply != 0 && reply != null)
            {
                if (reply == 1)
                {
                    list = from c in list
                           where c.StudentLogComments.Any()
                           select c;
                }
                if (reply == 2)
                {
                    list = from c in list
                           where !c.StudentLogComments.Any()
                           select c;
                }
                CookieHelper.SetObj("reply", reply.ToString());
            }
            var pager = _studentlog.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword, int? reply, int? page)
        {
            var list = _studentlog.GetStudentLog(ConfigHelper.StudentID);
            ViewBag.Reply = SelectListHelper.GetReplyList(reply);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            if (reply != 0 && reply != null)
            {
                if (reply == 1)
                {
                    list = from c in list
                           where c.StudentLogComments.Any()
                           select c;
                }
                if (reply == 2)
                {
                    list = from c in list
                           where !c.StudentLogComments.Any()
                           select c;
                }
                CookieHelper.SetObj("reply", reply.ToString());
            }
            else
                CookieHelper.Del("reply");
            var pager = _studentlog.GetPageList(list);
            return View(pager);
        }
        public ActionResult Add()
        {
            if (TreaStu == null)
            {
                ViewBag.Message = "您未在当前论文内,无法进行任何操作!";
                return View("404");
            }

            var model = new StudentLog()
            {
                TreatiseStudent = TreaStu
            };
            ViewBag.Title = TreaStu.TreatiseSubject?.SubjectTitle ?? "未找到对应课题";
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(1);
            ViewBag.IsTeacherHelp = SelectListHelper.GetIsTeacherHelpList(1);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Add(StudentLog model, string filename, string fileurl)
        {
            if (TreaStu == null)
            {
                ViewBag.Message = "您未在当前论文内,无法进行任何操作!";
                return View("404");
            }
            model.TreatiseStudentID = TreaStu.ID;
            if (!string.IsNullOrWhiteSpace(filename) && !string.IsNullOrWhiteSpace(fileurl))
            {
                var annex = new Annex() { AnnexName = filename, AnnexPath = fileurl };
                _annex.SaveOrEdit(annex);
                model.AnnexID = annex.ID;
            }
            _studentlog.SaveOrEdit(model);
            return RedirectToAction("Search");
        }
        public ActionResult Detail(int id)
        {
            var model = _studentlog.GetModel(id);
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var model = _studentlog.GetModel(id);
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(model.IsSharing);
            ViewData["IsTeacherHelp"] = SelectListHelper.GetIsTeacherHelpList(model.IsTeacherHelp);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
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
        public ActionResult Delete(int id)
        {
            return Json(_studentlog.Delete(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteHuiFu(int id)
        {
            return Json(_stucomment.Delete(id), JsonRequestBehavior.AllowGet);
        }

    }
}
