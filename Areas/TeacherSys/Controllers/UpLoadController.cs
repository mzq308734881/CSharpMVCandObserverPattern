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

namespace LanShanPRMS.Areas.TeacherSys.Controllers
{
    [UserTeacher]
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
        /// 课题相关资料
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult Subject(HttpPostedFileBase file)
        {
            var model = new UserInfoService().GetInfo(ConfigHelper.InfoID);
            return UpLoad("Subject", model.LoginName, file);
        }
        /// <summary>
        /// 论文的相关文档
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Word(HttpPostedFileBase file)
        {
            var model = new UserInfoService().GetInfo(ConfigHelper.InfoID);
            return UpLoad("Word", model.LoginName, file);
        }
    }
}
