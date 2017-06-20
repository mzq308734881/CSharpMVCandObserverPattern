using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;

namespace LanShanPRMS.Attribute
{
    public class UserStudentAttribute : LoginAttribute
    {
        private Student Student => Info.Students.FirstOrDefault(d => d.State != 0);

        public UserStudentAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
                return;
            Passordurl = "/studentsys/user/editpwd";
            base.OnAuthorization(filterContext);
            var pwd = StringHelper.Md5Encrypt(Info.LoginName);
            if (Info != null && Info.Password == pwd)
                filterContext.Result = new RedirectResult(Passordurl);
        }
        public override bool IsLogged()
        {
            if (ConfigHelper.IsDebug)
                return base.IsLogged();
            return Info?.ID != 0 && Student?.ID != 0 && Student?.ID == ConfigHelper.StudentID;
        }
    }
}