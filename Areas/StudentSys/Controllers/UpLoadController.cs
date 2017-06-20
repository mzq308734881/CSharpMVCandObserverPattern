using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Service;

namespace LanShanPRMS.Areas.StudentSys.Controllers
{
    [UserStudent]
    public class UpLoadController : BaseUpLoadController
    {
        /// <summary>
        /// 上传工具与资源
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Tool(HttpPostedFileBase file)
        {
            return UpLoad("Tool", file);
        }
        /// <summary>
        /// 论文的相关文档
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Word(HttpPostedFileBase file)
        {
            var model = new StudentService().GetModel(ConfigHelper.StudentID);
            return UpLoad("Word", model.UserInfo.LoginName, file);
        }
        public JsonResult Log(HttpPostedFileBase file)
        {
            return UpLoad("Log", file);
        }
    }
}
