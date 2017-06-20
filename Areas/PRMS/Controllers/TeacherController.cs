using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ViewModel;
using System.Data;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class TeacherController : BaseAdminController
    {
        //
        // GET: /PRMS/Teacher/
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly IUserInfoService _userInfo = new UserInfoService();
        private readonly ISchoolService _school = new SchoolService();
        public ActionResult Index()
        {
            CookieHelper.Del("teaname");
            CookieHelper.Del("teano");
            var model = new SchoolDTO() { TeacherPage = _teacher.GetPageList() };
            return View(model);
        }
        public ActionResult Search(int? id, int? page)
        {
            var model = new SchoolDTO() { SchoolID = id };
            var teaname = "";
            var teano = "";

            var list = _teacher.GetAllList();
            if (id != 0 && id != null)
            {
                teaname = CookieHelper.GetValue("teaname");
                teano = CookieHelper.GetValue("teano");
                var ids = _school.GetUserChildSchool(id ?? 0);
                list = from c in list where ids.Contains(c.SchoolID ?? 0) select c;
            }
            else
            {
                CookieHelper.Del("teaname");
                CookieHelper.Del("teano");
            }
            if (!string.IsNullOrWhiteSpace(teaname))
            {
                list = from c in list where c.UserInfo.LoginName.Contains(teaname) select c;
                CookieHelper.SetObj("teaname", teaname);
            }
            if (!string.IsNullOrWhiteSpace(teano))
            {
                list = from c in list where c.UserInfo.TrueName.Contains(teano) select c;
                CookieHelper.SetObj("teano", teano);
            }
            model.TeacherPage = _teacher.GetPageList(list, page ?? 1);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Search(string teaname, string teano)
        {
            var list = _teacher.GetAllList();
            if (!string.IsNullOrWhiteSpace(teaname))
            {
                CookieHelper.SetObj("teaname", teaname);
                list = from c in list where c.UserInfo.TrueName.Contains(teaname) select c;
            }
            else
                CookieHelper.Del("teaname");
            if (!string.IsNullOrWhiteSpace(teano))
            {
                CookieHelper.SetObj("teano", teano);
                list = from c in list where c.UserInfo.LoginName.Contains(teano) select c;
            }
            else
                CookieHelper.Del("teano");
            var model = new SchoolDTO() { TeacherPage = _teacher.GetPageList(list) };
            return PartialView(model);
        }
        public ActionResult Add(int? id)
        {
            var school = _school.GetModel(id ?? 0);
            var model = new Teacher() { SchoolID = id };
            model.School = school;
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(1);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Teacher model, FormCollection fc)
        {
            model.ID = 0;
            model.UserInfo.Password = StringHelper.Md5Encrypt(model.UserInfo.LoginName);
            model.UserInfo.SchoolID = model.SchoolID;
            _teacher.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _teacher.GetModel(id);
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model.UserInfo.Sex ?? 1);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Teacher model, FormCollection fc)
        {
            model = _teacher.GetModel(id);
            if (model.ID != 0)
                UpdateModel(model);
            model.UserInfo.SchoolID = model.SchoolID;
            _teacher.SaveOrEdit(model);
            return RedirectToAction("index");
        }

        public ActionResult Detail(int? id)
        {
            var model = _teacher.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }
        public ActionResult Delete(int id)
        {
            var msg = _teacher.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CZPwd(int? id)
        {
            return Json(_userInfo.ResetPwd(id ?? 0), JsonRequestBehavior.AllowGet);
        }

        #region 导入Excel批量添加老师数据
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
            var user = _admin.LoginUser();
            if (user == null) return Content("用户不存在!");
            var excelUrl = CookieHelper.GetValue("FileUrl");
            var conn = ConfigHelper.ServerPath + excelUrl;
            var list = GetList(conn);
            list = list.Where(d => d.State == 1).ToList();

            foreach (var model in from item in list where item.State == 1 select item.Teacher)
            {
                model.UserInfo.CreateInfoID = user.InfoID;
                model.UserInfo.SchoolID = model.SchoolID;
                model.UserInfo.Password = StringHelper.Md5Encrypt(model.UserInfo.LoginName);
                _teacher.SaveOrEdit(model);
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

                if (model.State != 0)
                {
                    if (loginname.Contains(model.Teacher.UserInfo.LoginName))
                    {
                        model.Message = " 系统中已存在此登陆账号，不会被导入";
                        model.State = 2;
                    }
                    var count = from c in list
                                where c.Teacher.UserInfo.LoginName == model.Teacher.UserInfo.LoginName
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
            var school = dr[0].ToString();
            var major = dr[1].ToString();
            var loginname = dr[2].ToString();
            var truename = dr[3].ToString();
            var sex = dr[4].ToString();
            var isexpret = dr[5].ToString();
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
            model.Teacher.UserInfo.LoginName = loginname;
            model.Teacher.UserInfo.TrueName = truename;

            //性别
            if (string.IsNullOrEmpty(sex))
                model.Teacher.UserInfo.Sex = 1;
            model.Teacher.UserInfo.Sex = sex.Contains("女") ? 2 : 1;

            // 是否专家
            if (string.IsNullOrEmpty(isexpret))
                model.Teacher.IsExpert = 0;
            model.Teacher.IsExpert = isexpret.Contains("否") ? 0 : 1;

            // 联系电话
            if (!string.IsNullOrEmpty(tel))
            {
                if (VerificationViewModel.IsMobilePhone(tel))
                    model.Teacher.UserInfo.Tel = tel.Trim();
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
                    model.Teacher.UserInfo.Email = mail.Trim();
                else
                {
                    model.Message = "电子邮箱错误";
                    model.State = 0;
                    return model;
                }

            }

            // QQ
            if (!string.IsNullOrEmpty(qq))
                model.Teacher.UserInfo.QQ = qq.Trim();


            // WeChat
            if (!string.IsNullOrEmpty(whatch))
                model.Teacher.UserInfo.WeChat = whatch.Trim();

            var schoolist = _school.GetAllList().Where(d => d.State == 1);
            var schoolid = 0;
            // 学院
            if (!string.IsNullOrEmpty(school))
            {
                school = school.Trim();
                var colleges = from c in schoolist
                               where c.InfoType == 2 && c.InfoName == school
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

            if (!string.IsNullOrEmpty(major))
            {
                major = major.Trim();
                var majors = from c in schoolist
                             where c.ParentID == schoolid
                             where c.InfoName == major
                             select c;
                model.Teacher.SchoolID = majors.Count() != 1 ? schoolid : majors.FirstOrDefault().ID;
            }
            else
                model.Teacher.SchoolID = schoolid;

            model.State = 1;
            return model;


        }
        #endregion
    }

}
