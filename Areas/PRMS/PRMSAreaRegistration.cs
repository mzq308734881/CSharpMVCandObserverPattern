using System.Web.Mvc;

namespace LanShanPRMS.Areas.PRMS
{
    public class PRMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "prms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PRMS_default",
                "prms/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "LanShanPRMS.Areas.PRMS.Controllers" }
            );
        }
    }
}