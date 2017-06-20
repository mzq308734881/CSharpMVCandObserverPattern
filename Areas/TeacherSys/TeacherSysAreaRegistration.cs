using System.Web.Mvc;

namespace LanShanPRMS.Areas.TeacherSys
{
    public class TeacherSysAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "teachersys";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TeacherSys_default",
                "teachersys/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "LanShanPRMS.Areas.TeacherSys.Controllers" }
            );
        }
    }
}
