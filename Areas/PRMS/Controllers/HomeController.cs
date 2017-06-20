using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;
using LanShanPRMS.ControllerBase;
using LanShanPRMS.IService;
using LanShanPRMS.Model.Models;
using LanShanPRMS.Service;
using LanShanPRMS.ViewModel;

namespace LanShanPRMS.Areas.PRMS.Controllers
{
    [UserAdmin]
    public class HomeController : BaseAdminController
    {
        private readonly ITreatiseService _treatise = new TreatiseService();
        private readonly ISchoolService _school = new SchoolService();
        private readonly ITreatiseProcessService _process = new TreatiseProcessService();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult TreatiseList()
        {
            var list =
                _treatise.GetAllList()
                    .OrderByDescending(d => d.StartTime)
                    .Select(d => new EchartsView() { name = d.TreatiseName, value = d.TreatiseStudents.Count });

            var model = new EchartsViewModel(list.ToList());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SchoolProcesss(int? id)
        {
            var trea = _treatise.GetModel(ConfigHelper.TreatiseID);
            if (id == null || id < 1) id = trea?.SchoolID;

            if (id == null || id < 1)
                return Json(new EchartsStack(), JsonRequestBehavior.AllowGet);
            var schoolist = _school.GetLevelDownByCurrentID(id ?? 0).ToList();

            var process = _process.GetTreatiseList(trea.ID).ToList();
            var model = new EchartsStack
            {
                Legendata = (from c in process select c.ProcessName).ToArray(),
                School = schoolist.ToDictionary(d => d.InfoName, d => d.ID),
                List = from c in schoolist
                       select new Series()
                       {
                           name = c.InfoName,
                           data = process.Select(d => d.StudentProcesses.Count(e => e.Student.School.CascadeID.Contains(c.ID + ".") || e.Student.SchoolID == c.ID)).ToArray()
                       },
            };

            return Json(model, JsonRequestBehavior.AllowGet);


        }
    }
}
