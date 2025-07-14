using Dorak.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AdminArea.Controllers
{
    public class OperatorController : Controller
    {
        private readonly OperatorServices operatorServices;

        public OperatorController(OperatorServices _operatorServices)
        {

            operatorServices = _operatorServices;
        }
        public IActionResult Index(string searchText = "", string operatorName = "",
            string city = "", int pageNumber = 1,
            int pageSize = 5)
        {
            var searchList = operatorServices.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            return View(searchList);
        }
        [HttpGet]
        public IActionResult Edit(string operatorId)
        {
            var selected = operatorServices.GetOperatorsById(operatorId);
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(Operator _operator)
        {
            if (_operator == null)
            {
                return NotFound();
            }
            operatorServices.EditOperator(_operator);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string operatorId)
        {
            if (string.IsNullOrEmpty(operatorId))
            {
                return NotFound();
            }
            if (!await operatorServices.DeleteOperator(operatorId))
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
    }
}
