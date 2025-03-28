using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dorak.Models;
using Repositories;

namespace AdminArea.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CenterController : Controller
    {
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
        public async Task<IActionResult> Delete(int centerId)
        {
            var result = await centerRepository.GetByIdAsync(c => c.CenterId == centerId);
            if (result != null)
            {
                centerRepository.Delete(result);
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
