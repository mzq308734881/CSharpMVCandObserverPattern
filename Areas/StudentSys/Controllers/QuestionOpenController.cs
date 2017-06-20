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
    public class QuestionOpenController : BaseStudentController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        private readonly IStudentLogCommentService _stucomment = new StudentLogCommentService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        public ActionResult Search(int? id)
        {
            string keyword = "";
            if (id != 0 && id != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _studentlog.GetShareLog();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.Title.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _studentlog.GetPageList(list, id ?? 1);
            return View(pager);
        }
        public ActionResult Detail(int id)
        {
            var model = _studentlog.GetModel(id);
            var sharemodel = _stucomment.GetModel(id);
            var studnetlist = _studentlog.GetAllList();
            var studentid = ConfigHelper.StudentID;
            var sid = 0;
            foreach (var item in model.StudentLogComments)
            {
                if (item.StudentID != 0 && item.StudentID == studentid)
                    sid = int.Parse(item.StudentID.ToString());
            }
            ViewBag.id = sid;
            ViewBag.iid = studentid;
            var subjectstu = _tStudent.GetModel(ConfigHelper.StudentID);
            if (!studnetlist.Any())
                return View("404");//该学生没有课题
            ViewData["Subject"] = subjectstu.TreatiseSubject.SubjectTitle;
            if (sharemodel.StudentID == studentid)
                ViewData["isbenren"] = "yes";
            return View(model);
        }
        [HttpPost]
        public ActionResult Detail(int? id, StudentLogComment model, FormCollection fc)
        {
            var studentid = ConfigHelper.StudentID;
            var studentlog = _studentlog.GetModel(id ?? 0);
            model.StudentLogID = id;
            model.UserType = 1;
            model.StudentID = studentid;
            _stucomment.Save(model);
            return RedirectToAction("Detail", new { id = model.StudentLogID });
        }
        public ActionResult DeleteHuiFu(int id)
        {
            return Json(_stucomment.Delete(id), JsonRequestBehavior.AllowGet);
        }
    }
}
