using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class TagLibController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITagLibService _taglib = new TagLibService();
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            ViewBag.TagType = SelectListHelper.GetTypeList(0);
            var pager = _taglib.GetPageList();
            return View(pager);
        }
        public ActionResult Search(string keyword,int?id,string tagType)
        {
            var list = _taglib.GetAllList();
            ViewBag.TagType = SelectListHelper.GetTypeList(0);
             if (id != 0 && id != null){
                keyword = CookieHelper.GetValue("keyword");
                tagType = CookieHelper.GetValue("TagType");
            }
            else
            {
                CookieHelper.Del("keyword");
                CookieHelper.Del("TagType");
            }
             if (!string.IsNullOrWhiteSpace(keyword))
                 list = from c in list
                        where c.TagName.Contains(keyword)
                        select c;
            CookieHelper.SetObj("keyword", keyword);
            if (!string.IsNullOrWhiteSpace(tagType))
             {
                 var type = int.Parse(tagType);
                 list = from c in list
                        where c.TagType == type
                        select c;
                CookieHelper.SetObj("TagType", tagType);
            }
            return View("index", _taglib.GetPageList(list,id??1));
        }
        [HttpPost]
        public ActionResult Search(string keyword, string tagType)
        {
            CookieHelper.Del("keyword");
            ViewBag.TagType = SelectListHelper.GetTypeList(0);
            var list = _taglib.GetAllList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.TagName.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            if (!string.IsNullOrWhiteSpace(tagType))
            {
                var type = int.Parse(tagType);
                list = from c in list
                       where c.TagType == type
                       select c;
                CookieHelper.SetObj("TagType", tagType);
            }
            else
                CookieHelper.Del("TagType");
            var pager = _taglib.GetPageList(list);
            return View("index",pager);
        }
        public ActionResult Add()
        {
            var model = new TagLib();
            ViewBag.TagType = SelectListHelper.GetTypeList(0);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(TagLib model)
        {
            _taglib.SaveOrEdit(model);

            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var model = _taglib.GetModel(id);
            ViewBag.TagType = SelectListHelper.GetTypeList(model.TagType);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id,TagLib model)
        {
            model = _taglib.GetModel(id);
            if (model.ID != 0)
                UpdateModel(model);
            _taglib.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        public ActionResult Detail(int id)
        {
            var model = _taglib.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _taglib.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}
