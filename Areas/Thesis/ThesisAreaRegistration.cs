using System.Web.Mvc;

namespace LanShanPRMS.Areas.Thesis
{
    public class ThesisAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "thesis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Thesis_default",
                "thesis/{controller}/{action}/{id}",
               new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "LanShanPRMS.Areas.Thesis.Controllers" }
            );
        }
    }
}
