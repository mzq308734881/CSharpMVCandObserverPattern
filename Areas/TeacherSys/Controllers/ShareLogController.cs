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
    public class ShareLogController : BaseTeacherController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        private readonly IStudentLogCommentService _stucomment = new StudentLogCommentService();
        public ActionResult Search(int? page)
        {
            string keyword = "";
            if (page != 0 && page != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _studentlog.GetShareLog();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _studentlog.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(int? page, string keyword)
        {
            var list = _studentlog.GetShareLog();
            if (page != 0 && page != null)
            {
                keyword = CookieHelper.GetValue("keyword");
            }
            else
            {
                CookieHelper.Del("keyword");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _studentlog.GetPageList(list, page ?? 1,5);
            return View(pager);
        }
        public ActionResult Detail(int id)
        {
            var model = _studentlog.GetModel(id);
            return View(model);
        }
        public ActionResult Reply(int id)
        {
            var huifu = _studentlog.GetModel(id);
            return huifu == null ? View("404") : View(huifu);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Reply(int id, StudentLogComment model)
        {
            model.ID = 0;
            model.StudentLogID = id;
            model.UserType = (int)UserInfoType.Teacher;
            model.TeacherID = ConfigHelper.TeacherID;
            _stucomment.SaveOrEdit(model);
            return RedirectToAction("Reply", new { id = id });
        }
        public ActionResult DeleteHuiFu(int id)
        {
            return Json(_stucomment.Delete(id), JsonRequestBehavior.AllowGet);
        }
    }
}
