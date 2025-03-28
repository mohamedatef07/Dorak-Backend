using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories;

namespace AdminArea.Controllers
{
    public class RoleController : Controller
    {
        private RoleRepository roleRepository;
        public RoleController(RoleRepository _roleRepository)
        {
            roleRepository = _roleRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var list = roleRepository.GetAll().Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
            }).ToList();

            //ViewBag.Invalid = 0;
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> Add(string roleName)
        {
            if (roleName.IsNullOrEmpty())
            {
                ViewBag.Invalid = 1;
                var list = roleRepository.GetAll().Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList();
                return View(list);
            }
            else
            {
                var res = await roleRepository.AddRoleAsync(roleName);
                if (res.Succeeded)
                {
                    ViewBag.Invalid = 2;
                }
                else
                {
                    ViewBag.Invalid = 1;
                }
                var list = roleRepository.GetAll().Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                }).ToList();
                return View(list);
            }

        }

        [HttpGet]
        public IActionResult Edit(string roleId, string name)
        {
            var selected = roleRepository.GetAll().Select(i => i.Id == roleId).FirstOrDefault();
            return View(selected);
        }
        [HttpPost]
        public IActionResult Edit(IdentityRole role)
        {
            
            roleRepository.Edit(role);
            return RedirectToAction("Add");
        }

    }
}
