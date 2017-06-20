using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanShanPRMS.Model.Models;
using LanShanPRMS.IService;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.Core;
using LanShanPRMS.Service;

namespace LanShanPRMS.ViewModel
{
    public class UserModuleViewModel
    {
        private readonly IAdminService _admin = new AdminService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly IModuleService _module = new ModuleService();
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly IUserRoleService _userRole = new UserRoleService();
        public IEnumerable<Module> ParnetModuleList { get; set; }
        public IEnumerable<Module> TowModuleList { get; set; }
        public User User { get; set; }
        public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 未开始的项目
        /// </summary>
        public IQueryable<Treatise> NotStart { get; set; }
        /// <summary>
        /// 进行中的项目
        /// </summary>
        public IQueryable<Treatise> UnderWay { get; set; }
        /// <summary>
        /// 已结束的项目
        /// </summary>
        public IQueryable<Treatise> Complete { get; set; }
        /// <summary>
        /// 正在进行中的人数
        /// </summary>
        public int StudentUnderWay { get; set; }
        /// <summary>
        /// 论文完成的人数
        /// </summary>
        public int StudentEnd { get; set; }
        /// <summary>
        /// 参加毕业论文总人数  
        /// </summary>
        public int StudentAll { get; set; }
        /// <summary>
        /// 查询当前用所拥有的二级菜单
        /// </summary>
        /// <returns></returns>
        public List<Module> GetTowModuleList()
        {
            return (from c in _userRole.GetMenuListByUser(User.ID) where c.ParentID != null select c).ToList();
        }

        /// <summary>
        /// 查询当前用所拥有的一级菜单
        /// </summary>
        /// <returns></returns>
        public List<Module> GetParnetModuleList()
        {
            var ids = string.Join(".", GetTowModuleList().Select(d => d.CascadeID));
            return _module.GetAllList().Where(d=>ids.Contains(d.ID.ToString())&&d.Layer==0).ToList();
        }

        public UserModuleViewModel()
        {
            var user = _admin.LoginUser();
            if (user == null) return;
            User = user;
            UserInfo = user?.UserInfo ?? new UserInfo();
            TowModuleList = GetTowModuleList();
            ParnetModuleList = GetParnetModuleList();
            var treas = _treatise.GetList(d => d.State == 1);
            if (user.School != null)
            {
                var school = _school.GetUserChildSchool(User.SchoolID ?? 0);
                school.Add(User.SchoolID ?? 0);
                treas = treas.Where(d => school.Contains(d.SchoolID ?? 0));
            }

            NotStart = treas.Where(d => d.TreatiseStage == 0);
            UnderWay = treas.Where(d => d.TreatiseStage == 1);
            Complete = treas.Where(d => d.TreatiseStage == 2);
            StudentAll = 0;
            StudentUnderWay = 0;
            StudentEnd = 0;
            return;
        }
    }
}
