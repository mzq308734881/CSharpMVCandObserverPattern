using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LanShanPRMS.Attribute;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using LanShanPRMS.ControllerBase;

namespace CMS.Web.Filters
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        #region 字段和属性
        /// <summary>
        /// 模块别名，可配置更改
        /// </summary>
        public string ModuleAlias { get; set; }
        /// <summary>
        /// 权限动作
        /// </summary>
        public string OperaAction { get; set; }
        /// <summary>
        /// 权限说明
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 是否是Ajax调用
        /// </summary>
        public bool IsAjax { get; set; }
        /// <summary>
        /// 基类实例化
        /// </summary>
        public BaseController baseController = new BaseController();

        #endregion

        /// <summary>
        /// 权限认证
        /// </summary>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (ConfigHelper.UserID == 0)
                filterContext.Result = new ContentResult { Content = string.Format("<script type='text/javascript'>alert('请先登录！');window.location.href='{0}';</script>", FormsAuthentication.LoginUrl) };

            // filterContext.Result = new ContentResult { Content = "<script>alert('抱歉,你不具有当前操作的权限！');history.go(-1)</script>" };
            if (filterContext.IsChildAction)
                return;
            if (string.IsNullOrWhiteSpace(ModuleAlias)) return;
            if (ModuleAlias.ToLower() == "default") return;
            //     filterContext.HttpContext.User
            //1、判断用户是否存在
            //2、判断模块是否对应
            //3、验证是否有访问此页面的权限，查看加操作
            //4、有权限访问页面，将此页面的权限集合传给页面
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                filterContext.Result = AjaxError(filterContext.Exception.Message, filterContext);
            else
                filterContext.Result = ViewError(filterContext.Exception.Message, filterContext);
            //Let the system know that the exception has been handled
            filterContext.ExceptionHandled = true;
        }
        /// <summary>
        /// 返回错误信息（Ajax调用）
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>JsonResult</returns>
        protected JsonResult AjaxError(string message, ExceptionContext filterContext)
        {
            //If message is null or empty, then fill with generic message
            if (String.IsNullOrEmpty(message))
                message = "系统异常，请刷新后重试！";

            //Set the response status code to 500
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Needed for IIS7.0
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new JsonResult
            {
                //can extend more properties 
                Data = new RootViewModel() { Message = message },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
        /// <summary>
        /// 返回错误视图（未完成）
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>JsonResult</returns>
        protected ViewResult ViewError(string message, ExceptionContext filterContext)
        {
            //If message is null or empty, then fill with generic message
            if (String.IsNullOrEmpty(message))
                message = "系统异常，请刷新后重试！";

            //Set the response status code to 500
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Needed for IIS7.0
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new ViewResult() { ViewName = "Error" };
        }
    }
}