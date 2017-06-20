using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Log;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class TreatiseProcessController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IStudentService _student = new StudentService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();
        private readonly IStudentProcessService _studentProcess = new StudentProcessService();
        private readonly IAnnexService _annex = new AnnexService();
        private readonly IStudentProcessLogService _stuLog = new StudentProcessLogService();
        private Log _log { get; set; }
        public TreatiseProcessController()
        {
            _log = LogFactory.GetLogger("student");
        }
        public ActionResult Search(int id)
        {
            CookieHelper.Del("DocPath");
            var process = _process.GetModel(id);
            if (process == null || process.TreatiseID != ConfigHelper.TreatiseID) return View("404");
            var treastu = _tStudent.GetModel(process.TreatiseID ?? 0, ConfigHelper.StudentID);
            if (treastu == null || treastu.CheckState != 3)
                throw new Exception("您未参加此论文或选题还未通过！");
            //return View("404");
            var stupross = _studentProcess.GetModel(process.ID, Student.ID);
            var model = new StudentProcessViewModel() { Process = process, StudentProcess = stupross };
            var stage = _studentProcess.CheckProcessIsOpen(process.Treatise, process, ConfigHelper.StudentID);
            if (stage == ProcessStage.Starting)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        Server.Execute(WordHelper.Read(Url.Action("SaveDoc", new { sid = Student.ID, tid = process.TreatiseID, pid = process.ID }), Student.UserInfo.TrueName), htw, false);
                        model.Word = sb.ToString();
                    }
                }
                //if (ConfigHelper.IsDebug)                
                //    return View("Starting2", model);                
            }
            return View(stage.ToString(), model);
        }

        public void SaveDoc(int sid, int tid, int pid)
        {
            var student = _student.GetModel(sid);
            var trea = _treatise.GetModel(tid);
            var treastu = _tStudent.GetModel(tid, sid);
            var pross = _process.GetModel(pid);
            var stupross = _studentProcess.GetModel(pid, sid) ?? new StudentProcess()
            {
                EmailAlert = 0,
                ProcessID = pid,
                IsOnTime = 0,
                StudentID = treastu.StudentID,
                TeacherID = treastu.TeacherID,
                TreatiseStudentID = treastu.ID,
            };
            stupross.SubmitTime = DateTime.Now;

            _studentProcess.SaveOrEdit(stupross);
            var studentname = treastu?.Student?.UserInfo?.LoginName ?? ("学生编号" + sid);
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
                var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
                _annex.SaveOrEdit(annex);
                var model = new StudentProcessLog();
                model.Comment = filename;
                model.StudentProcessID = stupross.ID;
                model.ID = 0;
                model.IsBest = 0;
                model.CheckState = 0;
                model.AnnexID = annex.ID;

                _stuLog.SaveOrEdit(model);
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
            //TODO


        }
        public ActionResult SaveLog(int id, StudentProcessLog model, string filename, string filepath)
        {
            var pross = _process.GetModel(id);
            if (pross == null) return View("404");
            var stupross = _studentProcess.GetModel(pross.ID, Student.ID);
            var treastu = _tStudent.GetModel(ConfigHelper.TreatiseID, ConfigHelper.StudentID);
            if (treastu == null) return View("404");
            if (stupross == null)
            {
                stupross = new StudentProcess()
                {
                    EmailAlert = 0,
                    ProcessID = id,
                    IsOnTime = 0,
                    StudentID = treastu.StudentID,
                    TeacherID = treastu.TeacherID,
                    TreatiseStudentID = treastu.ID,
                };
                _studentProcess.SaveOrEdit(stupross);
            }
            var annex = new Annex() { AnnexName = filename, AnnexPath = filepath };
            _annex.SaveOrEdit(annex);
            model.StudentProcessID = stupross.ID;
            model.ID = 0;
            model.IsBest = 0;
            model.CheckState = 0;
            model.AnnexID = annex.ID;
            _stuLog.SaveOrEdit(model);
            return RedirectToAction("Search", new { id = pross.ID });
        }
        public ActionResult Subject(int? id)
        {
            var model = _tStudent.GetModel(id ?? 0);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/studentsys/treatise'</script>");
            return View(model);
        }
        public ActionResult Submit(int id)
        {
            var model = _stuLog.GetModel(id);
            if (model == null)
                return View("404");
            model.IsBest = 1;
            _stuLog.SaveOrEdit(model);
            return RedirectToAction("Search", new { id = model.StudentProcess.ProcessID });
        }

        public ActionResult Delete(int id)
        {
            return Json(_stuLog.Delete(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail(int id)
        {
            var log = _stuLog.GetModel(id) ?? new StudentProcessLog();
            var model = new StudentProcessViewModel()
            {
                ProcessLog = log,
                Annex = _annex.GetModel(log.AnnexID ?? 0),
                CheckAnnex = _annex.GetModel(log.CheckAnnexID ?? 0),
            };
            return View(model);
        }
    }
}
