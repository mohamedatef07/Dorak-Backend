using Dorak.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AdminArea.Controllers
{
    public class ProviderController : Controller
    {

        private readonly ProviderServices providerService;

        public ProviderController(ProviderServices _providerService)
        {

            providerService = _providerService;
        }
        public IActionResult Index(string searchText = "", string providerName = "",
            string city = "", int pageNumber = 1,
            int pageSize = 5)
        {
            var searchList = providerService.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            return View(searchList);
        }

        [HttpGet]
        public IActionResult Edit(string providerId)
        {
            var selected = providerService.GetProviderById(providerId);
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(Provider provider)
        {
            if (provider == null)
            {
                return NotFound();
            }
            providerService.EditProvider(provider);
            return RedirectToAction("Index");
        }
        public IActionResult Delete(string providerId)
        {
            var selected = providerService.GetProviderById(providerId);
            if (selected == null)
            {
                return NotFound();
            }
            if (providerService.DeleteProvider(selected))
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
