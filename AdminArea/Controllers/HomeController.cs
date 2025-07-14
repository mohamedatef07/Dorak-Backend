using Microsoft.AspNetCore.Mvc;
using Services;

namespace AdminArea.Controllers
{
    public class HomeController : Controller
    {
        private readonly CenterServices centerServices;

        public HomeController(CenterServices _centerServices)
        {
            centerServices = _centerServices;
        }

        public IActionResult Index()
        {
            var statistics = centerServices.GetStatisticsViewModel();
            return View(statistics);
        }

    }
}
