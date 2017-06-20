using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Common.Log;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Common.Email;

namespace LanShanPRMS.Attribute
{
    public class ErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            WriteLog(context);
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult()
                {
                    Data = new JsonErrorViewModel() {Message = context.Exception.Message},
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }
            else
            {
                context.Result = new ViewResult() { ViewName = "404" };
            }
                
        }
        private void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            var log = LogFactory.GetLogger(context.Controller.ToString());
            var ex = context.Exception;
            log.Error(ex);
            LogHelper.LogError(context.Controller.ToString(), ex);
        }
    }
}