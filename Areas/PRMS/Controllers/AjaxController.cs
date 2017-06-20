using System.Collections.Generic;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.IService;
using LanShanPRMS.Core;
using LanShanPRMS.Model.Models;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.PRMS.Controllers
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
        ///// <summary>
        ///// 排除项目中已包含的课题
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public JsonResult GetTreatiseTeacherSubject(int id)
        //{
        //    //查询cookie里面的项目下面的课题
        //    var treasubs = _tSubject.GetTreatiseList(ConfigHelper.TreatiseID).Select(d => d.SubjectID).ToList();
        //    //根据课题ID查询id和课题名称在老师课题下面集合
        //    var list = _subject.GetTeacherPassSubjects(id).Where(d => !treasubs.Contains(d.ID)).Select(d => new { ItemID = d.ID, ItemName = d.Title }).ToList();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
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
        /// 查询项目未分配学生
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTreatiseStudentCount(int id, int no)
        {
            var treastu = _tStudent.GetTreatiseList(ConfigHelper.TreatiseID).Count();
            var teastu = _tTeacher.GetTreatiseList(ConfigHelper.TreatiseID);
            if (id != 0)
                teastu = teastu.Where(d => d.ID != id);
            var stucount = teastu.Sum(d => d.StudentSum) ?? 0;
            var count = treastu - stucount;
            return Json(new RootViewModel() { State = count - no >= 0 ? 1 : 0, Message = "剩余人数：" + count });
        }

        public JsonResult ZTree(int? id)
        {
            List<School> categorylist;
            if (_admin.IsSuperAdmin(Admin.InfoID))
                categorylist = _school.GetAllList().Where(d => d.State == 1).ToList();
            else
            {
                var ids = _school.GetUserSchool(Admin.SchoolID ?? 0);
                categorylist = _school.GetAllList().Where(d => ids.Contains(d.ID)).ToList();
            }
            var list = from c in categorylist orderby c.ID select new ZNode{ id = c.ID, name = c.InfoName, pId = c.ParentID ?? 0, isParent = (c.School1.Any(d => d.State == 1)), open =true };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JQueryTree(int? id, int? lv)
        {
            var user = _admin.LoginUser();
            List<School> categorylist=new List<School>();
            if (_admin.IsSuperAdmin(user.InfoID))
                categorylist = _school.GetAllList().Where(d => d.ParentID == id && d.State == 1).ToList();
            else
            {
                if (id != null)
                {
                    var ids = _school.GetUserChildSchool(id ?? 0);
                    categorylist = _school.GetAllList().Where(d => ids.Contains(d.ID)).ToList();
                }
            }
            var list = from c in categorylist orderby c.ID select new { id = c.ID, name = c.InfoName, pid = id, isParent = (c.School1.Any(d => d.State == 1)), type = c.InfoType };
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}