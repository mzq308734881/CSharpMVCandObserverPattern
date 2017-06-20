using System;
using System.Web.Mvc;
using LanShanPRMS.Common.Log;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;

namespace LanShanPRMS.ControllerBase
{

    public class BaseController : Controller
    {
        protected Log Log = LogFactory.GetLogger("Controller");
        /// <summary>
        /// 是否为Ajax调用
        /// </summary>
        public bool IsAjax { get; set; }

        protected readonly IAdminService _admin = new AdminService();
        /// <summary>
        /// 当前登陆用户基础信息
        /// </summary>
        public UserInfo Info => _admin.LoginInfo();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException();
            IsAjax = filterContext.HttpContext.Request.IsAjaxRequest();
            base.OnActionExecuting(filterContext);
        }
    }
}
