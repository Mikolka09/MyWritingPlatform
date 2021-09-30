using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWritingPlatform.Models;
using MyWritingPlatform.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public UsersController(UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model, IFormFile AvatarFile)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Login = model.Login,
                    Email = model.Email,
                    UserName = model.Email,
                    Year = model.Year,
                    DateTimeRegister = DateTime.Now
                };

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

                //Изменяем размер картинки
                byte[] imgB = System.IO.File.ReadAllBytes(filePath);

                using (MemoryStream ms = new MemoryStream(imgB, 0, imgB.Length))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        int h = 0;
                        int w = 0;
                        if (img.Width > 512)
                        {
                            int changeSize = 100 - (Math.Abs(img.Width - 512) / (img.Width / 100));

                            h = (img.Height / 100) * changeSize;
                            w = (img.Width / 100) * changeSize;
                        }
                        else
                        {
                            h = img.Height;
                            w = img.Width;
                        }

                        using (Bitmap b = new Bitmap(img, new Size(w, h)))
                        {
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                                imgB = ms2.ToArray();
                            }
                        }
                    }
                }

                System.IO.File.WriteAllBytes(filePath, imgB);

                #endregion

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //назначаем роль User
                    var addRoles = new string[] { "User" };
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
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
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
            return View(model);
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

                        //Изменяем размер картинки
                        byte[] imgB = System.IO.File.ReadAllBytes(filePath);

                        using (MemoryStream ms = new MemoryStream(imgB, 0, imgB.Length))
                        {
                            using (Image img = Image.FromStream(ms))
                            {
                                int h = 0;
                                int w = 0;
                                if (img.Width > 512)
                                {
                                    int changeSize = 100 - (Math.Abs(img.Width - 512) / (img.Width / 100));

                                    h = (img.Height / 100) * changeSize;
                                    w = (img.Width / 100) * changeSize;
                                }
                                else
                                {
                                    h = img.Height;
                                    w = img.Width;
                                }

                                using (Bitmap b = new Bitmap(img, new Size(w, h)))
                                {
                                    using (MemoryStream ms2 = new MemoryStream())
                                    {
                                        b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        imgB = ms2.ToArray();
                                    }
                                }
                            }
                        }

                        System.IO.File.WriteAllBytes(filePath, imgB);

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
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
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
            return View(model);
        }
    }
}
