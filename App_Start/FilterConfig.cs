using System.Web;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;

namespace LanShanPRMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (ConfigHelper.IsDebug)
                filters.Add(new DebugAttribute());
            else
                filters.Add(new ErrorAttribute());
        }
    }
}