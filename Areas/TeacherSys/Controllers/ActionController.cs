using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class ActionController : BaseTeacherController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly IStudentProcessLogService _studentLog = new StudentProcessLogService();
        private readonly IAnnexService _annex = new AnnexService();
        private readonly IStudentProcessReviewService _review = new StudentProcessReviewService();
        public ActionResult Edit(int? id)
        {
            var log = _studentLog.GetModel(id ?? 0);

            var treastu = log.StudentProcess.TreatiseStudent;
            var student = treastu.Student;
            var model = new StudentProcessViewModel()
            {
                ProcessLog = log,
                TreatiseStudent = treastu,
                TreatiseSubject = treastu.TreatiseSubject ?? new TreatiseSubject(),
                UserInfo = student.UserInfo ?? new UserInfo(),
                Annex = _annex.GetModel(log.AnnexID ?? 0),
                CheckAnnex = _annex.GetModel(log.CheckAnnexID ?? 0),
            };
            return View(model);
        }
        public ActionResult Change(int id)
        {
            var model = _treatise.GetModel(id);
            if (model == null)
                return Json(new RootViewModel() { Message = "无法找到对应项目，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
            ConfigHelper.SetTreatiseID(model.ID);
            return Json(new RootViewModel() { State = 1, Message = "等待页面重新载入……" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int? id)
        {
            var model = _sProcess.GetModel(id ?? 0);
            return model != null ? View(model) : View("404");
        }
        public ActionResult Check(int id)
        {
            var log = _studentLog.GetModel(id);
            if (log == null) return View("404");
            var treastu = log.StudentProcess?.TreatiseStudent ?? new TreatiseStudent();
            var student = treastu.Student;
            var model = new StudentProcessViewModel()
            {
                Process = log?.StudentProcess?.TreatiseProcess ?? new TreatiseProcess(),
                ProcessLog = log,
                TreatiseStudent = treastu,
                TreatiseSubject = treastu.TreatiseSubject ?? new TreatiseSubject(),
                UserInfo = student.UserInfo ?? new UserInfo(),
                Annex = _annex.GetModel(log.AnnexID ?? 0),
                CheckAnnex = _annex.GetModel(log.CheckAnnexID ?? 0),
            };
            return View(model);
        }

        public ActionResult Review(int? id)
        {
            var log = _studentLog.GetModel(id ?? 0);

            var treastu = log.StudentProcess.TreatiseStudent;
            var student = treastu.Student;
            var model = new StudentProcessViewModel()
            {
                ProcessLog = log,
                TreatiseStudent = treastu,
                TreatiseSubject = treastu.TreatiseSubject ?? new TreatiseSubject(),
                UserInfo = student.UserInfo ?? new UserInfo(),
                Annex = _annex.GetModel(log.AnnexID ?? 0),
                CheckAnnex = _annex.GetModel(log.CheckAnnexID ?? 0),
            };
            if (log.CheckState != 0)
                return View("Reviewed", model);
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    Server.Execute(WordHelper.Read(Url.Action("SaveReview", new { id = log.ID }), student.UserInfo.TrueName, model.Annex?.AnnexPath), htw, false);
                    model.Word = sb.ToString();
                }
            }
            ViewBag.CheckState = SelectListHelper.StudentProcessLogCheckStateList(log.CheckState);
            return log.CheckState == 0 ? View(model) : View("Reviewed", model);
        }
        public void SaveReview(int? id)
        {
            var model = _studentLog.GetModel(id ?? 0);
            var studentname = model?.StudentProcess?.TreatiseStudent?.Student?.UserInfo?.LoginName ?? ("学生编号" + id);
            var dirpath = "/UpFiles/Word/" + studentname + "/";
            var fs = new PageOffice.FileSaver();
            try
            {
                var reviewlist = _review.GetList(d => d.LogID == id && d.EventID == 12 && d.CheckInfoID == Teacher.InfoID);
                if (reviewlist.Any())
                {
                    fs.CustomSaveResult = "已审阅流程记录无法再次审阅！！";
                    fs.Close();
                    return;
                }
                var filename = fs.LocalFileName ?? fs.FileName;
                var filepath = dirpath + DateTime.Now.ToString("yyyyMMddHHssmm") + ".docx";
                if (!Directory.Exists(ConfigHelper.ServerPath + dirpath))
                    Directory.CreateDirectory(ConfigHelper.ServerPath + dirpath);
                fs.SaveToFile(ConfigHelper.ServerPath + filepath);
                var comment = fs.GetFormField("CheckComment");
                var checkstate = StringHelper.StringToZero(fs.GetFormField("CheckState"));
                if (model == null)
                {
                    fs.CustomSaveResult = "参数错误,请刷新后重试!";
                    fs.Close();
                    return;
                }
                if (checkstate == 0 || string.IsNullOrWhiteSpace(comment))
                {
                    fs.CustomSaveResult = "请填写下方评阅意见及评阅结果!";
                    fs.Close();
                    return;
                }
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                model.StudentProcessReviews.Add(
                    new StudentProcessReview()
                    {
                        CheckState = checkstate,
                        CheckUserType = (int)ProcessIdentity.Adviser,
                        CheckAnnexID = annex.ID,
                        CheckInfoID = Teacher.InfoID
                    }
                );
                _studentLog.SaveOrEdit(model);
                fs.CustomSaveResult = "OK";
            }
            catch (Exception ex)
            {
                var filePath = ConfigHelper.ServerPath + "/UpFiles/Temp/";
                if (!System.IO.Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                fs.SaveToFile(filePath + fs.LocalFileName ?? fs.FileName ?? "123.doc");
                fs.CustomSaveResult = "文件保存出错！请尝试重新保存或联系管理员！";
                LogHelper.LogError("导师在线编辑", ex);
                Log.Error(ex);
            }
            fs.Close();
        }
        public ActionResult Question(int? id)
        {
            var log = _studentLog.GetModel(id ?? 0);
            if (log == null) return View("404");
            return View(log);
        }
        [HttpPost]
        public ActionResult Question(int? id, StudentProcessLog model)
        {
            return RedirectToAction("check", new {id });
        }
        public void SaveQuestion(int? id)
        {
            var model = _studentLog.GetModel(id ?? 0);
            var studentname = model?.StudentProcess?.TreatiseStudent?.Student?.UserInfo?.LoginName ?? ("学生编号" + id);
            var dirpath = "/UpFiles/Word/" + studentname + "/";
            var fs = new PageOffice.FileSaver();
            try
            {
                var filename = fs.LocalFileName ?? fs.FileName;
                var filepath = dirpath + DateTime.Now.ToString("yyyyMMddHHssmm") + ".docx";
                if (!Directory.Exists(ConfigHelper.ServerPath + dirpath))
                    Directory.CreateDirectory(ConfigHelper.ServerPath + dirpath);
                fs.SaveToFile(ConfigHelper.ServerPath + filepath);
                var comment = fs.GetFormField("CheckComment");
                var checkstate = StringHelper.StringToZero(fs.GetFormField("CheckState"));
                if (model == null)
                {
                    fs.CustomSaveResult = "参数错误,请刷新后重试!";
                    fs.Close();
                    return;
                }
                if (checkstate == 0 || string.IsNullOrWhiteSpace(comment))
                {
                    fs.CustomSaveResult = "请填写下方评阅意见及评阅结果!";
                    fs.Close();
                    return;
                }
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                model.CheckState = checkstate;
                model.CheckComment = comment;
                model.CheckAnnexID = annex.ID;
                _studentLog.SaveOrEdit(model);
                fs.CustomSaveResult = "OK";
            }
            catch (Exception ex)
            {
                var filePath = ConfigHelper.ServerPath + "/UpFiles/Temp/";
                if (!System.IO.Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                fs.SaveToFile(filePath + fs.LocalFileName ?? fs.FileName ?? "123.doc");
                fs.CustomSaveResult = "文件保存出错！请尝试重新保存或联系管理员！";
                LogHelper.LogError("导师在线编辑", ex);
                Log.Error(ex);
            }
            fs.Close();
        }
    }
}