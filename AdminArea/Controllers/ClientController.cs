using Dorak.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AdminArea.Controllers
{
    public class ClientController : Controller
    {
        private readonly ClientServices clientService;

        public ClientController(ClientServices _clientService)
        {

            clientService = _clientService;
        }
        public IActionResult Index(string searchText = "", string providerName = "",
            string city = "", int pageNumber = 1,
            int pageSize = 5)
        {
            var searchList = clientService.Search(searchText: searchText, pageNumber: pageNumber, pageSize: pageSize);
            return View(searchList);
        }

        [HttpGet]
        public IActionResult Edit(string clientId)
        {
            var selected = clientService.GetClientById(clientId);
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(Client client)
        {
            if (client == null)
            {
                return NotFound();
            }
            clientService.EditClient(client);
            return RedirectToAction("Index");
        }
        public IActionResult Delete(string clientId)
        {
            var selected = clientService.GetClientById(clientId);
            if (selected == null)
            {
                return NotFound();
            }
            if (clientService.DeleteClient(selected))
            {
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
