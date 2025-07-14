using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories;

namespace AdminArea.Controllers
{
    //[Authorize(Roles ="Admin")]
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
        public async Task<IActionResult> Edit(string roleName)
        {
            var selected = await roleRepository.GetRoleByNameAsync(roleName);
            return View(selected);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {

            var result = await roleRepository.UpdateRoleAsync(role.Id, role.Name);
            if (result.Succeeded)
            {
                return RedirectToAction("Add");
            }
            return View("Edit");
        }
        public async Task<IActionResult> Delete(string roleName)
        {
            var result = await roleRepository.DeleteRoleAsync(roleName);

            if (result.Succeeded)
            {
                return RedirectToAction("Add");
            }

            // Handle errors if deletion fails
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Add");
        }


    }
}
