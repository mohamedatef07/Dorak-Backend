using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dorak.Models;
using Repositories;
using Data;
using Services;

namespace AdminArea.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CenterController : Controller
    {
        public CenterServices centerServices;
        public CenterController(CenterServices _centerServices)
        {
            centerServices = _centerServices;
        }
        public IActionResult Index(string searchText = "", string centerName = "",
            string city = "", int pageNumber = 1,
            int pageSize = 5)
        {
            var searchList = centerServices.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            return View(searchList);
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
        [HttpGet]
        public IActionResult Details(int centerId)
        {
            var center = centerServices.GetById(centerId);
            if (center != null)
            {
                return View("Details", center);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Delete(int centerId)
        {
            if (centerServices.Delete(centerId))
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult Active(int centerId)
        {
            if (centerServices.Active(centerId))
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult InActive(int centerId)
        {
            if (centerServices.Inactive(centerId))
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
