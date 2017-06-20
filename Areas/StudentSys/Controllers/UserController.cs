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
using LanShanPRMS.Core;

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    public class UserController : BaseStudentController
    {
        private readonly IStudentService _student = new StudentService();
        [UserStudent]
        public ActionResult Index()
        {
            return View(Student);
        }

        [UserStudent]
        public ActionResult Search()
        {
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(Student.UserInfo.Sex ?? 1);
            return View(Student);
        }
        [UserStudent]
        public ActionResult Edit()
        {
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(Student.UserInfo.Sex ?? 1);
            return View(Student);
        }
        [UserStudent]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Student model)
        {
            model = _student.GetModel(ConfigHelper.StudentID);
            if (model.ID != 0)
                UpdateModel(model);
            _student.SaveOrEdit(model);
            return RedirectToAction("search");
        }
        [UserStudent]
        public ActionResult Pwd()
        {
            return View();
        }
        [UserStudent]
        [HttpPost, ValidateInput(false)]
        public ActionResult Pwd(string oldpwd, string newpwd, string newpwd1)
        {
            var model = _admin.LoginStudent();
            if (model == null)
                return Json(new RootViewModel("未找到对应学生，请刷新后重试！"), JsonRequestBehavior.AllowGet);
            model.UserInfo.Password = StringHelper.Md5Decrypt(model.UserInfo.Password);
            if (oldpwd != model.UserInfo.Password)
                return Json(new RootViewModel("新旧密码不匹配"), JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel("重复输入密码错误！"), JsonRequestBehavior.AllowGet);
            if (newpwd == oldpwd)
                return Json(new RootViewModel("新旧密码不能相同！"), JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel("两次密码不一致！"), JsonRequestBehavior.AllowGet);
            model.UserInfo.Password = StringHelper.Md5Encrypt(newpwd);
            _student.SaveOrEdit(model);
            return Json(new RootViewModel(1, "修改成功！"), JsonRequestBehavior.AllowGet);
        }
        [UserStudent(false)]
        public ActionResult EditPwd()
        {
            return View();
        }
        [UserStudent(false)]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPwd(string oldpwd, string newpwd, string newpwd1)
        {
            var model = _admin.LoginStudent();
            if (model == null)
                return Json(new RootViewModel("未找到对应学生，请刷新后重试！"), JsonRequestBehavior.AllowGet);
            model.UserInfo.Password = StringHelper.Md5Decrypt(model.UserInfo.Password);
            if (oldpwd != model.UserInfo.Password)
                return Json(new RootViewModel("新旧密码不匹配"), JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel("重复输入密码错误！"), JsonRequestBehavior.AllowGet);
            if (newpwd == oldpwd)
                return Json(new RootViewModel("新旧密码不能相同！"), JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel("两次密码不一致！"), JsonRequestBehavior.AllowGet);
            model.UserInfo.Password = StringHelper.Md5Encrypt(newpwd);
            _student.SaveOrEdit(model);
            return Json(new RootViewModel(1, "修改成功！"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail()
        {
            return View();
        }
    }
}
