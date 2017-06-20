using System;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class RoleController : BaseAdminController
    {
        //
        // GET: /PRMS/role/
        private readonly IRoleService _role = new RoleService();
        private readonly IModuleService _module = new ModuleService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("pageIndex");
            var pager = _role.GetPageList();
            return View(pager);
        }
        public ActionResult Search(int? page)
        {
            CookieHelper.SetObj("pageIndex", page.ToString());
            var keyword = "";
            if (page == null)
            {
                keyword = Request.Form["keyword"] ?? "";
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = StringHelper.ClearFormat(keyword);
                    CookieHelper.SetObj("keyword", keyword);
                }
            }
            else
            {
                keyword = CookieHelper.GetValue("keyword");
            }
            var pager = _role.GetPageList(keyword, page ?? 1);
            return View("index", pager);
        }
        [HttpGet]
        public ActionResult Add()
        {
            var model = new Role();
            ViewBag.Pan = _module.ParentList();
            ViewBag.Two = _module.TowModules();
            ViewBag.Three = _module.ThreeModules();
            return View(model);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Role model, FormCollection fc,string actionid)
        {
            _role.SaveOrEdit(model);
            if (string.IsNullOrEmpty(actionid))
                return RedirectToAction("index");
            var actionidlist = actionid.Split(',');
            _role.SaveRoleModule(actionidlist, model.ID);
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var model = _role.GetModel(id);
            ViewBag.Pan = _module.ParentList();
            ViewBag.Two = _module.TowModules();
            ViewBag.Three = _module.ThreeModules();
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Role model, FormCollection fc)
        {
            model = _role.GetModel(id);
            if (model == null)
                return View("404");
            UpdateModel(model);

            _role.SaveOrEdit(model);
            _role.DeleteRoleModule(id);
            var moduleIDlist = Request["actionid"];
            if (string.IsNullOrEmpty(moduleIDlist))
                return RedirectToAction("index");
            var actionlist = moduleIDlist.Split(',');
            _role.SaveRoleModule(actionlist, model.ID);
            return RedirectToAction("index");
        }

        public ActionResult Detail(int? id)
        {
            var model = _role.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }

        public ActionResult Delete(int id)
        {
            var msg = _role.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public void DeleteCookie()
        {

        }
        public Pager<Role> GetPageList()
        {
            var keyword = CookieHelper.GetValue("keyword");
            var pageIndex = CookieHelper.GetValue("pageIndex");
            if (string.IsNullOrEmpty(pageIndex))
                pageIndex = "1";
            var pager = _role.GetPageList(keyword, int.Parse(pageIndex));
            return pager;
        }
        public ActionResult Return()
        {
            var list = GetPageList();
            return View("Search", list);
        }
    }
}
