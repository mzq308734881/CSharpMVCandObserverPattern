using System.Web.Mvc;

namespace LanShanPRMS.Areas.StudentSys
{
    public class StudentSysAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "studentsys";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StudentSys_default",
                "studentsys/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "LanShanPRMS.Areas.StudentSys.Controllers" }
            );
        }
    }
}
