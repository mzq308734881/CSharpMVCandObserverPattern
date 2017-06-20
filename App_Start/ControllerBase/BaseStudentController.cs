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

    public class BaseStudentController : BaseController
    {
        /// <summary>
        /// 学生基础信息
        /// </summary>
        protected Student Student { get { return Info.Students.FirstOrDefault(d => d.State != 0); } }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException();
            if (Student == null)
                filterContext.Result = new RedirectResult(FormsAuthentication.LoginUrl);
        }

    }
}
