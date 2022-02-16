using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static Prospecto.Enums.Enums;

namespace Prospecto.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        private RoleManager<IdentityRole> roleManager;
        public RolesController(RoleManager<IdentityRole> roleMgr)
        {
            roleManager = roleMgr;
        }

        public ViewResult Index() => View(roleManager.Roles);

        public IActionResult Create() => View("~/Views/Roles/Add.cshtml");

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    Alert("El Dato Fue Guardado", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("Necesita un Nombre el Rol", NotificationType.error);
                    return View("~/Views/Roles/Add.cshtml");
                }

            }
            return View("~/Views/Roles/Add.cshtml");
        }

        public IActionResult Update(string id)
        {
            Alert("Opción Desabilitada", NotificationType.info);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            Alert("Opción Desabilitada", NotificationType.info);
            return RedirectToAction("Index");
            //IdentityRole role = await roleManager.FindByIdAsync(id);
            //if (role != null)
            //{
            //    IdentityResult result = await roleManager.DeleteAsync(role);
            //    if (result.Succeeded)
            //    {
            //        Alert("El Dato Fue Borrado", NotificationType.success);
            //        return RedirectToAction("Index");
            //    }
            //    else
            //    {
            //        Alert("El Dato No Pudo Ser Borrado", NotificationType.success);
            //        return View("~/Views/Roles/Index.cshtml");
            //    }
            //}
            //else
            //    //ModelState.AddModelError("", "No role found");
            //    Alert("Rol No Se Encontro", NotificationType.success);
            //    return View("Index", roleManager.Roles);
        }


    }
}