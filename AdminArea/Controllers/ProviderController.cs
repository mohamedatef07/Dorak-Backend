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


    }
}
