using System;
using System.Web.Mvc;

namespace LanShanPRMS.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public bool Ignore { get; set; }
        public AjaxOnlyAttribute(bool ignore = false)
        {
            Ignore = ignore;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return Ignore || controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}
