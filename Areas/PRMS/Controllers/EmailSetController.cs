using System;
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
    public class EmailSetController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly IEmailSetService _emailset = new EmailSetService();
        public ActionResult Index()
        {
            var pager = _emailset.GetPageList();
            return View(pager);
        }

        public ActionResult Search(int? id)
        {
            return View("index");
        }
        public ActionResult Add()
        {
            var model = new EmailSet();
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(EmailSet model, string pwd)
        {
            var user = _admin.LoginUser();
            model.CreateUserID = user.ID;
            model.CreateTime = DateTime.Now;
            model.Password = StringHelper.Md5Encrypt(pwd);
            _emailset.SaveOrEdit(model);
            _emailset.ChangeListState(model.ID);
            return RedirectToAction("index");
        }
        public ActionResult Edit(int id)
        {
            var model = _emailset.GetModel(id);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmailSet model, string pwd, int? id)
        {
            model = _emailset.GetModel(id ?? 0);
            if (model == null) return View("404");
            var a = ModelState.IsValid;
            UpdateModel(model);
            if (!string.IsNullOrWhiteSpace(pwd))
            {
                model.Password = StringHelper.Md5Encrypt(pwd);
                _emailset.SaveOrEdit(model);
            }
            return RedirectToAction("index");
        }
        public ActionResult Enable(int? id)
        {
            return Json(_emailset.ChangeListState(id ?? 0), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int? id)
        {
            return Json(_emailset.Delete(id ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id)
        {
            var model = _emailset.GetModel(id);
            return View(model);
        }
    }
}
