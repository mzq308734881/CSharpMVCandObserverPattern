using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Service;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Core;
using LanShanPRMS.IService;

namespace LanShanPRMS.ViewModel
{
    public class TeacherViewModel
    {
        readonly private ITeacherService _teacher = new TeacherService();
        private readonly ISchoolService _school = new SchoolService();
        readonly private ISubjectService _subject = new SubjectService();
        readonly private ITreatiseTeacherService _tTeacher = new TreatiseTeacherService();
        readonly private ITreatiseService _treatise = new TreatiseService();
        public Teacher Teacher { get; set; }
        public UserInfo UserInfo { get; set; }
        public string SchoolName => ConfigHelper.SchoolName + ConfigHelper.MajorName;
        /// <summary>
        /// 审核中的课题
        /// </summary>
        public int Checking { get; set; }
        /// <summary>
        /// 进行中的项目
        /// </summary>
        public List<Treatise> TreatiseList { get; set; }
        /// <summary>
        /// 参加毕业论文总人数
        /// </summary>
        public int Altogether { get; set; }
        public TeacherViewModel()
        {
            var user = _teacher.GetModel(ConfigHelper.TeacherID);
            if (user != null)
            {
                Teacher = user;
                UserInfo = user.UserInfo ?? new UserInfo();
                Checking = GetCheckSubjectCount();
                TreatiseList = GetTreatise();
              
            }
            else
            {
                Teacher = new Teacher();
                UserInfo = new UserInfo();
                TreatiseList = new List<Treatise>();
                Checking = 0;
            }
        }
        private int GetCheckSubjectCount()
        {
            return _subject.GetAllList().Count(c => c.TeacherID == Teacher.ID && c.SubjectState != 1);
        }
        public List<Treatise> GetTreatise()
        {
            var list = (from d in _tTeacher.GetAllList() where d.TeacherID == Teacher.ID select d.TreatiseID).ToList();
            return (from c in _treatise.GetAllList() where list.Contains(c.ID) select c).ToList();
        }
    }
}
