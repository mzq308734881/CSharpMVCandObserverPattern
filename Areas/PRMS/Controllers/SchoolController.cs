using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Core;
using LanShanPRMS.IService;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class SchoolController : BaseAdminController
    {
        //
        // GET: /PRMS/School/
        private readonly ISchoolService _school = new SchoolService();
        private readonly IUserService _user = new UserService();
        public ActionResult Index()
        {
            var list = _school.GetAllList().Where(d => d.ParentID != null && d.InfoType == 2).ToList();
            var model = new SchoolDTO() { SchoolList = list };
            return View(model);
        }
        public ActionResult Search(int? id, int? page)
        {
            var model = new SchoolDTO() { SchoolID = id };
            var keyword = "";
            var list = _school.GetLevelDownByCurrentID(id ?? 0);
            if (id != 0 && id != null)
            {
                CookieHelper.Del("keyword");
                var ids = _school.GetUserChildSchool(id ?? 0);
                list = from c in list where ids.Contains(c.ID) select c;
            }
            else
            {
                keyword = CookieHelper.GetValue("keyword");
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    list = from c in list where c.InfoName.Contains(keyword) select c;

                }
            }
            if (id != 0 && id != null)
            {
                keyword = CookieHelper.GetValue("keyword");
                var ids = _school.GetUserChildSchool(id ?? 0);
                list = from c in list where ids.Contains(c.ID) select c;
            }
            else
            {
                CookieHelper.Del("keyword");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list where c.InfoName.Contains(keyword) select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            model.SchoolList = _school.GetPageList(list, page ?? 1);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _school.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
                list = list.Where(d => d.InfoName.Contains(keyword));
            return PartialView(list);
        }

        public ActionResult Add(int? id)
        {
            var school = _school.GetModel(id ?? 0);
            var model = new School() { ParentID = id };
            if (school != null)
                model.School2 = school;
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(School model)
        {
            model.ID = 0;
            var parent = _school.GetModel(model.ParentID ?? 0);
            if (parent == null)
            {
                ModelState.AddModelError("ParentID", "请选择上级学校信息！");
                return View(model);
            }
            model.InfoType = parent.InfoType + 1;
            model.CreateUserID = Admin.ID;
            _school.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Edit(int? id)
        {
            var model = _school.GetModel(id ?? 0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, School model)
        {
            model = _school.GetModel(id);
            UpdateModel(model);
            var parent = _school.GetModel(model.ParentID ?? 0);
            if (parent == null) return View("404");
            model.InfoType = parent.InfoType + 1;
            _school.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Detail(int? id)
        {
            var model = _school.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }

        public ActionResult Delete(int id)
        {
            return Json(_school.Delete(id), JsonRequestBehavior.AllowGet);
        }

        #region 导入
        public ActionResult Import()
        {
            CookieHelper.Del("FilePath");
            return View();
        }
        [HttpPost]
        public ActionResult Import(string filepath)
        {
            try
            {
                string conn = ConfigHelper.ServerPath + filepath;
                var list = GetList(conn);
                CookieHelper.SetObj("FilePath", filepath);
                return View(list);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("院系导入出错", ex);
                return Content("<script>alert('院系导入失败，请使用正确的模板，并填写所有信息');window.location.href ='/prms/school/import';</script>");
            }

        }
        //导入数据库
        public ActionResult ImportDB()
        {
            string ExcelUrl = CookieHelper.GetValue("FilePath");
            string conn = ConfigHelper.ServerPath + ExcelUrl;
            var list = GetList(conn);
            list = list.Where(d => d.State == 1).ToList();
            var schoolid = ConfigHelper.SchoolID;
            if (schoolid == 0)
                return Json(new RootViewModel() { Message = "未找到对应学校ID！" }, JsonRequestBehavior.AllowGet);
            var model = _school.SaveList(list, Admin.ID, Admin.SchoolID ?? 0);
            CookieHelper.Del("FileUrl");
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        ///获取excel里面的数据并转化成对应的List       
        public List<ImportSchoolViewModel> GetList(string conn)
        {
            var list = new List<ImportSchoolViewModel>();
            DataSet ds = VerificationViewModel.ExcelToDataSet(conn);
            var school = _school.GetAllList().ToList();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                ImportSchoolViewModel model = GetModel(dr, school);
                if (model != null)
                {
                    model.ID = i + 1;
                    list.Add(model);
                }
            }
            return list;
        }
        ///获取excel一行的记录转化成实体
        public ImportSchoolViewModel GetModel(DataRow dr, List<School> list)
        {
            if (dr == null)
                return null;
            var department = dr[0].ToString();
            var collage = dr[1].ToString();
            var specialty = dr[2].ToString();
            var gradename = dr[3].ToString();
            var classname = dr[4].ToString();
            if (string.IsNullOrEmpty(department))
                return new ImportSchoolViewModel()
                {
                    Message = " 此为空行，导入时会被忽略！",
                    State = 2
                };
            var model = new ImportSchoolViewModel() { Department = department.Trim() };
            if (!string.IsNullOrWhiteSpace(collage))
            {
                model.College = collage.Trim();
                if (string.IsNullOrWhiteSpace(model.Department))
                {
                    model.State = 0;
                    model.Message = "院名称为空无法导入！";
                }
            }

            if (!string.IsNullOrWhiteSpace(specialty))
            {
                model.Specialty = specialty.Trim();
                if (string.IsNullOrWhiteSpace(model.College))
                {
                    model.State = 0;
                    model.Message = "系名称为空无法导入！";
                }
            }
            if (!string.IsNullOrWhiteSpace(gradename))
            {
                model.Grade = gradename.Trim();
                if (string.IsNullOrWhiteSpace(model.Specialty))
                {
                    model.State = 0;
                    model.Message = "专业名称为空无法导入！";
                }
            }
            if (string.IsNullOrWhiteSpace(classname)) return model;
            model.ClassName = classname.Trim();
            if (!string.IsNullOrWhiteSpace(model.Grade)) return model;
            model.State = 0;
            model.Message = "年级名称为空无法导入！";
            return model;
        }
        #endregion
    }
}
