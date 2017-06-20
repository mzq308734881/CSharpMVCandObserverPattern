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
    public class QuestionController : BaseTeacherController
    {
        private readonly IStudentLogService _studentlog = new StudentLogService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly IQuestionService _question = new QuestionService();
        private readonly IQuestionReplyService _reply=new QuestionReplyService();
        public ActionResult Index()
        {
            var list = _question.GetTeacherQuestions(ConfigHelper.TreatiseID,ConfigHelper.TeacherID);
            var pager = _question.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(int? id, int? type)
        {
            string keyword = "";
            if (id != 0 && id != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _question.GetTeacherQuestions(ConfigHelper.TreatiseID, ConfigHelper.TeacherID);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list where c.Title.Contains(keyword) select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _question.GetPageList(list, id ?? 1);
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
        public ActionResult Reply(int id)
        {
            var question = _question.GetModel(id);
            return question == null ? View("404") : View(question);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Reply(int id, QuestionReply model)
        {
            model.ID = 0;
            model.QuestionsID = id;
            model.UserType = (int) UserInfoType.Teacher;
            model.TeacherID = ConfigHelper.TeacherID;
            _reply.SaveOrEdit(model);
            return RedirectToAction("Reply",new {id=id});
        }
        public ActionResult Detail(int id)
        {
            var model = _question.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            return Json(_studentlog.Delete(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteHuiFu(int id)
        {
            return Json(_question.Delete(id), JsonRequestBehavior.AllowGet);
        }

    }
}
