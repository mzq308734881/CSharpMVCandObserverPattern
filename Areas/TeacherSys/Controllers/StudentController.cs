using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;
using LanShanPRMS.IService;
using System.Text;
using System.IO;
using System.Web.UI;
using LanShanPRMS.Common.Log;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
    public class StudentController : BaseTeacherController
    {
        //
        // GET: /PRMS/Student/
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly IStudentProcessService _sProcess = new StudentProcessService();
        private readonly IStudentProcessLogService _studentLog = new StudentProcessLogService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly ITeacherService _teacher = new TeacherService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IAnnexService _annex = new AnnexService();
        private Log _log { get; set; }
        public StudentController()
        {
            _log = LogFactory.GetLogger("student");
        }

        public ActionResult Index()
        {
            var list = _tStudent.GetTreatiseTeacherPass(ConfigHelper.TreatiseID, ConfigHelper.TeacherID);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            return View(new TreatiseTeacherViewModel(trea, ConfigHelper.TeacherID) { ProcessList = trea.TreatiseProcesses.ToList(), TeacherStudent = list });
        }
        public ActionResult Search(string keywords)
        {
            var keyword = StringHelper.ClearFormat(keywords);
            var list = _tStudent.GetTreatiseTeacherPass(ConfigHelper.TreatiseID, ConfigHelper.TeacherID);
            if (!string.IsNullOrEmpty(keyword))
                list = from c in list where c.Student.UserInfo.LoginName.Contains(keyword) || c.Student.UserInfo.TrueName.Contains(keyword) select c;
            CookieHelper.SetObj("keyword", keyword);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            return View(new TreatiseTeacherViewModel(trea, ConfigHelper.TeacherID) { ProcessList = trea.TreatiseProcesses.ToList(), TeacherStudent = list });
        }
        public ActionResult Detail(int id)
        {
            var model = _sProcess.GetModel(id);
            return View(model);
        }
        public ActionResult Check(int id)
        {
            var log = _studentLog.GetModel(id);

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
                return View("Checked", model);
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    Server.Execute(WordHelper.Read(Url.Action("SaveDoc", new { id = log.ID }), student.UserInfo.TrueName, model.Annex?.AnnexPath), htw, false);
                    model.Word = sb.ToString();
                }
            }
            ViewBag.CheckState = SelectListHelper.StudentProcessLogCheckStateList(log.CheckState);
            return log.CheckState == 0 ? View(model) : View("Checked", model);
        }

        public void SaveDoc(int? id)
        {
            var model = _studentLog.GetModel(id ?? 0);
            var studentname = model?.StudentProcess?.TreatiseStudent?.Student?.UserInfo?.LoginName ?? ("学生编号" + id);
            var dirpath = "/UpFiles/Word/" + studentname + "/";
            var fs = new PageOffice.FileSaver();
            try
            {
                var filename = fs.LocalFileName ?? fs.FileName;
                var filepath = dirpath + DateTime.Now.ToString("yyyyMMddHHssmm") + ".docx";
                var temppath = ConfigHelper.ServerPath + "/UpFiles/Temp/" + fs.LocalFileName ?? fs.FileName ?? "123.doc";
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
                _log.Error(ex);
            }
            fs.Close();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Check(int id, StudentProcessLog log, string filename, string filepath)
        {
            log = _studentLog.GetModel(id);
            if (log == null) return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            UpdateModel(log);
            var annex = new Annex() { AnnexPath = filepath, AnnexName = filename };
            _annex.SaveOrEdit(annex);
            log.CheckAnnexID = annex.ID;
            log.CheckTime = DateTime.Now;
            log.CheckInfoID = ConfigHelper.TeacherID;
            _studentLog.SaveOrEdit(log);
            return RedirectToAction("Detail", new { id = log.StudentProcessID });
        }

        public ActionResult Point(int id, StudentProcess model)
        {
            model = _sProcess.GetModel(id);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            UpdateModel(model);
            model.ScoreTime = DateTime.Now;
            model.TimeMark = model.TimeMark > model.TreatiseProcess.TimeMark
                ? model.TreatiseProcess.TimeMark
                : model.TimeMark;
            model.QualityMark = model.QualityMark > model.TreatiseProcess.QualityMark
                ? model.TreatiseProcess.QualityMark
                : model.QualityMark;
            model.TotalMark = model.TimeMark + model.QualityMark;
            model.IsOnTime = model.TreatiseProcess.EndTime > DateTime.Now ? 1 : 0;
            var check = model.StudentProcessLogs.FirstOrDefault(d => d.CheckState == 1);
            model.SubmitTime = check != null ? check.CreateTime : DateTime.Now;
            _sProcess.SaveOrEdit(model);
            return RedirectToAction("Detail", new { id = model.ID });
        }
        
    }
}
