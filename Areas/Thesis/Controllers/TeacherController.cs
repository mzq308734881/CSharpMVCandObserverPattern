using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class TeacherController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        private readonly ITeacherService _teacher = new TeacherService();

        public ActionResult Search(int? page)
        {
            var id = ConfigHelper.TreatiseID;
            var trachername = "";
            var trea1 = _treatise.GetModel(id);
            if (page == null || page < 1)
            {
                page = 1;
                CookieHelper.Del("trachername");
            }
            else
                trachername = StringHelper.ClearFormat(CookieHelper.GetValue("trachername"));
            var sum = trea1.TreatiseStudents.Count;
            var allotsum = _tTeacher.GetTreatiseList(id).Sum(d => d.StudentSum) ?? 0;
            var model = _treatise.GetModel(id);
            if (model == null) return View("404");
            var list = _tTeacher.GetTreatiseList(model.ID);
            if (!string.IsNullOrWhiteSpace(trachername))
                list = from c in list where c.Teacher.UserInfo.TrueName.Contains(trachername) select c;
            list = list.OrderByDescending(d => d.Teacher.UserInfo.ID);
            var trea = new TreatiseConfigViewModel(model)
            {
                //学生总数
                ListCount = sum,
                //已分配人数
                SumCount = allotsum,
                TeacherPage = _tTeacher.GetPageList(list, page ?? 1),
                UnCount = sum - allotsum
            };
            return View(trea);
        }
        [HttpPost]
        public ActionResult Search(string trachername)
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            var list = _tTeacher.GetTreatiseList(model.ID);

            if (!string.IsNullOrWhiteSpace(trachername))
            {
                CookieHelper.SetObj("trachername", trachername);
                list = from c in list where c.Teacher.UserInfo.TrueName.Contains(trachername) select c;
            }
            else
                CookieHelper.Del("trachername");
            list = list.OrderByDescending(d => d.Teacher.UserInfo.ID);
            var trea = new TreatiseConfigViewModel(model)
            {
                TeacherPage = _tTeacher.GetPageList(list)
            };
            return View("search", trea);
        }
        public ActionResult Add()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (trea == null) return View("404");
            var model = new TreatiseTeacher() { TreatiseID = ConfigHelper.TreatiseID };
            var list = from c in _teacher.GetAllList() where c.TreatiseTeachers.All(d => d.TreatiseID != ConfigHelper.TreatiseID)&&c.Subjects.Any() select c;
            ViewBag.TeacherID = list.ToSelectList(d => d.UserInfo.TrueName + " - " + d.UserInfo.LoginName, d => d.ID.ToString(), "请选择"); ;
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(int? id, TreatiseTeacher model)
        {
            var trea = _treatise.GetModel(id ?? 0);
            if (trea == null) return View("404");
            model.ID = 0;
            //id是项目id
            model.TreatiseID = id;
            _tTeacher.SaveOrEdit(model);
            return RedirectToAction("search");
        }
        public ActionResult Edit(int? id)
        {
            var model = _tTeacher.GetModel(id ?? 0);
            if (model == null) return Content("参数错误");
            var user = _admin.LoginUser();
            if (user == null) return Content("用户不存在");
            var teachers = _teacher.GetAllList();
            if (!_admin.IsSuperAdmin())
                teachers = _teacher.GetSchoolTeacher(user.SchoolID ?? 0);
            ViewBag.TeacherID = teachers.ToSelectList(d => d.UserInfo.TrueName, d => d.ID.ToString(), model.TeacherID);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int? id, TreatiseTeacher model)
        {
            model = _tTeacher.GetModel(id ?? 0);
            if (model == null) return Content("参数错误");
            UpdateModel(model);
            _tTeacher.SaveOrEdit(model);
            return RedirectToAction("search");
        }

        public ActionResult Detail(int? id)
        {
            var model = _tTeacher.GetModel(id ?? 0);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = "OK";
            var model = _tTeacher.GetModel(id);
            if (model.Treatise.TreatiseStage != 0)
                return Json("项目已开启或已结束，无法删除流程！", JsonRequestBehavior.AllowGet);
            try
            {
                _tTeacher.Delete(id);
            }
            catch (Exception ex)
            {
                msg = "删除失败！请确认项目是否已开启！,错误信息：" + ex.Message;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #region  导师导入
        public ActionResult Import()
        {
            var id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id);
            CookieHelper.Del("FileUrl");
            var sum = trea.TreatiseStudents.Count;
            var allotsum = _tTeacher.GetTreatiseList(id).Sum(d => d.StudentSum) ?? 0;
            return trea == null ? View("404") : View(new TreatiseConfigViewModel(trea)
            {
                SumCount = sum,
                UnCount = sum - allotsum
            });
        }

        [HttpPost]
        public ActionResult Import(string filepath)
        {
            var id = ConfigHelper.TreatiseID;
            var treatise = _treatise.GetModel(id);
            if (treatise == null)
                return Content("<script>alert('参数错误！请返回后重试！');</script>");
            if (string.IsNullOrWhiteSpace(filepath))
                return Content("<script>alert('文件上传失败！请刷新后重试！');</script>");
            //Server.MapPath链接的地址。list里面放了个id和链接地址
            var list = GetList(id, ConfigHelper.ToServerPath(filepath));
            CookieHelper.SetObj("FileUrl", filepath);
            ViewBag.Treatise = treatise;
            ViewBag.TreatiseID = id;
            var importsum = list.Sum(d => d.StudentCount);
            var allotsum = _tTeacher.GetTreatiseList(id).Sum(d => d.StudentSum) ?? 0;
            var sum = treatise.TreatiseStudents.Count;
            var model = new TreatiseConfigViewModel(treatise)
            {
                Imports = list,
                SumCount = sum,
                UnCount = sum - allotsum,
                ListCount = importsum
            };

            if (list.Any(d => d.State == 0))
            {
                model.Message = "部分数据不可导！";
                return View(model);
            }
            if (sum < importsum + allotsum)
            {
                model.Message = "导师分配的学生数大于项目总学生数！";
                return View(model);
            }
            model.State = 1;
            return View(model);
        }
        //导入数据库
        public JsonResult ImportDB()
        {
            var id = ConfigHelper.TreatiseID;
            var excelpath = CookieHelper.GetValue("FileUrl");
            var conn = ConfigHelper.ToServerPath(excelpath);
            var list = GetList(id, conn);
            var treatise = _treatise.GetModel(id);
            if (treatise == null)
                return Json(new RootViewModel() { Message = "参数错误！请返回后重试！" }, JsonRequestBehavior.AllowGet);
            try
            {
                foreach (var item in list.Where(d => d.State == 1))
                {
                    var tTeacher = new TreatiseTeacher()
                    {
                        TreatiseID = id,
                        TeacherID = item.Teacher.ID,
                        StudentSum = item.StudentCount
                    };
                    _tTeacher.SaveOrEdit(tTeacher);
                }
                CookieHelper.Del("FileUrl");
                return Json(new RootViewModel() { State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new RootViewModel() { Message = "数据导入出错，错误代码：" + ex.Message + "！" }, JsonRequestBehavior.AllowGet);
            }
        }
        ///获取excel里面的数据并转化成对应的List       
        public List<ImportViewModel> GetList(int id, string conn)
        {
            var list = new List<ImportViewModel>();
            var ds = VerificationViewModel.ExcelToDataSet(conn);
            var teachers = (from c in _tTeacher.GetTreatiseList(id) select c.TeacherID).ToList();
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var dr = ds.Tables[0].Rows[i];
                var model = GetModel(dr);
                if (model != null)
                {
                    model.ID = i + 1;
                    if (model.State != 0)
                    {
                        if (list.Count(d => d.Teacher.ID == model.Teacher.ID) > 1)
                        { model.State = 0; model.Message = "存在重复导师"; }
                        if (teachers.Contains(model.Teacher.ID))
                        { model.State = 0; model.Message = "已被导入当前项目"; }
                    }
                    list.Add(model);
                }

            }
            return list;
        }
        ///获取excel一行的记录转化成实体
        public ImportViewModel GetModel(DataRow dr)
        {
            if (dr == null)
                return null;

            var logname = dr[0].ToString();
            if (string.IsNullOrEmpty(logname))
                return new ImportViewModel() { Message = "当前行导师信息为空为空，不会被导入", State = 2 };

            var teacher = _teacher.GetModelByLogName(logname);
            if (teacher == null)
                return new ImportViewModel() { Message = "未找到当前导师" };

            var count = StringHelper.StringToZero(dr[1].ToString());
            if (count == 0)
                return new ImportViewModel() { Message = "学生人数不能为空" };
            return new ImportViewModel() { State = 1, Teacher = teacher, StudentCount = count };
        }
        #endregion
    }
}
