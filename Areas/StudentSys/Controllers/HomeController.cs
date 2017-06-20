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
    public class HomeController : BaseStudentController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly INoticeService _notice = new NoticeService();
        private readonly IEmailLogService _log = new EmailLogService();

        public ActionResult Index()
        {
            var tlist = _treatise.GetStudentTreatises(ConfigHelper.StudentID);
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea != null && tlist.Any(d => d.ID == trea.ID))
                ConfigHelper.SetTreatiseID(trea.ID);
            else
            {
                trea = tlist.FirstOrDefault(d => d.State == 1);
                if (trea != null)
                    ConfigHelper.SetTreatiseID(trea.ID);
            }
            var notice = _notice.GetStudentNoticeCount(ConfigHelper.StudentID);
            var list = _notice.GetListByType(0).Take(5).ToList();
            var model = new TreatiseStudentViewModel(trea) { UnCount = notice, Notices = list };
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
        [HttpPost]
        public ActionResult Email()
        {

            var commnet = Request.Form["commnet"];
            var log = new EmailLog()
            {
                Title = "测试邮件",
                Comment = commnet,
                Recipient = "guheshiwei@163.com",
                CreateUser = "定时邮件"
            };
            _log.SendEmail(log);
            return Content("<script>alert('发送成功！');window.location.href ='/studentsys/home/index'</script>");
        }
        public ActionResult Detail(int ?id)
        {
            var model = _notice.GetModel(id??0);
            return model != null ? View(model) : View("404");
        }
    }
}
