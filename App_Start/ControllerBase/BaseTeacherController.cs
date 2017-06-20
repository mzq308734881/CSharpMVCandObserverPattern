using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.ControllerBase
{

    public class BaseTeacherController : BaseController
    {
        /// <summary>
        /// 老师基础信息
        /// </summary>
        protected Teacher Teacher { get { return Info.Teachers.FirstOrDefault(d => d.State != 0); } }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException();
            if (Teacher == null)
                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        }

    }
}
