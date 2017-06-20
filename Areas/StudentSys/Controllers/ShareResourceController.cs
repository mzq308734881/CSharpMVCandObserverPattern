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

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class ShareResourceController : BaseStudentController
    {
        private readonly IToolService _tool = new ToolService();
        private readonly int ToolType = 1;
        public ActionResult Search(int? page, int? sort)
        {
            string keyword = "";
            var list = _tool.GetSharingListBySchool(Student.SchoolID ?? 0, ToolType);
            ViewBag.Sort = SelectListHelper.GetSortList(0);
            if (sort != null)
            {
                list = _tool.GetSharingListBySchool(Student.SchoolID ?? 0, ToolType, (ToolSort)sort);
                CookieHelper.SetObj("Sort", sort.ToString());
            }
            if (page != 0 && page != null)
            {
                keyword = CookieHelper.GetValue("keyword");
                sort = StringHelper.StringToZero(CookieHelper.GetValue("Sort"));
            }
            else
            {
                CookieHelper.Del("keyword");
                CookieHelper.Del("Sort");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
                list = from c in list
                       where c.FileName.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
            CookieHelper.SetObj("keyword", keyword);
            var pager = _tool.GetPageList(list, page ?? 1);
            return View(pager);
        }
        [HttpPost]
        public ActionResult Search(string keyword,int? sort)
        {
            ViewBag.Sort = SelectListHelper.GetSortList(0);
            var list = _tool.GetSharingListBySchool(Student.SchoolID ?? 0, ToolType);
            if (sort != null)
            {
                list = _tool.GetSharingListBySchool(Student.SchoolID ?? 0, ToolType, (ToolSort)sort);
            }
            else
            {
                CookieHelper.Del("sort");
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                list = from c in list
                       where c.FileName.Contains(keyword) || c.Comment.Contains(keyword)
                       select c;
                CookieHelper.SetObj("keyword", keyword);
            }
            else
                CookieHelper.Del("keyword");
           
            var pager = _tool.GetPageList(list);
            return View(pager);
        }
        public ActionResult Detail(int id)
        {
            var model = _tool.GetModel(id);
            return View(model);
        }
    }
}
