using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.Attribute
{
    public class LoginAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private readonly IAdminService _admin = new AdminService();
        public string Passordurl = "/";
        public bool Ignore;
        public UserInfo Info => _admin.LoginInfo();

        public LoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
                return;
            base.OnAuthorization(filterContext);
            if (IsLogged()) return;
            filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        }
        public virtual bool IsLogged()
        {
            return Info != null && Info.ID != 0;
        }
    }
}