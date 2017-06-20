using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.IService;
using LanShanPRMS.Core;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class AjaxController : BaseAdminController
    {
        //[Dependency]
        //public IUserService _user { get; set; }
        //public IAdminService _admin { get; set; }
        private readonly IStudentService _student = new StudentService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly ISubjectService _subject = new SubjectService();
        private readonly ITreatiseSubjectService _tSubject = new TreatiseSubjectService();
        private readonly IUserInfoService _info = new UserInfoService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        //
        // GET: /Ajax/
        //[UserAuthorize(ModuleAlias = "ajax", OperaAction = "jquerytree", Comment = "栏目菜单")]

        /// <summary>
        /// 验证是否存在当前用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult ExistLoginName(int id, string name)
        {
            return Json(_info.IsExist(id, name), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取剩余质量分
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="id"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public ActionResult GetQualityGrades(int tid, int? id, float no)
        {
            var root = _treatise.QualityGradesExist(tid, id, no);
            return Json(root, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 获取剩余时间分
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="id"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        public ActionResult GetTimeGrades(int tid, int? id, float no)
        {
            var root = _treatise.TimeGradesExist(tid, id, no);
            return Json(root, JsonRequestBehavior.AllowGet);
        }

        #region  学校信息
        /// <summary>
        /// 通过上级获得下级学校信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSchoolByParent(int id)
        {
            var parent = _school.GetModel(id);
            var schoollist = from c in _school.GetAllList() select c;
            if (Admin != null && parent != null)
            {
                var schoolist = _school.GetUserSchool(Admin.SchoolID ?? 0);
                if (schoolist.Any(d => d == id))
                    schoollist = from c in schoollist where c.ParentID == id select c;
                else
                    schoollist = from c in schoollist where c.ParentID == id select c;
            }
            else
                schoollist = schoollist.Take(0);


            var list = from c in schoollist
                       select new
                       {
                           ItemID = c.ID,
                           ItemName = c.InfoName
                       };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult GetSchoolTeacher(int id)
        {
            var list = _teacher.GetSchoolTeacher(id).Select(d => new { ItemID = d.ID, ItemName = d.UserInfo.TrueName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTeacherSubject(int id)
        {
            var list = _subject.GetTeacherPassSubjects(id).Select(d => new { ItemID = d.ID, ItemName = d.Title }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 排除项目中已包含的课题
        /// </summary>
        /// <param name="id">TreatiseTeacherID</param>
        /// <returns></returns>
        public JsonResult GetTreatiseTeacherSubject(int id)
        {
            //查询项目里面的课题
            var treasubs = _tSubject.GetTreatiseList(ConfigHelper.TreatiseID).Where(d => d.CheckState == 0 || d.CheckState == 1).Select(d => d.SubjectID).ToList();
            var teacher = _tTeacher.GetModel(id);
            //根据课题ID查询id和课题名称在老师课题下面集合
            var list = _subject.GetTeacherPassSubjects(teacher.TeacherID).Where(d => !treasubs.Contains(d.ID)).Select(d => new { ItemID = d.ID, ItemName = d.Title }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 排除项目中已包含的课题
        /// </summary>
        /// <param name="id">TeacherID</param>
        /// <returns></returns>
        public JsonResult GetTeacherTreatiseSubject(int id)
        {
            var teacher = _teacher.GetModel(id);
            //查询项目里面的课题
            var list = _tSubject.GetTreatiseSubjectsNotMax(ConfigHelper.TreatiseID).Where(d => d.CheckState == 1 && d.TeacherID == teacher.ID).Select(d => new { ItemID = d.ID, ItemName = d.SubjectTitle }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 排除项目中已包含的学生
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTreatiseStudent()
        {
            var treatisestu = _tStudent.GetTreatiseList(ConfigHelper.TreatiseID).Select(d => d.StudentID).ToList();
            var list = _student.GetTreatiseStudent(ConfigHelper.TreatiseID).Where(d => !treatisestu.Contains(d.ID)).Select(d => new { ItemID = d.ID, ItemName = d.UserInfo.TrueName }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询项目导师的未分配学生
        /// </summary>
        /// <returns></returns>
        public JsonResult TeacherStudentCount(int id, int param)
        {
            var treastu = _tStudent.GetTreatiseList(ConfigHelper.TreatiseID).Count();
            var treatea = _tTeacher.GetTreatiseList(ConfigHelper.TreatiseID);
            if (id != 0)
                treatea = treatea.Where(d => d.ID != id);
            var stucount = treastu - (treatea.Sum(d => d.StudentSum) ?? 0);
            var count = stucount - param;
            return Json(new ValidformViewModel() { status = count >= 0 ? "y" : "n", info = "剩余人数：" + (count >= 0 ? count : stucount) });
        }
        /// <summary>
        /// 查询项目课题的未分配学生人数
        /// </summary>
        /// <returns></returns>
        public JsonResult SubjectStudentCount(int id, int param)
        {
            //学生总数
            var treastu = _tStudent.GetTreatiseList(ConfigHelper.TreatiseID).Count();
            var teastu = _tSubject.GetTreatisePassList(ConfigHelper.TreatiseID);
            if (id != 0)
                teastu = teastu.Where(d => d.ID != id);
            var stucount = treastu - (teastu.Sum(d => d.Total) ?? 0);
            var count = stucount - param;
            return Json(new ValidformViewModel() { status = count >= 0 ? "y" : "n", info = "剩余人数：" + (count >= 0 ? count : stucount) });
        }
        /// <summary>
        /// 查询项目可分配时间分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult TimeCount(int id, float param)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var times = trea.TreatiseProcesses.Where(d => d.State == 1).Sum(d => d.TimeMark);
            if (id != 0)
                times = trea.TreatiseProcesses.Where(d => d.State == 1 && d.ID != id).Sum(d => d.TimeMark);
            var treatime = trea.TimeGrades - times;
            var count = (float)treatime - param;
            return Json(new ValidformViewModel() { status = count >= 0 ? "y" : "n", info = "剩余可分配时间分：" + (count >= 0 ? count : (float)treatime) });
        }
        /// <summary>
        /// 查询项目可分配质量分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult QualityCount(int id, float param)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var times = trea.TreatiseProcesses.Where(d => d.State == 1).Sum(d => d.QualityMark);
            if (id != 0)
                times = trea.TreatiseProcesses.Where(d => d.State == 1 && d.ID != id).Sum(d => d.QualityMark);
            var treatime = (trea.QualityGrades - times) ?? 0;
            var count = (float)treatime - param;
            return Json(new ValidformViewModel() { status = count >= 0 ? "y" : "n", info = "剩余可分配质量分：" + (count >= 0 ? count : (float)treatime) });
        }

        public JsonResult TreatiseTeacherSubjectStudentTotal(int tid, int sid, int? stu)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            var tteacher = _tTeacher.GetModel(tid);
            var sum = tteacher.StudentSum - tteacher.TreatiseSubjects.Sum(d => d.Total) - stu;
            return Json(new ValidformViewModel() { status = sum >= 0 ? "y" : "n", info = "剩余可分配人数为：" + (sum >= 0 ? sum : sum + stu) + "！" });

        }
    }
}
