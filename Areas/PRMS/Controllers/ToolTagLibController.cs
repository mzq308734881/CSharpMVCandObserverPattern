using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class ToolTagLibController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly IToolTagService _tooltag = new ToolTagService();
        public ActionResult Index()
        {
            VerificationViewModel.DeleteCookie();
            var pager = _tooltag.GetPageList();
            return View("search", pager);
        }
        public ActionResult Search(string keyword,int?id) {
            var list = _tooltag.GetList(d => d.State == 1);
            if (!string.IsNullOrWhiteSpace(keyword)) {
                CookieHelper.SetObj("keyword", keyword);
                list = from c in list.Where(d => d.TagLib.TagName.Contains(keyword)) select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            list = list.OrderByDescending(d => d.ID);
            return View("search", _tooltag.GetPageList(list, id??1));
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            CookieHelper.Del("keyword");
            var list = _tooltag.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.TagLib.TagName.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            var pager = _tooltag.GetPageList(list);
            return View(pager);
        }
        public ActionResult Add()
        {
            var model = new ToolTagLib();
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(ToolTagLib model)
        {
            _tooltag.SaveOrEdit(model);
            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var model = _tooltag.GetModel(id);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, ToolTagLib model)
        {
            model = _tooltag.GetModel(id);
            if (model.ID != 0)
                UpdateModel(model);
            _tooltag.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Detail(int id)
        {
            var model = _tooltag.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _tooltag.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}
