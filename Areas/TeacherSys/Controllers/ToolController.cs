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

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class ToolController : BaseTeacherController
    {
        private readonly IToolService _tool = new ToolService();
        private readonly ITagLibService _tag = new TagLibService();
        private readonly IToolTagService _tooltag = new ToolTagService();
        private readonly int ToolType = 2;

        public ActionResult Index()
        {
            CookieHelper.Del("keyword");
            var list = _tool.GetToolsByUserType(ToolType, Teacher.InfoID ?? 0,UserInfoType.Teacher );
            var pager = _tool.GetPageList(list);
            return View(pager);
        }

        public ActionResult Search(int? page)
        {
            string keyword = "";
            if (page != 0 && page != null)
                keyword = CookieHelper.GetValue("keyword");
            else
                CookieHelper.Del("keyword");
            var list = _tool.GetToolsByUserType(ToolType, Teacher.InfoID ?? 0, UserInfoType.Teacher);
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.FileName.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _tool.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var list = _tool.GetToolsByUserType(ToolType, Teacher.InfoID ?? 0, UserInfoType.Teacher);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.FileName.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            var pager = _tool.GetPageList(list);
            return View(pager);
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
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(1);
            var ToolTag = from c in _tag.GetAllList() where c.TagType == 2 select c;
            ViewBag.Tag = SelectListExtension.ToSelectList(ToolTag, d => d.TagName, d => d.ID.ToString(), "请选择");
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(Tool model, int[] tag)
        {
            model.ToolType = ToolType;
            model.CreateInfoType = (int)UserInfoType.Teacher;
            model.CreateInfoID = Teacher.InfoID;
            model.SchoolID = Teacher.SchoolID;
            _tool.SaveOrEdit(model);
            _tool.SaveToolTag(model.ID, tag);
            return RedirectToAction("Search");
        }

        public ActionResult Edit(int id)
        {
            var model = _tool.GetModel(id);
            ViewBag.IsSharing = SelectListHelper.GetIsOrNoSelectList(model.IsSharing);
            ViewBag.ToolType = SelectListHelper.GetTypeList(model.ToolType);
            var taglist = _tooltag.GeTagLibsByTool(model.ID);
            var list = _tooltag.GeTagLibsByType(model.ToolType ?? 1);
            if (model.ToolType != 1 && model.ToolType != 2) list = list.Take(0);
            ViewBag.Tag = list.ToSelectMultipleList(d => d.TagName, d => d.ID, "请选择", taglist);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id, Tool model, int[] tag)
        {
            model = _tool.GetModel(id);
            if (model == null) return View("404");
            UpdateModel(model);
            _tool.SaveOrEdit(model);
            _tool.DeleteToolTag(model.ID);
            _tool.SaveToolTag(model.ID, tag);
            return RedirectToAction("Search");
        }

    }
}
