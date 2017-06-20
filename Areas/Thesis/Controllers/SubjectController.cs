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
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class SubjectController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        public ActionResult Search(int? page)
        {
            string keyword = "";
            var id = ConfigHelper.TreatiseID;
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (page == null || page < 1)
            {
                page = 1;
                CookieHelper.Del("keyword");
            }
            else
                keyword = CookieHelper.GetValue("keyword");
            var sum = model.TreatiseStudents.Count;
            var allotsum = _tSubject.GetTreatisePassList(id).Sum(d => d.Total) ?? 0;
            var list = _tSubject.GetTreatiseList(ConfigHelper.TreatiseID);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = list.Where(d => d.SubjectTitle.Contains(keyword));
            var trea = new TreatiseConfigViewModel(model)
            {
                ListCount = sum,
                SumCount = allotsum,
                SubjectPage = _tSubject.GetPageList(list),
                UnCount = sum - allotsum
            };
            return model != null ? View(trea) : View("404");
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            var list = _tSubject.GetTreatiseList(ConfigHelper.TreatiseID);
            var sum = model.TreatiseStudents.Count;
            var allotsum = _tSubject.GetTreatisePassList(ConfigHelper.TreatiseID).Sum(d => d.Total) ?? 0;
            if (!string.IsNullOrWhiteSpace(keyword))
                list = list.Where(d => d.SubjectTitle.Contains(keyword));
            var trea = new TreatiseConfigViewModel(model)
            {
                ListCount = sum,
                SumCount = allotsum,
                SubjectPage = _tSubject.GetPageList(list),
                UnCount = sum - allotsum
            };
            return model != null ? View(trea) : View("404");
        }
        public ActionResult Add()
        {
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            if (trea == null) return View("404");
            var model = new TreatiseSubject() { TreatiseID = id };
            ViewBag.TreatiseTeacherID = _tTeacher.GetTreatiseList(id)
                .ToSelectList(d => d.Teacher.UserInfo.TrueName + " - " + d.Teacher.UserInfo.LoginName, d => d.ID.ToString(), "请选择");
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(TreatiseSubject model)
        {
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            if (trea == null) return View("404");
            var tteacher = _tTeacher.GetModel(model.TreatiseTeacherID ?? 0);
            var subject = _subject.GetModel(model.SubjectID ?? 0);
            if (_tSubject.IsExist(id, model.SubjectID ?? 0))
                return View("404");
            model.TreatiseID = id;
            model.CheckState = 1;
            model.SubjectTitle = subject.Title;
            model.SubjectComment = subject.Comment;
            model.SubjectNature = subject.Nature;
            model.SubjectPassStandard = subject.PassStandard;
            model.SubjectResearchAreas = subject.ResearchAreas;
            model.SubjectResearchValue = subject.ResearchValue;
            model.TeacherID = tteacher.TeacherID;
            model.CheckUserID = ConfigHelper.UserID;
            model.CheckComment = "管理员手动创建";
            subject.LastYear = model.CheckTime = DateTime.Now;
            _tSubject.SaveOrEdit(model);
            _subject.SaveAdmin(subject);
            return RedirectToAction("search");
        }
        public ActionResult Delete(int id)
        {
            var msg = "OK";
            var model = _tSubject.GetModel(id);
            if (model.Treatise.TreatiseStage != 0)
                return Json("项目已开启或已结束，无法删除此课题！", JsonRequestBehavior.AllowGet);
            try
            {
                _tSubject.Delete(model.ID);
            }
            catch (Exception ex)
            {
                msg = "删除失败！请确认项目是否已开启！,错误信息：" + ex.Message;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int id)
        {
            var model = _tSubject.GetModel(id);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');</script>");
            ViewData["CheckState"] = SelectListHelper.TreatiseSubjectCheckStateList(model.CheckState);
            return View(model);
        }
        public ActionResult Subject(int id)
        {
            var model = _subject.GetModel(id);
            return View(model);
        }
        public ActionResult Check(int id)
        {
            var model = _tSubject.GetModel(id);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');</script>");
            ViewData["CheckState"] = SelectListHelper.TreatiseSubjectCheckStateList(model.CheckState);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Check(int id, TreatiseSubject model)
        {
            model = _tSubject.GetModel(id);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');</script>");
            UpdateModel(model);
            if (model.CheckState == 1)
                model.Subject.LastYear = model.CheckTime ?? DateTime.Now;
            _tSubject.SaveOrEdit(model);

            var list = _tSubject.GetTreatiseList(model.TreatiseID ?? 0);
            var treatise = _treatise.GetModel(model.TreatiseID ?? 0);
            var trea = new TreatiseConfigViewModel(treatise)
            {
                SubjectPage = _tSubject.GetPageList(list)
            };
            return View("search", trea);
        }
    }
}
