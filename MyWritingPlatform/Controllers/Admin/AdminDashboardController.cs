using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Data;
using MyWritingPlatform.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminDashboardController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }

        // GET: Posts
        public async Task<IActionResult> IndexPost()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User);
            return View("~/Views/Admin/IndexPost.cshtml", await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> DetailsPost(int? id)
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

            return View("~/Views/Admin/DetailsPost.cshtml", post);
        }

        // GET: Posts/Create
        public IActionResult CreatePost()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View("~/Views/Admin/CreatePost.cshtml");
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("Id,ImgPost,Title,ShortDescription,Description,Published,Censor,UserId,CategoryId")] Post post, int[] tags, IFormFile ImgPostFile)
        {
            if (ModelState.IsValid)
            {
                #region Обработка изображения

                var wwwRootPath = _environment.WebRootPath; // URL - для сайта
                var fileName = Path.GetRandomFileName().Replace('.', '_')
                     + Path.GetExtension(ImgPostFile.FileName);
                var filePath = Path.Combine(wwwRootPath + "\\storage\\avatars\\", fileName); // Реальный путь 

                post.ImgPost = "/storage/avatars/" + fileName; // ссылка на картинку

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImgPostFile.CopyToAsync(stream);
                }

                #endregion

                post.Tags = _context.Tags.Where(t => tags.Contains(t.Id)).ToList();
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexPost));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View("~/Views/Admin/CreatePost.cshtml", post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View("~/Views/Admin/EditPost.cshtml", post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("Id,ImgPost,Title,ShortDescription,Description,Published,Censor,CategoryId")] Post post, int[] tags, IFormFile ImgPostFile)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            Post oldPost = _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).FirstOrDefault(p => p.Id == post.Id);
            post.Category = _context.Categories.First(c => c.Id == post.CategoryId);
            post.User = oldPost.User;

            if (ImgPostFile != null)
            {

                #region Обработка изображения

                var wwwRootPath = _environment.WebRootPath; // URL - для сайта
                var fileName = Path.GetRandomFileName().Replace('.', '_')
                     + Path.GetExtension(ImgPostFile.FileName);
                var filePath = Path.Combine(wwwRootPath + "\\storage\\avatars\\", fileName); // Реальный путь 

                post.ImgPost = "/storage/avatars/" + fileName; // ссылка на картинку

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImgPostFile.CopyToAsync(stream);
                }

                #endregion
            }
            else
                post.ImgPost = oldPost.ImgPost;

            bool oldCensor = post.Censor;
            _context.Entry(post).State = EntityState.Detached;
            await _context.SaveChangesAsync();
            post = _context.Posts.Where(p => p.Id == id).Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).First();
            List<Tag> oldTags = post.Tags;
            post.Censor = oldCensor;

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
                return RedirectToAction(nameof(IndexPost));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View("~/Views/Admin/EditPost.cshtml", post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> DeletePost(int? id)
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

            return View("~/Views/Admin/DeletePost.cshtml", post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexPost));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
