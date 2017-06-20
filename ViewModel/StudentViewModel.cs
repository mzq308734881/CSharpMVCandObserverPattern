using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.IService;
using LanShanPRMS.Service;

namespace LanShanPRMS.ViewModel
{
    public class StudentViewModel
    {
        private readonly IStudentService _student = new StudentService();
        private readonly ITreatiseStudentService _tStudent = new TreatiseStudentService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        public Student Student { get; set; }
        public UserInfo UserInfo { get; set; }
        public string SchoolName { get; set; }
        /// <summary>
        /// 进行中的项目
        /// </summary>
        public List<Treatise> TreatiseList { get; set; }
        public StudentViewModel()
        {
            var user = _student.GetModel(ConfigHelper.StudentID);
            if (user != null)
            {
                Student = user;
                UserInfo = user.UserInfo?? new UserInfo();
                var list = (from c in _tStudent.GetList(d => d.StudentID == Student.ID) select c.TreatiseID).Distinct().ToList();
                var treatises = from c in _treatise.GetAllList() where list.Contains(c.ID) select c;
                TreatiseList = treatises.ToList();
            }
            else
            {
                Student = new Student();
                UserInfo = new UserInfo();
                TreatiseList = new List<Treatise>();
            }
        }
    }
}
