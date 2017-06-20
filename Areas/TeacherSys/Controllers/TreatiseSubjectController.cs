using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class TreatiseSubjectController : BaseTeacherController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        public ActionResult Search()
        {

            var teacher = _teacher.LoginTeacher();
            var id = ConfigHelper.TeacherID;
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID) ?? new Treatise();  
            var treaTeacher = _tTeacher.GetModel(trea.ID, teacher.ID);
            var model = new TreatiseTeacherViewModel(trea, teacher.ID);
            if (treaTeacher != null)
            {
                model.StudentSum = treaTeacher.StudentSum ?? 0;
                model.AllocatedSum = treaTeacher.TreatiseSubjects.Where(d => d.CheckState != 2).Sum(d => d.Total) ?? 0;
                model.SurplusSum = model.StudentSum - model.AllocatedSum;
            }
            model.UsedSubject = _tSubject.GetTreatiseTeacherList(trea.ID, teacher.ID).ToList();
            model.InTreatise = treaTeacher != null;
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var teacher = _teacher.LoginTeacher();
            var treatisedubject = _tSubject.GetModel(id);
            if (treatisedubject.TeacherID != teacher.ID)
                return Json("参数错误，请返回后重试！", JsonRequestBehavior.AllowGet);
            if (treatisedubject.CheckState == 1)
                return Json("此课题已审核通过，仅能由管理员操作！", JsonRequestBehavior.AllowGet);
            _tSubject.Delete(treatisedubject.ID);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add()
        {
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();'</script>");
            var teacher = _teacher.LoginTeacher();
            var model = new TreatiseTeacherViewModel(trea, teacher.ID);
            var list = _subject.GetTeacherPassSubjects(teacher.ID);
            var treaTeacher = _tTeacher.GetModel(trea.ID, teacher.ID);
            if (treaTeacher == null)
                return Content("<script>alert('您未在此项目中无法进行添加操作！');window.location.reload();'</script>");
            model.StudentSum = treaTeacher.StudentSum ?? 0;
            model.AllocatedSum = treaTeacher.TreatiseSubjects.Where(d => d.CheckState != 2).Sum(d => d.Total) ?? 0;
            model.SurplusSum = model.StudentSum - model.AllocatedSum;
            ViewBag.SubjectID = list.ToSelectList(d => d.Title, d => d.ID.ToString(), "请选择课题");
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(string[] subjectid, string[] studentsum)
        {
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            if (trea == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.reload();'</script>");
            var teacher = _teacher.LoginTeacher();
            var subject = StringHelper.StringArrToIntList(subjectid);
            subject.RemoveAt(0);
            if (subject.Any(d => d == 0))
                return Content("<script>alert('存在未选择课题，请查证后重试！');window.location.reload();</script>");

            if (subject.Count > subject.Distinct().Count())
                return Content("<script>alert('存在重复课题，请查证后重试！');window.location.reload();</script>");
            var list = _tSubject.GetTreatiseTeacherList(trea.ID, teacher.ID).Where(d => d.CheckState != 2);
            if (list.Any(d => subject.Contains(d.SubjectID ?? 0)))
                return Content("<script>alert('存在已导入课题，请查证后重试！');window.location.reload();</script>");

            var student = StringHelper.StringArrToIntList(studentsum);
            student.RemoveAt(0);

            if (student.Any(d => d == 0))
                return Content("<script>alert('课题人数必须大于0且小于总人数！');window.location.reload();</script>");
            var model = new TreatiseTeacherViewModel(trea, teacher.ID);
            var treaTeacher = _tTeacher.GetModel(trea.ID, teacher.ID);
            model.SurplusSum = treaTeacher.StudentSum ?? 0 - treaTeacher.TreatiseSubjects.Sum(d => d.Total) ?? 0;
            if (model.SurplusSum - student.Sum() < 0)
                return Content("<script>alert('当前学生数大于未分配总人数！');window.location.reload();</script>");
            var sublist = new List<TreatiseSubject>();
            for (var i = 0; i < subject.Count; i++)
            {
                var sub = _subject.GetModel(subject[i]);
                if (sub == null) continue;
                var treasub = new TreatiseSubject()
                {
                    SubjectID = subject[i],
                    SubjectTitle = sub.Title,
                    SubjectComment = sub.Comment,
                    SubjectNature = sub.Nature,
                    SubjectPassStandard = sub.PassStandard,
                    SubjectResearchAreas = sub.ResearchAreas,
                    SubjectResearchValue = sub.ResearchValue,
                    Total = student[i],
                    TeacherID = teacher.ID,
                    TreatiseID = trea.ID,
                    CheckState = 0,
                    TreatiseTeacherID = treaTeacher.ID
                };
                _tSubject.SaveOrEdit(treasub);
            }
            return RedirectToAction("search");
        }

    }
}
