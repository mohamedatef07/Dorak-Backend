using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;

namespace AdminArea.Controllers
{
    public class ProviderController : Controller
    {

        private readonly ProviderServices providerService;

        public ProviderController(ProviderServices providerService)
        {

            this.providerService = providerService;
        }



        public IActionResult Index(string searchText = "", int pageNumber = 1,
            int pageSize = 2)
        {
            var list = providerService.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(list);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var selected = providerService.GetProviderById(Id);
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(Provider provider)
        {
            providerService.EditProvider(provider);
            return RedirectToAction("Index");
        }


        public IActionResult Delete(string Id)
        {

            var selected = providerService.GetProviderById(Id);

            providerService.DeleteProvider(selected);

            return RedirectToAction("Index");
        }
    }
}
