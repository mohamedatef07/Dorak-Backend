using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dorak.Models;
using Repositories;
using Dorak.Models.Enums;
using Data;
using Services;

namespace AdminArea.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CenterController : Controller
    {
        //public CenterServices centerServices;
        public CenterRepository centerRepository;
        public CenterController(CenterRepository _centerRepository)
        {
            centerRepository = _centerRepository;
        }
        public IActionResult Index()
        {
            var centerList = centerRepository.GetAll()?.ToList();
            return View(centerList);
        }
        //[HttpGet]
        //public IActionResult Add()
        //{
        //    return View("Add");
        //}
        //[HttpPost]
        //public IActionResult Add(Center center)
        //{
        //    return View("Add");
        //}
        //[HttpGet]
        //public IActionResult Details(int centerId)
        //{
        //    var center = centerServices.GetById(c => c.CenterId == centerId);
        //    if (center != null)
        //    {
        //        return View("Details", center);
        //    }
        //    return NotFound();
        //}
        //[HttpGet]
        //public IActionResult Delete(int centerId)
        //{
        //    var result = centerServices.GetById(c => c.CenterId == centerId);
        //    if (result != null)
        //    {
        //        centerServices.Delete(result);
        //        CommitData.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return NotFound();
        //}
        //[HttpGet]
        //public IActionResult Active(int centerId)
        //{
        //    var center = centerServices.GetById(c => c.CenterId == centerId);
        //    if (center != null)
        //    {
        //        center.CenterStatus = CenterStatus.Active;
        //        centerServices.Edit(center);
        //        CommitData.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return NotFound();
        //}
        //[HttpGet]
        //public IActionResult InActive(int centerId)
        //{
        //    var center = centerServices.GetById(c => c.CenterId == centerId);
        //    if (center != null)
        //    {
        //        center.CenterStatus = CenterStatus.Inactive;
        //        centerServices.Edit(center);
        //        CommitData.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return NotFound();
        //}
    }
}
