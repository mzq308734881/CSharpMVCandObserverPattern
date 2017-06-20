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

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    public class UserController : BaseTeacherController
    {
        private readonly ITeacherService _teacher = new TeacherService();
        [UserTeacher]
        public ActionResult Index()
        {
            return View(Teacher);
        }
        [UserTeacher]
        public ActionResult Edit()
        {
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(Teacher.UserInfo.Sex ?? 1);
            return View(Teacher);
        }
        [UserTeacher]
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Teacher model)
        {
            model = _teacher.GetModel(Teacher.ID);
            if (model.ID != 0)
                UpdateModel(model);
            _teacher.SaveOrEdit(model);
            return RedirectToAction("search");
        }
        [UserTeacher]
        public ActionResult Pwd()
        {
            return View(Teacher);
        }
        [UserTeacher]
        [HttpPost, ValidateInput(false)]
        public ActionResult Pwd(string oldpwd, string newpwd, string newpwd1)
        {
            var model = _teacher.GetModel(Teacher.ID);
            if (model == null)
                return View("404");
            model.UserInfo.Password = StringHelper.Md5Decrypt(model.UserInfo.Password);
            if (oldpwd != model.UserInfo.Password)
                return Json(new RootViewModel() { Message = "新旧密码不匹配" }, JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel() { Message = "重复输入密码错误！！" }, JsonRequestBehavior.AllowGet);
            if (newpwd == oldpwd)
                return Json(new RootViewModel() { Message = "新旧密码不能相同！！" }, JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel() { Message = "两次密码不一致" }, JsonRequestBehavior.AllowGet);

            model.UserInfo.Password = StringHelper.Md5Encrypt(newpwd);
            _teacher.SaveOrEdit(model);
            return Json(new RootViewModel() { Message = "修改成功！！", State = 1 }, JsonRequestBehavior.AllowGet);
        }
        [UserTeacher(false)]
        public ActionResult EditPwd()
        {
            var model = _teacher.LoginTeacher();
            return View(model);
        }
        [UserTeacher(false)]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPwd(string oldpwd, string newpwd, string newpwd1)
        {
            var model = _teacher.GetModel(Teacher.ID);
            if (model == null)
                return View("404");
            model.UserInfo.Password = StringHelper.Md5Decrypt(model.UserInfo.Password);
            if (oldpwd != model.UserInfo.Password)
                return Json(new RootViewModel() { Message = "新旧密码不匹配" }, JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel() { Message = "重复输入密码错误！！" }, JsonRequestBehavior.AllowGet);
            if (newpwd == oldpwd)
                return Json(new RootViewModel() { Message = "新旧密码不能相同！！" }, JsonRequestBehavior.AllowGet);
            if (newpwd != newpwd1)
                return Json(new RootViewModel() { Message = "两次密码不一致" }, JsonRequestBehavior.AllowGet);

            model.UserInfo.Password = StringHelper.Md5Encrypt(newpwd);
            _teacher.SaveOrEdit(model);
            return Json(new RootViewModel() { Message = "修改成功！！", State = 1 }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail()
        {
            return View();
        }
    }
}
