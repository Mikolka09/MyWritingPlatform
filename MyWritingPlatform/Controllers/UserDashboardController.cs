using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using MyWritingPlatform.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _environment;

        public UserDashboardController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View("~/Views/User/Index.cshtml");
        }

        // GET: UserDashboard/Details/5
        public async Task<IActionResult> Details()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            return View("~/Views/User/Details.cshtml", currentUser);
        }

        public async Task<IActionResult> ChangePassword()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = currentUser.Id, Email = currentUser.Email };
            return View("~/Views/User/ChangePassword.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View("~/Views/User/ChangePassword.cshtml", model);
        }

        public async Task<IActionResult> Edit()
        {
            User user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Avatar = user.Avatar,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Login = user.Login,
                Email = user.Email,
                Year = user.Year
            };
            return View("~/Views/User/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, IFormFile AvatarFile)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Login = model.Login;
                    user.Year = model.Year;

                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        user.UserName = model.Email;
                        user.NormalizedEmail = model.Email.ToUpper();
                        user.NormalizedUserName = model.Email.ToUpper();
                    }
                    if (AvatarFile != null)
                    {

                        #region Обработка изображения

                        var wwwRootPath = _environment.WebRootPath; // URL - для сайта
                        var fileName = Path.GetRandomFileName().Replace('.', '_')
                             + Path.GetExtension(AvatarFile.FileName);
                        var filePath = Path.Combine(wwwRootPath + "\\storage\\avatars\\", fileName); // Реальный путь 

                        user.Avatar = "/storage/avatars/" + fileName; // ссылка на картинку

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await AvatarFile.CopyToAsync(stream);
                        }

                        #endregion
                    }

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View("~/Views/User/Edit.cshtml", model);
        }

        public async Task<IActionResult> Delete()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            return View("~/Views/User/Delete.cshtml", currentUser);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _userManager.DeleteAsync(user); 
            }
            return RedirectToAction("Index", "Home");
        }
    }
}



