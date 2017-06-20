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

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class UpLoadController : BaseUpLoadController
    {
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Excel(HttpPostedFileBase file)
        {
            var model = new StudentService().GetModel(ConfigHelper.StudentID);
            return UpLoad("Excel\\"+ DateTime.Now.ToString("yyyyMMdd"), file);
        }
    }
}
