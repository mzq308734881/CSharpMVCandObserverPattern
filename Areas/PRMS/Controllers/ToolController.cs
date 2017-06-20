using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Service;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class ToolController : BaseAdminController
    {
        private readonly IToolService _tool = new ToolService();
        private readonly IToolTagService _tooltag = new ToolTagService();
        private readonly ITagLibService _tag = new TagLibService();
        private readonly int _resourcesType = 1;
        private readonly int _toolType = 2;
        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("ToolType");
            CookieHelper.Del("IsSharing");
            CookieHelper.Del("Sort");
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            ViewBag.IsSharing = SelectListHelper.GetIsSharing(0);
            ViewBag.Sort = SelectListHelper.GetSortList(0);
            var pager = _tool.GetPageList();
            return View(pager);
        }
        public ActionResult Search(int? id)
        {
            var keyword = "";
            string ToolType = "";
            int? IsSharing = 0;
            int? Sort = 0;
            ViewBag.Sort = SelectListHelper.GetSortList(0);
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            ViewBag.IsSharing = SelectListHelper.GetIsSharing(0);
            var list = _tool.GetIsTop();
            if (Sort != null)
            {
                list = _tool.GetIsHot((ToolSort)Sort);
                CookieHelper.SetObj("Sort", Sort.ToString());
            }
            if (id != 0 && id != null)
            {
                keyword = CookieHelper.GetValue("keyword");
                ToolType = CookieHelper.GetValue("ToolType");
                IsSharing = StringHelper.StringToInt(CookieHelper.GetValue("IsSharing"));
                Sort = StringHelper.StringToZero(CookieHelper.GetValue("Sort"));
            }
            else
            {
                CookieHelper.Del("keyword");
                CookieHelper.Del("ToolType");
                CookieHelper.Del("IsSharing");
                CookieHelper.Del("Sort");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                CookieHelper.SetObj("keyword", keyword);
                list = from c in list.Where(d => d.FileName.Contains(keyword)) select c;
            }
            if (!string.IsNullOrWhiteSpace(ToolType))
            {
                var type = int.Parse(ToolType);
                list = from c in list
                       where c.ToolType == type
                       select c;
                CookieHelper.SetObj("ToolType", ToolType);
            }
            if (IsSharing != null)
            {
                list = from c in list
                       where c.IsSharing == IsSharing
                       select c;
                CookieHelper.SetObj("IsSharing", IsSharing.ToString());
            }
            list = list.OrderByDescending(d => d.IsTop);
            var pager = _tool.GetPageList(list, id ?? 1);
            return View("index", pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword, string ToolType, int? IsSharing, int? Sort)
        {

            var list = _tool.GetIsTop();
            if (Sort != null)
            {
                list = _tool.GetIsHot((ToolSort)Sort);
                CookieHelper.SetObj("Sort", Sort.ToString());
            }
            else
                CookieHelper.Del("Sort");
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.FileName.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
            if (!string.IsNullOrWhiteSpace(ToolType))
            {
                var type = int.Parse(ToolType);
                list = from c in list
                       where c.ToolType == type
                       select c;
                CookieHelper.SetObj("ToolType", ToolType);
            }
            else
                CookieHelper.Del("ToolType");
            if (IsSharing != null)
            {
                list = from c in list
                       where c.IsSharing == IsSharing
                       select c;
                CookieHelper.SetObj("IsSharing", IsSharing.ToString());
            }
            else
                CookieHelper.Del("issharing");
            list = list.OrderByDescending(d => d.IsTop);
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            ViewBag.IsSharing = SelectListHelper.GetIsSharing(0);
            ViewBag.Sort = SelectListHelper.GetSortList(0);
            var pager = _tool.GetPageList(list);
            return View("index", pager);
        }



        public ActionResult Delete(int id)
        {
            return Json(_tool.Delete(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int id)
        {
            var model = _tool.GetModel(id);
            return View(model);
        }

        public ActionResult Add()
        {
            var model = new Tool();
            var toolTag = from c in _tag.GetAllList() where c.TagType == 1 select c;
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(1);
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            ViewBag.Tag = toolTag.ToSelectList(d => d.TagName, d => d.ID.ToString(), "请选择");
            return View(model);
        }
        public ActionResult AddTool()
        {
            var model = new Tool();
            var toolTag = from c in _tag.GetAllList() where c.TagType == 2 select c;
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(1);
            ViewBag.ToolType = SelectListHelper.GetTypeList(0);
            ViewBag.Tag = toolTag.ToSelectList(d => d.TagName, d => d.ID.ToString(), "请选择");
            return View(model);
        }
        //添加资源
        [HttpPost]
        public ActionResult Add(Tool model, int[] tag)
        {
            model.CreateInfoType = (int)UserInfoType.Admin;
            model.CreateInfoID = Admin.InfoID;
            model.ToolType = _resourcesType;
            model.SchoolID = Admin.SchoolID;
            _tool.SaveOrEdit(model);
            _tool.SaveToolTag(model.ID, tag);
            return RedirectToAction("Search");
        }
        //添加工具  默认type的等于2
        [HttpPost]
        public ActionResult AddTool(Tool model, int[] tag)
        {
            model.CreateInfoType = (int)UserInfoType.Admin;
            model.CreateInfoID = Admin.InfoID;
            model.SchoolID = Admin.SchoolID;
            model.ToolType = _toolType;
            _tool.SaveOrEdit(model);
            _tool.SaveToolTag(model.ID, tag);
            return RedirectToAction("Search");
        }

        public ActionResult Edit(int id)
        {
            var model = _tool.GetModel(id);
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(model.IsSharing);
            ViewBag.ToolType = SelectListHelper.GetTypeList(model.ToolType);
            ViewData["www"] = model.FileName;
            var taglist = _tooltag.GeTagLibsByTool(model.ID);
            var list = _tooltag.GeTagLibsByType(model.ToolType ?? 1);
            if (model.ToolType != 1 && model.ToolType != 2) list = list.Take(0);
            ViewBag.Tagid = list.ToSelectMultipleList(d => d.TagName, d => d.ID, "请选择", taglist);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id, Tool model, int[] tagid)
        {
            model = _tool.GetModel(id);
            if (model == null) return View("404");
            UpdateModel(model);
            _tool.SaveOrEdit(model);
            _tool.DeleteToolTag(model.ID);
            _tool.SaveToolTag(model.ID, tagid);
            return RedirectToAction("Search");
        }
        public ActionResult IsTop(int id)
        {
            return Json(_tool.IsTop(id), JsonRequestBehavior.AllowGet);
        }

    }
}
