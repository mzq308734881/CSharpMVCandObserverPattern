using System.Web;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Service;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.PRMS.Controllers
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
            return UpLoad("Excel", file);
        }
        /// <summary>
        /// 课题相关资料
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult Subject(int? teacherid, HttpPostedFileBase file)
        {
            var model = new UserInfoService().GetTeacher(teacherid ?? 0);
            if (model.ID == 0 || model.State == 0)
                return Json(new UpLoadViewModel() { Message = "上传失败！未找到对应导师!请重新选择导师后重新上传!" });
            return UpLoad("Subject", model.UserInfo.LoginName, file);
        }
        /// <summary>
        /// 通知公告
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Notice(HttpPostedFileBase file)
        {
            return UpLoad("Notice", file);
        }
        /// <summary>
        /// 上传工具与资源
        /// </summary>
        /// <param name="file"></param>
        public JsonResult Tool(HttpPostedFileBase file)
        {
            return UpLoad("Tool", file);
        }
    }
}
