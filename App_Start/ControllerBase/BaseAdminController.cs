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

    public class BaseAdminController : BaseController
    {
        /// <summary>
        /// 管理员基础信息
        /// </summary>
        public User Admin => Info.Users.FirstOrDefault(d => d.State != 0);

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (filterContext == null) throw new ArgumentNullException();
        //    filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        //    if (Admin == null)
        //        filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        //}

    }
}
