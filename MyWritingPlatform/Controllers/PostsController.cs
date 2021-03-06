using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Data;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PostsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).Where(p => p.User.Id == currentUser.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImgPost,Title,ShortDescription,Description,Published,Censor,CategoryId")] Post post, int[] tags, IFormFile ImgPostFile)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            post.Published = DateTime.Now;
            post.Censor = false;
            post.User = await _context.Users.FirstOrDefaultAsync(p => p.Id == currentUser.Id);
            if (ModelState.IsValid)
            {
                #region Обработка изображения

                var wwwRootPath = _environment.WebRootPath; // URL - для сайта
                var fileName = Path.GetRandomFileName().Replace('.', '_')
                     + Path.GetExtension(ImgPostFile.FileName);
                var filePath = Path.Combine(wwwRootPath + "\\storage\\posts\\", fileName); // Реальный путь 

                post.ImgPost = "/storage/posts/" + fileName; // ссылка на картинку

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImgPostFile.CopyToAsync(stream);
                }

                //Изменяем размер картинки
                byte[] imgB = System.IO.File.ReadAllBytes(filePath);

                using (MemoryStream ms = new MemoryStream(imgB, 0, imgB.Length))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        int h = 0;
                        int w = 0;
                        if (img.Width > 768)
                        {
                            int changeSize = 100 - (Math.Abs(img.Width - 768) / (img.Width / 100));

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

                post.Tags = _context.Tags.Where(t => tags.Contains(t.Id)).ToList();
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            //var post = await _context.Posts.FindAsync(id);
            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImgPost,Title,ShortDescription,Description,Published,Censor,CategoryId")] Post post, int[] tags, IFormFile ImgPostFile)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            Post oldPost = _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).FirstOrDefault(p => p.Id == post.Id);
            post.Published = oldPost.Published;
            post.Censor = oldPost.Censor;
            post.Category = _context.Categories.First(c => c.Id == post.CategoryId); ;
            post.User = oldPost.User;

            if (ImgPostFile != null)
            {

                #region Обработка изображения

                var wwwRootPath = _environment.WebRootPath; // URL - для сайта
                var fileName = Path.GetRandomFileName().Replace('.', '_')
                     + Path.GetExtension(ImgPostFile.FileName);
                var filePath = Path.Combine(wwwRootPath + "\\storage\\posts\\", fileName); // Реальный путь 

                post.ImgPost = "/storage/posts/" + fileName; // ссылка на картинку

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImgPostFile.CopyToAsync(stream);
                }

                //Изменяем размер картинки
                byte[] imgB = System.IO.File.ReadAllBytes(filePath);

                using (MemoryStream ms = new MemoryStream(imgB, 0, imgB.Length))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        int h = 0;
                        int w = 0;
                        if (img.Width > 768)
                        {
                            int changeSize = 100 - (Math.Abs(img.Width - 768) / (img.Width / 100));

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
            else
                post.ImgPost = oldPost.ImgPost;

            Post newPost = post;
            _context.Entry(post).State = EntityState.Detached;
            await _context.SaveChangesAsync();
            post = _context.Posts.Where(p => p.Id == id).Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).First();
            List<Tag> oldTags = post.Tags;
            post.ImgPost = newPost.ImgPost;
            post.Censor = newPost.Censor;
            post.ShortDescription = newPost.ShortDescription;
            post.Description = newPost.Description;
            post.Title = newPost.Title;
            post.Published = newPost.Published;
            post.CategoryId = newPost.CategoryId;
            post.Category = newPost.Category;

            if (ModelState.IsValid)
            {
                try
                {
                    if (tags.Length != 0)
                    {
                        post.Tags = _context.Tags.Where(t => tags.Contains(t.Id)).ToList();
                    }
                    else
                    {
                        post.Tags = oldTags;
                    }
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
