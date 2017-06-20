using System;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Service;
using System.Web.Mvc;
using LanShanPRMS.Common.Log;
using LanShanPRMS.Model.Models;
using LanShanPRMS.ViewModel;
using System.Linq;

namespace LanShanPRMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService _login = new LoginService();
        private readonly IEmailLogService _log = new EmailLogService();
        private readonly IUserService _user = new UserService();
        private readonly IAdminService _admin = new AdminService();
        private readonly IUserInfoService _info = new UserInfoService();
        private readonly IReviewTeacherService _review = new ReviewTeacherService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        #region 权限
        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IFormsAuthentication formsAuth)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
        }
        #endregion

        public ActionResult Index()
        {
            //CookieHelper.Del("TreatiseID");
            //CookieHelper.Del("StudentID");
            //CookieHelper.Del("UserID");
            //CookieHelper.Del("TeacherID");
            CookieHelper.Del("CheckCode");
            var nameuser = CookieHelper.GetValue("eumsaenr");
            var password = CookieHelper.GetValue("dpraosws");
            var roleselect = CookieHelper.GetValue("trcoellees");
            if (nameuser != null && password != null)
            {
                ViewBag.name = nameuser;
                ViewBag.pwd = password;
                ViewBag.role = roleselect;
            }
            return PartialView();
        }

        public void VerifyCode()
        {
            VerifyCode v = new VerifyCode(5);
        }
        public ActionResult Reg()
        {
            var log = new EmailLog()
            {
                Title = "测试邮件",
                Comment = "测试邮件内容",
                Recipient = "guheshiwei@163.com",
                CreateUser = "定时邮件"
            };
            var logs = LogFactory.GetLogger("home");
            throw new Exception("这是个错误信息");
            _log.FeedBack(log);
            return Json(new RootViewModel() { State = 1, Message = "发送成功" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FindPassword()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Login(string username, string password, string code)
        {
            var ccode = CookieHelper.GetValue("CheckCode");
            if (ccode != code)
                return Json(new LoginViewModel(0, "验证码输入不正确！"));
            var model = _login.Login(username, password);
            if (model.State == 0)
                return Json(model, JsonRequestBehavior.AllowGet);
            ConfigHelper.SetInfoID(model.InfoID);
            switch (model.UserType)
            {
                case 1:
                    ConfigHelper.SetUserID(model.UserID);
                    break;
                case 2:
                    ConfigHelper.SetTeacherID(model.TeacherID);
                    break;
                case 3:
                    ConfigHelper.SetStudentID(model.StudentID);
                    break;
                default:
                    if (model.State == 1)
                    {
                        model.State = 0;
                        model.Message = "用户信息存在错误，请联系管理员修正！";
                    }
                    break;
            }
            CookieHelper.Del("CheckCode");
            FormsAuth.SignIn(username, false);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            CookieHelper.Del("CheckCode");
            CookieHelper.Del("StudentID");
            CookieHelper.Del("TeacherID");
            CookieHelper.Del("UserID");
            CookieHelper.Del("TreatiseID");
            FormsAuth.SignOut();
            return RedirectToAction("index");
        }

        public ActionResult Reset(string username, string code, string password)
        {
            string sessioncode = CookieHelper.GetValue("CheckCode");
            var model = _login.ResetPwd(username, password, code, sessioncode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Email(string username, string email, int roleselect)
        {
            string sessioncode = CookieHelper.GetValue("CheckCode");
            var model = _login.Email(username, email, sessioncode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ForgetPwd()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ForgetPwd(string username, string email)
        {
            var model = _info.Froget(username, email);
            var log = new EmailLog()
            {
                Title = "找回密码",
                Recipient = "guheshiwei@163.com",
            };
            _log.SendEmail(log);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BiuBiu()
        {
            return View();
        }

        public JsonResult Demo()
        {
            var module = new ModuleActionService();
            foreach (var item in module.GetAllList().OrderBy(d => d.ModuleID).Select(d => d.ID).ToList())
            {
                var model = module.GetModel(item);
                module.SaveOrEdit(model);
            }
            return Json("module", JsonRequestBehavior.AllowGet);
        }

        public JsonResult YouDao()
        {
            var youdao = new TranslateYouDaoService();
            var model = youdao.Translate("有道翻译");
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        TreatiseProcessEventService _event = new TreatiseProcessEventService();
        TreatiseProcessService _process = new TreatiseProcessService();
        public JsonResult Review()
        {
            var list = _event.GetAllList().Select(d => d.ID).ToArray();
            var prolist = _process.GetAllList().Select(d => d.ID).ToList();
            _event.Delete(d => d.ID > 0);
            foreach (var item in prolist)
            {
                var pro = _process.GetModel(item);

                pro.TreatiseProcessEvents = _event.GetProcessEventList(pro.ID, list);
                _process.SaveOrEdit(pro);
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
    }
}
