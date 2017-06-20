using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.IService;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class ProcessController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly ITreatiseProcessEventService _event = new TreatiseProcessEventService();

        #region  流程相关
        public ActionResult Index(int? page)
        {
            var id = ConfigHelper.TreatiseID;
            var model = _treatise.GetModel(id);
            var trea = new TreatiseConfigViewModel(model)
            {
                ProcessList = model.TreatiseProcesses.Where(d => d.State == 1).ToList()
            };
            return View("search", trea);
        }
        public ActionResult Search(int? page)
        {
            var id = ConfigHelper.TreatiseID;
            var model = _treatise.GetModel(id);
            var trea = new TreatiseConfigViewModel(model)
            {
                ProcessList = model.TreatiseProcesses.Where(d => d.State == 1).ToList()
            };
            return View("search", trea);
        }
        public ActionResult Add()
        {

            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            if (trea == null)
                return View("404");
            var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var sort = (from c in trea.TreatiseProcesses where c.State == 1 select c.Sort).ToList();
            list = (from c in list where !sort.Contains(c) select c).ToList();
            ViewBag.Sort = list.ToSelectList(d => d.ToString(), d => d.ToString());
            var model = new TreatiseProcess() { TreatiseID = id };
            var pro = trea.TreatiseProcesses.Where(d => d.State == 1).OrderByDescending(d => d.Sort).FirstOrDefault();
            if (pro != null) model.StartTime = pro.EndTime;
            ViewBag.ActionList = _event.GetAllList();
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Add(TreatiseProcess model, int[] eventid)
        {
            var id = ConfigHelper.TreatiseID;
            var treatise = _treatise.GetModel(id);
            if (treatise == null) return View("404");
            model.ID = 0;
            model.TreatiseID = id;
            model.CreateUserID = Admin.ID;
            model.TreatiseProcessEvents = _event.GetProcessEventList(model.ID, eventid);
            _process.SaveOrEdit(model);
            return RedirectToAction("index");
        }
        //[HttpPost, ValidateInput(false)]
        //public JsonResult Add(TreatiseProcess model)
        //{
        //    var id = ConfigHelper.TreatiseID;
        //    var treatise = _treatise.GetModel(id);
        //    if (treatise == null) return Json(new RootViewModel() { Message = "参数错误，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
        //    var user = _admin.LoginUesr();
        //    model.ID = 0;
        //    model.TreatiseID = id;
        //    model.CreateUserID = user.ID;
        //    _process.SaveOrEdit(model);
        //    return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Edit(int id)
        {
            var model = _process.GetModel(id);
            if (model == null) return View("404");
            var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            //当论文流程的排序sort
            var sort = (from c in model.Treatise.TreatiseProcesses where c.ID != id && c.State == 1 select c.Sort).ToList();
            list = (from c in list where !sort.Contains(c) select c).ToList();
            ViewBag.Sort = list.ToSelectList(d => d.ToString(), d => d.ToString(), model.Sort);
            ViewBag.TreatiseID = model.TreatiseID;
            var eventid = model.TreatiseProcessEvents.Select(d => d.EventID).ToList();
            ViewBag.ActionList = _event.GetIdentityActionList(eventid);
            return View(model);
        }
        //[HttpPost, ValidateInput(false)]
        //public ActionResult Edit(int id, TreatiseProcess model)
        //{
        //    model = _process.GetModel(id);
        //    if (model == null)
        //        return Json(new RootViewModel() { Message = "参数错误，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
        //    UpdateModel(model);
        //    _process.SaveOrEdit(model);
        //    return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(int id, TreatiseProcess model, int[] eventid)
        {
            model = _process.GetModel(id);
            if (model == null) return View("404");
            UpdateModel(model);
            _event.Delete(d => d.ProcessID == id);
            model.TreatiseProcessEvents = _event.GetProcessEventList(model.ID, eventid);
            _process.SaveOrEdit(model);

            return RedirectToAction("Index");
        }
        public ActionResult Detail(int id)
        {
            var model = _process.GetModel(id);
            if (model == null) return PartialView("404");
            var eventid = model.TreatiseProcessEvents.Select(d => d.EventID).ToList();
            ViewBag.ActionList = _event.GetIdentityActionList(eventid);
            return PartialView(model);
        }
        public ActionResult Delete(int? id)
        {
            var msg = "OK";
            var model = _process.GetModel(id ?? 0);
            if (model == null)
                return Json("未找到对应流程，无法删除！请刷新后重试！", JsonRequestBehavior.AllowGet);
            if (model.TreatiseID != ConfigHelper.TreatiseID)
                return Json("参数错误，无法删除流程！请刷新后重试！", JsonRequestBehavior.AllowGet);
            if (model.Treatise.TreatiseStage != 0)
                return Json("项目已开启或已结束，无法删除流程！", JsonRequestBehavior.AllowGet);
            try
            {
                model.State = 0;
                _process.SaveOrEdit(model);
            }
            catch (Exception ex)
            {
                msg = "删除失败！请确认项目是否已开启！,错误信息：" + ex.Message;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OpenNext(int? id)
        {
            var model = _treatise.GetModel(id ?? 0);
            if (model == null)
                return Json("参数错误，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            if (model.TreatiseStage != 1)
                return Json("项目未开始或已结束，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            if (!model.TreatiseProcesses.Any())
                return Json("尚未配置流程！，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            var list = model.TreatiseProcesses.ToList();
            var max = list.Max(d => d.Sort);
            var nsort = 0;
            var paper = list.FirstOrDefault(d => d.IsOpen == 1);
            if (paper != null)
            {
                var opaper = _process.GetModel(paper.ID);
                opaper.IsOpen = 2;
                _process.SaveOrEdit(opaper);
            }
            else
                paper = new TreatiseProcess() { Sort = 0 };
            nsort = paper.Sort == 0 ? list.Min(d => d.Sort) ?? 0 : paper.Sort ?? 0;
            nsort = nsort + 1 > max ? max ?? 0 : nsort + 1;
            var npaper = list.FirstOrDefault(d => d.Sort == nsort);
            if (npaper == null)
                return Json("参数错误，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            var nnpaper = _process.GetModel(npaper.ID);
            nnpaper.IsOpen = 1;
            _process.SaveOrEdit(nnpaper);
            return Json("成功开启！", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CloseTreatise(int? id)
        {
            var model = _treatise.GetModel(id ?? 0);
            if (model == null)
                return Json("参数错误，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            if (model.TreatiseStage != 1)
                return Json("项目未开始或已结束，无法进行流程操作！", JsonRequestBehavior.AllowGet);
            var str = _process.CloseAll(id ?? 0);
            return Json(str, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 论文流程延期
        /// </summary>
        [HttpPost]
        public JsonResult Delay(int? id, int? day)
        {
            if (day == null || day <= 0)
                return Json(new RootViewModel() { Message = "延期天数必须为大于0的整天数！" }, JsonRequestBehavior.AllowGet);
            var model = _process.Delay(id ?? 0, day ?? 0);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
