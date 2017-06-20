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
using System.Collections.Generic;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class UserController : BaseAdminController
    {
        //
        // GET: /PRMS/User/
        private readonly IUserService _user = new UserService();
        private readonly IUserInfoService _info=new UserInfoService();
        private readonly IRoleService _role = new RoleService();
        private readonly ISchoolService _school = new SchoolService();
        public ActionResult Index()
        {
            DeleteCookie();
            var model = _user.GetDtoModel(ConfigHelper.UserID);
            var list = _user.GetAllList();
            if (model.User.SchoolID != null)
            {
                var schoolist = _school.GetUserChildSchool(model.User.SchoolID ?? 0);
                list = _user.GetUserList("", schoolist);
            }
            var pager = _user.GetPageList(list);
            return View(pager);
        }
        public ActionResult Search(string keyword, string userno, int? id)
        {
            var list = _user.GetList(d => d.State == 1 && d.UserInfo.UserType == 1);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                CookieHelper.SetObj("keyword", keyword);
                list = from c in list where c.UserInfo.TrueName.Contains(keyword) select c;
            }
            if (!string.IsNullOrWhiteSpace(userno))
            {
                CookieHelper.SetObj("userno", userno);
                list = from c in list where c.UserInfo.LoginName.Contains(userno) select c;
            }
            list = list.OrderByDescending(d => d.ID);
            return View("index", _user.GetPageList(list, id ?? 1));
        }
        [HttpPost]
        public ActionResult Search(string keyword, string userno)
        {
            CookieHelper.Del("keyword");
            var list = _user.GetList(d => d.State == 1 && d.UserInfo.UserType == 1);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                CookieHelper.SetObj("keyword", keyword);
                list = from c in list where c.UserInfo.TrueName.Contains(keyword) select c;
            }
            else
                CookieHelper.Del("keyword");
            if (!string.IsNullOrWhiteSpace(userno))
            {
                CookieHelper.SetObj("userno", userno);
                list = from c in list where c.UserInfo.LoginName.Contains(userno) select c;
            }
            else
                CookieHelper.Del("userno");
            var pager = _user.GetPageList(list);
            return View("index", pager);
        }
        public ActionResult Add()
        {
            var model = new UserDTO(ConfigHelper.SchoolID);
            ViewData["RoleID"] = _role.GetAllList().ToSelectMultipleList(d => d.RoleName, d => d.ID);
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model?.UserInfo?.Sex ?? 1);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(UserDTO model, FormCollection fc)
        {
            if (model.SchoolID == 0)
            {
                ModelState.AddModelError("SchoolID", "学校信息不可为空！");
                ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model?.UserInfo?.Sex ?? 1);
                ViewData["RoleID"] = _role.GetAllList().ToSelectMultipleList(d => d.RoleName, d => d.ID, model.RoleID.ToList());
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.UserInfo.Password))
                model.UserInfo.Password = model.UserInfo.LoginName;

            model.User.SchoolID = model.SchoolID;
            model.UserInfo.Password = StringHelper.Md5Encrypt(model.UserInfo.Password);
            model.UserInfo.SchoolID = model.SchoolID;
            model.UserInfo.UserType = 1;
            _user.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _user.GetDtoModel(id);
            ViewData["RoleID"] = _role.GetAllList().ToSelectMultipleList(d => d.RoleName, d => d.ID, model.RoleID.ToList());
            ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model.UserInfo.Sex ?? 1);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, UserDTO model, FormCollection fc)
        {
            if (model.RoleID == null) model.RoleID = new List<int?>();
            if (model.SchoolID == 0)
            {
                ModelState.AddModelError("SchoolID", "学校信息不可为空！");
                ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model?.UserInfo?.Sex ?? 1);
                ViewData["RoleID"] = _role.GetAllList().ToSelectMultipleList(d => d.RoleName, d => d.ID, model.RoleID.ToList());
                return View(model);
            }
            if (!model.RoleID.Any())
            {
                ModelState.AddModelError("RoleID", "至少选择一个角色！");
                ViewData["UserInfo.Sex"] = SelectListHelper.GetSexSelectList(model?.UserInfo?.Sex ?? 1);
                ViewData["RoleID"] = _role.GetAllList().ToSelectMultipleList(d => d.RoleName, d => d.ID, model.RoleID.ToList());
                return View(model);
            }
            model = _user.GetDtoModel(id);
            if (model.User.ID != 0)
                UpdateModel(model);
            model.User.SchoolID = model.SchoolID;
            model.UserInfo.SchoolID = model.SchoolID;
            _user.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Detail(int? id)
        {
            var model = _user.GetDtoModel(id ?? 0);
            if (model != null)
            {
                return View(model);
            }
            else
                return View("404");
        }
        public ActionResult Delete(int id)
        {
            var msg = _user.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public void DeleteCookie()
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("pageIndex");
        }
        public ActionResult ResetPwd(int? id)
        {
            return Json(_info.ResetPwd(id ?? 0), JsonRequestBehavior.AllowGet);
        }
    }
}
