using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;
using LanShanPRMS.IService;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class TreatiseConfigController : BaseAdminController
    {
        //
        // GET: /PRMS/Student/
        readonly ITreatiseService _treatise = new TreatiseService();
        public ActionResult Index(int? id)
        {
            VerificationViewModel.DeleteCookie();
           
            if (id == null || id == 0)
                id = ConfigHelper.TreatiseID;
            var trea = _treatise.GetModel(id ?? 0);
            if (trea == null || trea.ID == 0)
            {
                var list = _treatise.GetAllList();
                if (list.Any(d => d.TreatiseStage == 1))
                    trea = list.FirstOrDefault(d => d.TreatiseStage == 1) ?? new Treatise();
                else
                    trea = list.FirstOrDefault() ?? new Treatise();
            }
            var model = new TreatiseConfigViewModel(trea);
            model.ProcessList =trea.TreatiseProcesses.ToList();
            ConfigHelper.SetTreatiseID(trea.ID);
            return View(model);
        }
    }
}
