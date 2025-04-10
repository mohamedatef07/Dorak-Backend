using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dorak.Models;
using Repositories;
using Data;
using Services;
using Dorak.ViewModels.ServiceViewModel;
using Dorak.ViewModels;

namespace AdminArea.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        public S_Services s_Services;

        public ServicesController(S_Services _s_Services)
        {
            s_Services = _s_Services;

        }

        public IActionResult Index(string searchText = "", string ServiceName = "", int pageNumber = 1,
           int pageSize = 3)
        {
            var searchList = s_Services.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            return View(searchList);
        }
        //Services/Index
        //public IActionResult Index()
        //{
        //    var servicesList = s_Services.GetAll()?.ToList();
        //    return View(servicesList);
        //}

        // Add service
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddServiceViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var service = new Service
            {
                ServiceName = vm.ServiceName,
                Description = vm.Description,
                //Priority = vm.Priority,
                BasePrice = vm.BasePrice
            };

            s_Services.Add(service);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var service = s_Services.GetById(id);
            if (service == null)
                return NotFound();

            var vm = new ServiceViewModel
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                //Priority = service.Priority,
                BasePrice = service.BasePrice
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult Edit(ServiceViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var service = new Service
            {
                ServiceId = vm.ServiceId,
                ServiceName = vm.ServiceName,
                Description = vm.Description,
                //Priority = vm.Priority,
                BasePrice = vm.BasePrice
            };

            s_Services.Edit(service);
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Delete(int id)
        {
            s_Services.Delete(id);
            return RedirectToAction("Index");

            //return NotFound();
        }


    }
}