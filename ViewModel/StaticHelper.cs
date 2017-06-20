using System.Web.Mvc;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.ViewModel
{
    [Authorize]
    public static class StaticHelper
    {
        #region 查询用户真实姓名
        /// <summary>
        /// 查询导师姓名
        /// </summary>
        /// <param name="teacherid"></param>
        /// <returns></returns>
        public static string TeacherName(int? teacherid)
        {
            ITeacherService teacher = new TeacherService();
            var model = teacher.GetModel(teacherid ?? 0) ?? new Teacher();
            return model?.UserInfo?.TrueName ?? "未找到对应导师";
        }
        /// <summary>
        /// 查询导师姓名
        /// </summary>
        /// <param name="teacherid"></param>
        /// <returns></returns>
        public static string TeacherNo(int? teacherid)
        {
            ITeacherService teacher = new TeacherService();
            var model = teacher.GetModel(teacherid ?? 0) ?? new Teacher();
            return model?.UserInfo?.LoginName ?? "未找到对应导师";
        }
        /// <summary>
        /// 查询学生姓名
        /// </summary>
        /// <param name="studentid"></param>
        /// <returns></returns>
        public static string StudentName(int? studentid)
        {
            IStudentService student = new StudentService();
            var model = student.GetModel(studentid ?? 0) ?? new Student();
            return model?.UserInfo?.TrueName ?? "未找到对应学生";
        }
        /// <summary>
        /// 查询学生姓名
        /// </summary>
        /// <param name="studentid"></param>
        /// <returns></returns>
        public static string StudentNo(int? studentid)
        {
            IStudentService student = new StudentService();
            var model = student.GetModel(studentid ?? 0) ?? new Student();
            return model?.UserInfo?.LoginName ?? "未找到对应学生";
        }
        /// <summary>
        /// 查询管理员姓名
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string AdminName(int? userid)
        {
            IUserService user = new UserService();
            var model = user.GetDtoModel(userid ?? 0);
            return model?.UserInfo?.TrueName ?? "未找到对应管理员";
        }
        /// <summary>
        /// 查询用户姓名
        /// </summary>
        /// <param name="infoid"></param>
        /// <returns></returns>
        public static string InfoName(int? infoid)
        {
            IAdminService admin = new AdminService();
            var model = admin.LoginInfo(infoid ?? 0);
            return model?.TrueName ?? "未找到对应管理员";
        }
        /// <summary>
        /// 查询用户姓名
        /// </summary>
        /// <param name="infoid"></param>
        /// <returns></returns>
        public static string InfoNo(int? infoid)
        {
            IAdminService admin = new AdminService();
            var model = admin.LoginInfo(infoid ?? 0);
            return model?.LoginName ?? "未找到对应管理员";
        }
        #endregion
        #region 查询附件信息

        public static Annex GetAnnex(int annexid)
        {
            var model = new AnnexService().GetModel(annexid);
            return model;
        }
        #endregion

        public static string GetSchoolName(int? schoolid)
        {
            ISchoolService school = new SchoolService();
            return school.GetModel(schoolid ?? 0)?.InfoName ?? "未找到对应学校信息";
        }
        public static string GetSchoolFullName(int? schoolid)
        {
            ISchoolService school = new SchoolService();
            return school.GetSchoolFullName(schoolid ?? 0);
        }
    }
}
