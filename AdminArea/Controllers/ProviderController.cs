using Dorak.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace AdminArea.Controllers
{
    public class ProviderController : Controller
    {
        private ProviderRepository providerRepository;
        public ProviderController(ProviderRepository _providerRepository)
        {

            providerRepository = _providerRepository;   
        }




        public IActionResult Index(string searchText = "", int pageNumber = 1,
            int pageSize = 2)
        {
            var list = providerRepository.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(list);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var selected = providerRepository.GetList(i => i.ProviderId == Id).FirstOrDefault();
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(Provider provider)
        {
            providerRepository.Edit(provider);
            return RedirectToAction("Index");
        }


        public IActionResult Delete(string Id) 
        {

            var selected = providerRepository.GetList(i => i.ProviderId == Id).FirstOrDefault();
            providerRepository.Delete(selected);
            providerRepository.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
