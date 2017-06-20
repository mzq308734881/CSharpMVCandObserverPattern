using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class StudentController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly IStudentService _student = new StudentService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly IUserInfoService _userInfo = new UserInfoService();
        public ActionResult Index()
        {
            CookieHelper.Del("stuno");
            CookieHelper.Del("stuname");
            var model = new SchoolDTO() { StudentPage = _student.GetPageList() };
            return View(model);
        }
        public ActionResult Search(int? id, int? page)
        {
            var model = new SchoolDTO() { SchoolID = id };
            var stuno = "";
            var stuname = "";
            var list = _student.GetAllList();
            if (id != 0 && id != null)
            {
                CookieHelper.Del("stuno");
                CookieHelper.Del("stuname");
                var ids = _school.GetUserChildSchool(id ?? 0);
                list = from c in list where ids.Contains(c.SchoolID ?? 0) select c;
            }
            else
            {
                stuno = CookieHelper.GetValue("stuno");
                stuname = CookieHelper.GetValue("stuname");
                if (!string.IsNullOrWhiteSpace(stuno))
                {
                    list = from c in list where c.UserInfo.LoginName.Contains(stuno) select c;
                    CookieHelper.SetObj("stuno", stuno);
                }
                if (!string.IsNullOrWhiteSpace(stuname))
                {
                    list = from c in list where c.UserInfo.TrueName.Contains(stuname) select c;
                    CookieHelper.SetObj("stuname", stuname);
                }
            }
            model.StudentPage = _student.GetPageList(list, page ?? 1);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Search(string stuno, string stuname)
        {
            var list = _student.GetAllList();
            if (!string.IsNullOrWhiteSpace(stuno))
            {
                CookieHelper.SetObj("stuno", stuno);
                list = from c in list where c.UserInfo.LoginName.Contains(stuno) select c;
            }
            else
            {
                CookieHelper.Del("stuno");
            }
            if (!string.IsNullOrWhiteSpace(stuname))
            {
                CookieHelper.SetObj("stuname", stuname);
                list = from c in list where c.UserInfo.TrueName.Contains(stuname) select c;
            }
            else
            {
                CookieHelper.Del("stuname");
            }
            var model = new SchoolDTO() { StudentPage = _student.GetPageList(list) };
            return PartialView(model);
        }
        public ActionResult Add(int? id)
        {
            var school = _school.GetModel(id ?? 0);
            var model = new Student() { SchoolID = id };
                model.School = school;
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(1);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Student model, FormCollection fc,int ?id)
        {
            model.ID = 0;
            var school = _school.GetModel(model.SchoolID??0);
            if (school == null)
            {
                ModelState.AddModelError("SchoolID", "请选择学校信息!");
                ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model.UserInfo.Sex ?? 1);
                return View(model);
            }
            model.UserInfo.SchoolID = model.SchoolID;
            model.UserInfo.Password = StringHelper.Md5Encrypt(model.UserInfo.LoginName);
            _student.SaveOrEdit(model);
            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var model = _student.GetModel(id);
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model.UserInfo.Sex ?? 1);
            return View(model);
        }
        [HttpPost, ValidateInput(false), HandleError(View = "error")]
        public ActionResult Edit(int id, Student model, FormCollection fc)
        {
            model = _student.GetModel(id);
            UpdateModel(model);
            model.UserInfo.SchoolID = model.SchoolID;
            _student.SaveOrEdit(model);
            return RedirectToAction("index");
        }

        public ActionResult Detail(int id)
        {
            var model = _student.GetModel(id);
            return model != null ? View(model) : View("404");
        }
        public ActionResult Delete(int id)
        {
            var msg = _student.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetPwd(int? id)
        {
            return Json(_userInfo.ResetPwd(id ?? 0), JsonRequestBehavior.AllowGet);
        }

        #region 导入Excel批量添加学生数据
        /// <summary>
        /// 导入页
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            CookieHelper.Del("FileUrl");
            return View();
        }
        //导入结果预览
        [HttpPost]
        public ActionResult Import(string fileurl)
        {
            var conn = ConfigHelper.ServerPath + fileurl;
            var list = GetList(conn);
            CookieHelper.SetObj("FileUrl", fileurl);
            return View(list);
        }
        //导入数据库
        public ActionResult ImportDB()
        {
            var excelUrl = CookieHelper.GetValue("FileUrl");
            if (excelUrl == null)
            {
                return Content("请先上传文件");
            }
            var conn = ConfigHelper.ServerPath + excelUrl;
            var user = _admin.LoginUser();
            if (user == null) return Content("用户不存在！");
            var list = GetList(conn);
            list = list.Where(d => d.State == 1).ToList();
            foreach (var item in list)
            {
                if (item.State != 1) continue;
                var model = item.Student;
                model.UserInfo.CreateInfoID = user.InfoID;
                model.UserInfo.SchoolID = model.SchoolID;
                model.UserInfo.Password = StringHelper.Md5Encrypt(model.UserInfo.LoginName);
                _student.SaveOrEdit(model);
            }
            CookieHelper.Del("FileUrl");
            return RedirectToAction("index");
        }

        ///获取excel里面的数据并转化成对应的List       
        public List<ImportViewModel> GetList(string conn)
        {
            var list = new List<ImportViewModel>();
            var ds = VerificationViewModel.ExcelToDataSet(conn);
            var loginname = _userInfo.GetLoginNameList();
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var dr = ds.Tables[0].Rows[i];
                var model = GetModel(dr);
                if (model == null) continue;
                model.ID = i + 1;

                if (model.State == 1)
                {
                    if (loginname.Contains(model.Student.UserInfo.LoginName))
                    {
                        model.Message = " 系统中已存在此登陆账号，不会被导入";
                        model.State = 2;
                    }
                    var count = from c in list
                                where c.Student.UserInfo.LoginName == model.Student.UserInfo.LoginName
                                select c;
                    if (count.Count() > 1)
                    {
                        model.Message = "在此批数据中登陆名重复";
                        model.State = 0;
                    }
                }
                list.Add(model);
            }
            return list;
        }
        ///获取excel一行的记录转化成实体
        public ImportViewModel GetModel(DataRow dr)
        {
            var user = _admin.LoginUser();
            if (dr == null)
                return null;
            var major = dr[0].ToString();
            var grade = dr[1].ToString();
            var classname = dr[2].ToString();
            var loginname = dr[3].ToString();
            var truename = dr[4].ToString();
            var sex = dr[5].ToString();
            var tel = dr[6].ToString();
            var mail = dr[7].ToString();
            var qq = dr[8].ToString();
            var whatch = dr[9].ToString();

            #region 登录帐号
            if (string.IsNullOrEmpty(loginname))
                return new ImportViewModel()
                {
                    Message = "当前行登陆账号为空，不会被导入",
                    State = 3
                };
            loginname = loginname.Trim();
            #endregion

            #region 老师姓名

            if (string.IsNullOrEmpty(truename))
                return new ImportViewModel()
                {
                    Message = "老师姓名不能为空",
                };
            truename = truename.Trim();

            #endregion

            var model = new ImportViewModel();
            model.Student.UserInfo.LoginName = loginname;
            model.Student.UserInfo.TrueName = truename;

            //性别
            if (string.IsNullOrEmpty(sex))
                model.Student.UserInfo.Sex = 1;
            model.Student.UserInfo.Sex = sex.Contains("女") ? 2 : 1;

            // 联系电话
            if (!string.IsNullOrEmpty(tel))
            {
                if (VerificationViewModel.IsMobilePhone(tel))
                    model.Student.UserInfo.Tel = tel.Trim();
                else
                {
                    model.Message = "手机号不正确";
                    model.State = 0;
                    return model;
                }
            }
            // 电子邮箱
            if (!string.IsNullOrEmpty(mail))
            {
                if (VerificationViewModel.IsEmail(mail))
                    model.Student.UserInfo.Email = mail.Trim();
                else
                {
                    model.Message = "电子邮箱错误";
                    model.State = 0;
                    return model;
                }

            }

            // QQ
            if (!string.IsNullOrEmpty(qq))
                model.Student.UserInfo.QQ = qq.Trim();


            // WeChat
            if (!string.IsNullOrEmpty(whatch))
                model.Student.UserInfo.WeChat = whatch.Trim();

            var schoolist = _school.GetAllList();
            var schoolid = 0;
            // 专业
            if (!string.IsNullOrEmpty(major))
            {
                major = major.Trim();
                var colleges = from c in schoolist
                               where c.InfoType == 4 && c.InfoName == major
                               select c;
                if (colleges.Count() != 1)
                {
                    model.Message = "学院信息不正确";
                    model.State = 0;
                    return model;
                }
                else
                {
                    schoolid = colleges.FirstOrDefault().ID;
                }
            }
            else
            {
                model.Message = "  学院信息不能为空";
                model.State = 0;
                return model;
            }
            //年级
            if (!string.IsNullOrEmpty(grade))
            {
                grade = grade.Trim();
                var majors = schoolist.Where(c => c.ParentID == schoolid && c.InfoName == grade);
                if (majors.Count() == 1)
                    schoolid = majors.FirstOrDefault().ID;
            }
            //班级
            if (!string.IsNullOrWhiteSpace(classname))
            {
                classname = classname.Trim();
                var classs = schoolist.Where(c => c.ParentID == schoolid && c.InfoName == classname);
                if (classs.Count() == 1)
                    schoolid = classs.FirstOrDefault().ID;
            }
            if (schoolid == 0)
            {
                model.Message = "  学院信息不能为空";
                model.State = 0;
                return model;
            }

            model.Student.SchoolID = schoolid;
            model.State = 1;
            return model;


        }
        #endregion
    }
}
