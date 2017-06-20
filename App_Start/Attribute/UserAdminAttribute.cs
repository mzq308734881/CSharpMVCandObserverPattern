using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;

namespace LanShanPRMS.Attribute
{
    public class UserAdminAttribute : LoginAttribute
    {
        private User User => Info.Users.FirstOrDefault(d => d.State != 0);

        public UserAdminAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
                return;
            Passordurl = "/prms/user/";
            base.OnAuthorization(filterContext);
        }
        public override bool IsLogged()
        {
            if (ConfigHelper.IsDebug)
                return base.IsLogged();
            return Info?.ID != 0 && User?.ID == ConfigHelper.UserID;
        }
    }
}