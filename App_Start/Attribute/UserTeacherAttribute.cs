using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.Attribute
{
    public class UserTeacherAttribute : LoginAttribute
    {
        private Teacher Teacher => Info.Teachers.FirstOrDefault(d=>d.State!=0);

        public UserTeacherAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
                return;
            Passordurl = "/teachersys/user/editpwd";
            base.OnAuthorization(filterContext);
            var pwd = StringHelper.Md5Encrypt(Info.LoginName);
            if (Info != null && Info.Password == pwd)
                filterContext.Result = new RedirectResult(Passordurl);
        }

        public override bool IsLogged()
        {
            if (ConfigHelper.IsDebug)
                return base.IsLogged();
            return Info?.ID != 0  && Teacher?.ID==ConfigHelper.TeacherID;
        }
        
    }
}