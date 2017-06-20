using System;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using LanShanPRMS.Common.Email;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Common.Log;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Attribute
{
    public class DebugAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            WriteLog(context);
        }
        private static void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            var log = LogFactory.GetLogger(context.Controller.ToString());
            var exce = context.Exception;
            log.Error(exce);
            LogHelper.LogError(context.Controller.ToString(), exce);
            //try
            //{
            //    Email.From(LanShanEmail.From)
            //        .To(LanShanEmail.To)
            //        .Body(exce.StackTrace + exce.Message + exce.TargetSite)
            //        .Subject(exce.Message)
            //        .UsingClient(LanShanEmail.Client).Send();
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex);
            //    LogHelper.LogError(context.Controller.ToString(), ex);
            //}
        }
    }
}