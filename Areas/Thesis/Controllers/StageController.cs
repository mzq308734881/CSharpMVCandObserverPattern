using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class StageController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        public ActionResult Search()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var isfull = _tStudent.GetTreatiseList(trea.ID).Count(d => d.TreatiseSubject == null);
            return View(new TreatiseViewModel() { Treatise = trea, ProcessList = trea.TreatiseProcesses.ToList(), UnCount = isfull });
        }

        [HttpPost]
        public ActionResult EnterChooseSubejct()
        {
            var model = new RootViewModel();
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null || trea.State == 0)
                return Json(new RootViewModel() { Message = "参数错误！未找到对应论文！" }, JsonRequestBehavior.AllowGet);
            if (trea.TreatiseStage != 0)
                return Json(new RootViewModel() { Message = "该论文不处于论文配置阶段，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);
            var total = trea.TimeGrades + trea.QualityGrades;
            var prototal = trea.TreatiseProcesses.Sum(d => d.QualityMark + d.TimeMark);
            var sum = total - prototal;
            if (sum > 0)
                return Json(new RootViewModel() { Message = "存在未分配分数：" + sum + "分，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);

            sum = trea.TreatiseStudents.Count - (trea.TreatiseTeachers.Sum(d => d.StudentSum) ?? 0);
            if (sum > 0)
                return Json(new RootViewModel() { Message = "存在未分配导师人数：" + sum + "人，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);
            sum = trea.TreatiseStudents.Count - (trea.TreatiseSubjects.Sum(d => d.Total) ?? 0);
            if (sum > 0)
                return Json(new RootViewModel() { Message = "存在未分配课题人数：" + sum + "人，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);
            trea.TreatiseStage = 1;
            _treatise.SaveOrEdit(trea);
            model = new RootViewModel() { State = 1, Message = "论文正式进入选题阶段！" };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EnterStartProcess()
        {
            var model = new RootViewModel();
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null || trea.State == 0)
                return Json(new RootViewModel() { Message = "参数错误！未找到对应论文！" }, JsonRequestBehavior.AllowGet);
            if (trea.TreatiseStage != 1)
                return Json(new RootViewModel() { Message = "该论文不处于选题阶段，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);
            var sum = trea.TreatiseStudents.Count(d => d.CheckState != 3);
            if (sum > 0)
                return Json(new RootViewModel() { Message = "存在未选题学生：" + sum + "人，无法进行阶段变更操作！" }, JsonRequestBehavior.AllowGet);
            trea.TreatiseStage = 2;
            _treatise.SaveOrEdit(trea);
            var pro = trea.TreatiseProcesses.OrderBy(d => d.Sort).FirstOrDefault(d => d.State == 1);
            if (pro != null)
            {
                var promodel = _process.GetModel(pro.ID);
                promodel.IsOpen = 1;
                _process.SaveOrEdit(promodel);
            }
            model = new RootViewModel() { State = 1, Message = "论文正式进入流程进行阶段！" };
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EndTreatise()
        {
            var model = new RootViewModel();

            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null || trea.State == 0)
                return Json(new RootViewModel() { Message = "参数错误！未找到对应论文！" }, JsonRequestBehavior.AllowGet);

            trea.TreatiseStage = 3;
            _treatise.SaveOrEdit(trea);
            model = new RootViewModel() { State = 1, Message = "论文已结束！" };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Change(int id)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea.TreatiseStage + 1 != id)
                return Json(new RootViewModel() { Message = "参数错误，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
            trea.TreatiseStage = id;
            _treatise.SaveOrEdit(trea);
            if (id != 2)
                return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
            var pro = trea.TreatiseProcesses.OrderBy(d => d.Sort).FirstOrDefault(d => d.State == 1);
            if (pro == null)
                return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
            var model = _process.GetModel(pro.ID);
            model.IsOpen = 1;
            _process.SaveOrEdit(model);
            return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Process(int id)
        {
            var model = _process.GetModel(id);
            if (model == null)
                return Json(new RootViewModel() { Message = "参数错误，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
            if (model.Treatise.TreatiseStage != 2)
                return Json(new RootViewModel() { Message = "当前项目尚未进行到流程阶段，无法开启！" }, JsonRequestBehavior.AllowGet);
            switch (model.IsOpen)
            {
                case 0:
                    model.IsOpen = 1;
                    _process.SaveOrEdit(model);
                    break;
                case 1:
                    var next = _process.GetNextProcess(model);
                    model.IsOpen = 2;
                    _process.SaveOrEdit(model);
                    if (next != null)
                    {
                        next.IsOpen = 1;
                        _process.SaveOrEdit(next);
                    }
                    break;
            }
            return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);

        }
    }
}