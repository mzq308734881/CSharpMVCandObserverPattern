using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;

namespace LanShanPRMS.Controllers
{
    public class AjaxHelpsController : Controller
    {
        #region  学校信息
        /// <summary>
        /// 通过上级获得下级学校信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSchoolByParent(int id)
        {
            var _user = new UserService();
            var _school = new SchoolService();
            var parent = _school.GetModel(id);
            var user = _user.GetModel(ConfigHelper.UserID);
            var schoollist = from c in _school.GetAllList() select c;
            if (user != null && parent != null)
                schoollist = from c in schoollist where c.ParentID == id select c;
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
        public static string GetSchoolNameByID(int id)
        {
            var _school = new SchoolService();
            var msg = "未找到课题";
            var model = _school.GetModel(id);
            if (model != null)
                msg = model.InfoName;
            return msg;
        }
        #endregion


        #region  用户信息相关
        public static string GetTeacherName(int? id)
        {
            var _userInfo = new UserInfoService();
            var msg = "未找到导师信息";
            var model = _userInfo.GetTeacher(id ?? 0);
            if (model != null)
                msg = model.UserInfo?.TrueName;
            return msg;
        }
        public static string GetStudentName(int? id)
        {
            var _userInfo = new UserInfoService();
            var msg = "未找到导师信息";
            var model = _userInfo.GetStudent(id ?? 0);
            if (model != null)
                msg = model.UserInfo?.TrueName;
            return msg;
        }
        public static string GetSubjectName(int? id)
        {
            var _subject = new SubjectService();
            var msg = "未找到课题信息";
            var model = _subject.GetModel(id ?? 0);
            if (model != null)
                msg = model.Title;
            return msg;
        }
        /// <summary>
        /// 依据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">默认UserID，1为TeacherID，2为StudentID</param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(int? id, int type)
        {
            var _userInfo = new UserInfoService();
            switch (type)
            {
                case 2:
                    return _userInfo.GetStudent(id ?? 0)?.UserInfo??new UserInfo();
                case 1:
                    return _userInfo.GetTeacher(id ?? 0)?.UserInfo ?? new UserInfo();
                default:
                    return _userInfo.GetAdmin(id ?? 0)?.UserInfo ?? new UserInfo();
            }
        }
        #endregion



    }
}
