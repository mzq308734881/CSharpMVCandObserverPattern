using System.Text;
using System.Web.Mvc;

namespace LanShanPRMS.Attribute
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public bool Ignore { get; set; }
        public AuthorizeAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Ignore == false)
                return;

            if (this.ActionAuthorize(filterContext))
                return;
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>alert('很抱歉！您的权限不足，访问被拒绝！');</script>");
            filterContext.Result = new ContentResult() { Content = sbScript.ToString() };
        }
        private bool ActionAuthorize(ActionExecutingContext filterContext)
        {
            //var operatorProvider = OperatorProvider.Provider.GetCurrent();
            //var roleId = operatorProvider.RoleId;
            //var moduleId = WebHelper.GetCookie("nfine_currentmoduleid");
            //var action = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString();
            //return new RoleAuthorizeApp().ActionValidate(roleId, moduleId, action);
            return true;
        }
    }
}