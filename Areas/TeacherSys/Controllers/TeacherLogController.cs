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
    public class TeacherLogController : BaseTeacherController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        private readonly IStudentLogCommentService _stucomment = new StudentLogCommentService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("reply");
            var list = _studentlog.GetTeacherHelpLog();
             ViewData["Reply"] = SelectListHelper.GetReplyList(0);
            var pager = _studentlog.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(int? page)
        {
           
            var keyword = "";
            int? reply = 0;
            ViewBag.Reply = SelectListHelper.GetReplyList(reply);
            var list = _studentlog.GetTeacherHelpLog();
            if (page != 0 && page != null)
            {
                keyword = CookieHelper.GetValue("keyword");
                reply =StringHelper.StringToInt(CookieHelper.GetValue("reply"));
            }
            else
            {
                CookieHelper.Del("keyword");
                CookieHelper.Del("reply");

            }
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
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _studentlog.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword, int? reply, int? page)
        {
            var list = _studentlog.GetTeacherHelpLog();
            ViewBag.Reply = SelectListHelper.GetReplyList(reply);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
            {
                CookieHelper.Del("keyword");
            }
            if (reply != null && reply != 0)
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
                CookieHelper.SetObj("Reply", reply.ToString());
            }
            else
                CookieHelper.Del("Reply");
            var pager = _studentlog.GetPageList(list);
            return View(pager);
        }
        public ActionResult Detail(int id)
        {
            var reply = _studentlog.GetModel(id);
            return reply == null ? View("404") : View(reply);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Detail(int id, StudentLogComment model)
        {
            model.ID = 0;
            model.StudentLogID = id;
            model.UserType = (int)UserInfoType.Teacher;
            model.TeacherID = ConfigHelper.TeacherID;
            _stucomment.SaveOrEdit(model);
            return RedirectToAction("Detail", new { id = id });
        }
        public ActionResult Delete(int id)
        {
            return Json(_studentlog.Delete(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Deletereply(int id)
        {
            return Json(_stucomment.Delete(id), JsonRequestBehavior.AllowGet);
        }

    }
}
