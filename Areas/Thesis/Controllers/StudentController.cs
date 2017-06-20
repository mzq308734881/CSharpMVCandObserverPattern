using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Core;
using LanShanPRMS.IService;
using LanShanPRMS.Attribute;
using LanShanPRMS.ControllerBase;

namespace LanShanPRMS.Areas.Thesis.Controllers
{
    [UserAdmin]
    public class StudentController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly IStudentService _student = new StudentService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        public ActionResult Search(int? page)
        {
            var stuno = "";
            var stuname = "";
            if (page == null || page < 1)
            {
                page = 1;
                CookieHelper.Del("stuno");
                CookieHelper.Del("stuname");
            }
            else
            {
                stuno = StringHelper.ClearFormat(CookieHelper.GetValue("stuno"));
                stuname = StringHelper.ClearFormat(CookieHelper.GetValue("stuname"));
            }
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            var list = _tStudent.GetTreatiseList(model.ID);
            if (!string.IsNullOrWhiteSpace(stuno))
                list = from c in list where c.Student.UserInfo.LoginName.Contains(stuno) select c;
            if (!string.IsNullOrWhiteSpace(stuname))
                list = from c in list where c.Student.UserInfo.TrueName.Contains(stuname) select c;
            list = list.OrderByDescending(d => d.Student.UserInfo.ID);
            var trea = new TreatiseConfigViewModel(model)
            {
                StudentPage = _tStudent.GetPageList(list, page ?? 1, 10)
            };
            return model != null ? View(trea) : View("404");
        }
        [HttpPost]
        public ActionResult Search(string stuno, string stuname)
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            var list = _tStudent.GetTreatiseList(model.ID);
            if (!string.IsNullOrWhiteSpace(stuno))
            {
                CookieHelper.SetObj("stuno", stuno);
                list = from c in list where c.Student.UserInfo.LoginName.Contains(stuno) select c;
            }
            else
                CookieHelper.Del("stuno");


            if (!string.IsNullOrWhiteSpace(stuname))
            {
                CookieHelper.SetObj("stuname", stuname);
                list = from c in list where c.Student.UserInfo.TrueName.Contains(stuname) select c;
            }
            else
                CookieHelper.Del("stuname");
            list = list.OrderByDescending(d => d.Student.UserInfo.ID);
            var trea = new TreatiseConfigViewModel(model)
            {
                StudentPage = _tStudent.GetPageList(list, 1, 10)
            };
            return View("search", trea);
        }

        public ActionResult Add()
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/prms/treatiseconfig'</script>");
            if (model.TreatiseStage > 2)
                return Content("<script>alert('项目已开启或已结束，无法再次添加学生！');window.location.href ='/prms/treatiseconfig'</script>");
            var schoolist = _school.GetUserChildSchool(model.SchoolID ?? 0);
            schoolist.Add(model.SchoolID ?? 0);
            //查询项目ID下面的所有学生id并返回一个list集合
            var treastu = _tStudent.GetTreatiseList(model.ID).Select(d => d.StudentID).ToList();
            //查询所有学校id下面但不包括项目id下面已经有的学生
            var StudentList = _student.GetAllList().Where(d => schoolist.Contains(d.SchoolID ?? 0) && !treastu.Contains(d.ID));
            var vm = new TreatiseConfigViewModel(model) { Students = StudentList };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Add(string viewName, FormCollection fc)
        {
            var model = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (model == null)
                return Content("<script>alert('参数错误！请返回后重试！');window.location.href ='/prms/treatiseconfig'</script>");
            if (model.TreatiseStage > 2)
                return Content("<script>alert('项目已开启或已结束，无法再次添加学生！');window.location.href ='/prms/treatiseconfig'</script>");
            //定义一个students的集合去替换Add页面中ID为studentid的区域
            string students = Request.Form["studentid"];
            if (!string.IsNullOrWhiteSpace(students))
            {
                var list = StringHelper.StringArrToIntList(students.Split(','));
                foreach (var sid in list)
                {
                    var student = _student.GetModel(sid);
                    if (student == null) continue;
                    _tStudent.SaveTreatiseStudentSubject(model.ID, student.ID);
                }
            }
            var studentlist = _tStudent.GetTreatiseList(model.ID);
            var pager = _tStudent.GetPageList(studentlist, 1, 10);
            ViewBag.TreatiseID = model.ID;
            var vm = new TreatiseConfigViewModel(model) { StudentPage = pager };
            return View("search", vm);

        }

        public ActionResult Detail(int id)
        {
            var model = _tStudent.GetModel(id);
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            var msg = _tStudent.Delete(id);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #region  导入项目学生
        public ActionResult Import()
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            CookieHelper.Del("FileUrl");

            return trea == null ? View("404") : View(new TreatiseConfigViewModel(trea));
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
            var list = GetList(ConfigHelper.ToServerPath(filepath));
            ViewBag.Treatise = treatise;
            ViewBag.TreatiseID = id;
            if (list.Any(d => d.State == 0))
                return View(new TreatiseConfigViewModel(treatise) { Imports = list });
            CookieHelper.SetObj("FilePath", filepath);

            if (list.Any(d => d.State == 0))
                return View(new TreatiseConfigViewModel(treatise) { Imports = list });
            return View(new TreatiseConfigViewModel(treatise) { Imports = list, State = 1 });
        }
        //导入数据库
        public JsonResult ImportDb()
        {
            var excelpath = CookieHelper.GetValue("FilePath");
            var list = GetList(ConfigHelper.ToServerPath(excelpath));
            var user = _admin.LoginUser();
            if (user == null) return Json(new RootViewModel() { Message = "用户不存在！" }, JsonRequestBehavior.AllowGet);
            var id = ConfigHelper.TreatiseID;
            var model = _treatise.GetModel(id);
            if (model == null)
                return Json(new RootViewModel() { Message = "参数错误！请返回后重试！" }, JsonRequestBehavior.AllowGet);
            try
            {
                foreach (var item in list.Where(d => d.State == 1 || d.State == 2))
                {
                    var student = item.Student;
                    //先判断学生是否存在数据库
                    //不存在就保存一次
                    if (item.State == 1)
                    {
                        student.UserInfo.CreateInfoID = user.InfoID;
                        student.UserInfo.Password = StringHelper.Md5Encrypt(student.UserInfo.LoginName);
                        _student.SaveOrEdit(student);
                    }
                    //保存学生与项目管理
                    _tStudent.SaveTreatiseStudentSubject(model.ID, student.ID);
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
        public List<ImportViewModel> GetList(string conn)
        {
            var list = new List<ImportViewModel>();
            var ds = VerificationViewModel.ExcelToDataSet(conn);
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var dr = ds.Tables[0].Rows[i];
                var model = GetModel(dr);
                if (model == null) continue;
                model.ID = i + 1;
                if (model.State == 1 && list.Count(d => d.State == 1 && d.Student.UserInfo.LoginName == model.Student.UserInfo.LoginName) > 1)
                    model.State = 0; model.Message += "  存在重复学生";
                list.Add(model);
            }
            return list;
        }
        ///获取excel一行的记录转化成实体
        public ImportViewModel GetModel(DataRow dr)
        {
            if (dr == null)
                return null;

            var studentNo = dr[3].ToString();
            if (string.IsNullOrEmpty(studentNo))
                return new ImportViewModel() { Message = "当前行学号为空，不会被导入", State = 3 };

            var schoolInfolist = _school.GetAllList().Where(d => d.State == 1);
            var schoolInfoId = 0;

            var college = dr[0].ToString();
            var grade = dr[1].ToString();
            var classes = dr[2].ToString();
            var studentname = dr[4].ToString();
            var sex = dr[5].ToString();
            var tel = dr[6].ToString();
            var email = dr[7].ToString();
            var qq = dr[8].ToString();
            var wechat = dr[9].ToString();
            var comment = dr[10].ToString();

            #region 登录帐号(学号)

            if (string.IsNullOrEmpty(studentNo))
                return new ImportViewModel()
                {
                    Message = "登录帐号不能为空",
                    State = 0
                };
            studentNo = studentNo.Trim();
            var student = _student.GetStudentByLoginName(studentNo);
            if (student != null)
                return new ImportViewModel()
                {
                    Student = student,
                    State = 2,
                    Message = "当前学生已存在于系统"
                };


            #endregion

            #region 专业

            if (string.IsNullOrEmpty(college))
                return new ImportViewModel()
                {
                    Message = "专业信息不能为空",
                    State = 0
                };
            college = college.Trim();
            var collegelist = from c in schoolInfolist
                              where c.InfoType == 4
                              where c.InfoName == college
                              select c;
            if (collegelist.Count() != 1)
                return new ImportViewModel()
                {
                    Message = "专业信息不正确",
                    State = 0
                };

            schoolInfoId = collegelist.FirstOrDefault().ID;

            #endregion

            #region 年级

            if (string.IsNullOrEmpty(grade))
                return new ImportViewModel()
                {
                    Message = "年级信息不能为空",
                    State = 0
                };


            grade = grade.Trim();
            var gradelist = from c in schoolInfolist
                            where c.ParentID == schoolInfoId
                            where c.InfoName == grade
                            select c;
            if (gradelist.Count() != 1)
                return new ImportViewModel()
                {
                    Message = "年级信息不正确",
                    State = 0
                };
            schoolInfoId = gradelist.FirstOrDefault().ID;

            #endregion

            #region 班级

            if (string.IsNullOrEmpty(classes))
                return new ImportViewModel()
                {
                    Message = "班级信息不能为空",
                    State = 0
                };

            classes = classes.Trim();
            var classlist = from c in schoolInfolist
                            where c.ParentID == schoolInfoId
                            where c.InfoName == classes
                            select c;
            if (classlist.Count() != 1)

                return new ImportViewModel()
                {
                    Message = "班级信息不正确",
                    State = 0
                };

            var model = new ImportViewModel();
            model.Student.UserInfo.LoginName = studentNo;

            model.Student.SchoolID = classlist.FirstOrDefault().ID;



            #endregion

            #region 姓名
            if (string.IsNullOrEmpty(studentname))
                return new ImportViewModel()
                {
                    Message = "姓名不能为空",
                    State = 0
                };
            studentname = studentname.Trim();
            model.Student.UserInfo.TrueName = studentname;
            #endregion

            #region 性别

            if (string.IsNullOrEmpty(sex))
                return new ImportViewModel()
                {
                    Message = "性别不能为空",
                    State = 0
                };
            if (sex.Contains("男"))
                model.Student.UserInfo.Sex = 1;
            else if (sex.Contains("女"))
                model.Student.UserInfo.Sex = 2;
            else
                return new ImportViewModel()
                {
                    Message = "性别不正确",
                    State = 0
                };



            #endregion

            #region 联系电话

            if (!string.IsNullOrEmpty(tel))
            {
                tel = tel.Trim();
                if (VerificationViewModel.IsMobilePhone(tel))
                    model.Student.UserInfo.Tel = tel;
                else
                    return new ImportViewModel()
                    {
                        Message = "手机号不正确",
                        State = 0
                    };
            }

            #endregion

            #region 电子邮箱

            if (!string.IsNullOrEmpty(email))
            {
                email = email.Trim();
                if (VerificationViewModel.IsEmail(email))
                    model.Student.UserInfo.Email = email;
                else
                    return new ImportViewModel()
                    {
                        Message = "电子邮箱错误",
                        State = 0
                    };
            }

            #endregion

            if (!string.IsNullOrEmpty(qq))
                model.Student.UserInfo.QQ = qq.Trim();

            if (!string.IsNullOrEmpty(wechat))
                model.Student.UserInfo.WeChat = wechat.Trim();

            if (!string.IsNullOrEmpty(comment))
                model.Student.UserInfo.Comment = comment.Trim();

            model.State = 1;

            return model;
        }
        #endregion
    }
}
