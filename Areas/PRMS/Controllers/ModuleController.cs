using System.Linq;
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
    public class ModuleController : BaseAdminController
    {
        //
        // GET: /PRMS/Module/
        private readonly IModuleService _module = new ModuleService();
        private readonly IModuleActionService _action = new ModuleActionService();
        public ActionResult Index(int? id)
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("pageIndex");
            var pager = _module.GetPageList();
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
                keyword = CookieHelper.GetValue("keyword");
            var source = _module.GetAllList();
            if (!string.IsNullOrEmpty(keyword))
                source = from c in source where c.ModuleName.Contains(keyword) select c;
            var pager = _module.GetPageList(source, page ?? 1);
            return View("index", pager);
        }
        public ActionResult Add()
        {
            var model = new Module();
            ViewData["ParentID"] = _module.ParentList().ToSelectList(d => d.ModuleName, d => d.ID.ToString(), "父级栏目");
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(Module model, FormCollection fc)
        {
            if (model.ParentID == 0) model.ParentID = null;
            _module.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Action(int? id)
        {
            ViewBag.MoudelID = id;
            var list = _action.GetActionByMoudel(id ?? 0);
            return View(list);
        }
        public ActionResult ActionAdd(int id)
        {
            var model = new ModuleAction() { Module = _module.GetModel(id), ModuleID = id };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ActionAdd(int id, ModuleAction model, FormCollection fc)
        {
            model.ID = 0;
            model.ModuleID = id;
            _action.SaveOrEdit(model);
            var list = _action.GetActionByMoudel(id);
            ViewBag.MoudelID = id;
            return Redirect("/prms/module/action/" + id);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _module.GetModel(id);
            var pid = model.ParentID ?? 0;
            ViewData["ParentID"] = _module.ParentList().ToSelectList(d => d.ModuleName, d => d.ID.ToString(), "父级栏目", pid);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, Module model, FormCollection fc)
        {
            model = _module.GetModel(id);
            if (model != null)
            {
                UpdateModel(model);
                _module.SaveOrEdit(model);
                return RedirectToAction("index");
            }
            else
                return View("404");
        }

        public ActionResult Detail(int? id)
        {
            var model = _module.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }
        public ActionResult Delete(int id)
        {
            var msg = _module.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}
