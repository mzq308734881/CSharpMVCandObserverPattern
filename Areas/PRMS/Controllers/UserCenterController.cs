using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.ViewModel;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class UserCenterController : BaseAdminController
    {
        //
        // GET: /PRMS/User/
        private readonly IUserService _user = new UserService();

        public ActionResult Detail(int? id)
        {
            var model = _user.GetDtoModel(ConfigHelper.UserID);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _user.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public void DeleteCookie()
        {
            CookieHelper.Del("keyword");
            CookieHelper.Del("pageIndex");
        }
        public ActionResult Pwd()
        {
            return View(Admin);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pwd(string oldpwd, string newpwd, string newpwd1)
        {
            var model = _admin.LoginUser();
            if (oldpwd != model.UserInfo.Password)
                return Json(new RootViewModel("新旧密码不匹配"), JsonRequestBehavior.AllowGet);

            if (newpwd != newpwd1)
                return Json(new RootViewModel("重复输入密码错误"), JsonRequestBehavior.AllowGet);

            if (newpwd == oldpwd)
                return Json(new RootViewModel("新旧密码不能相同"), JsonRequestBehavior.AllowGet);

            if (newpwd != newpwd1)
                return Json(new RootViewModel("两次密码不一致"), JsonRequestBehavior.AllowGet);

            model.UserInfo.Password = StringHelper.Md5Encrypt(newpwd);
            _user.SaveOrEdit(model);
            return Json(new RootViewModel() { Message = "修改成功！！", State = 1 }, JsonRequestBehavior.AllowGet);
        }
    }
}
