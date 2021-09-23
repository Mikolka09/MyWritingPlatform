using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Data;
using MyWritingPlatform.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers
{
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
            var post = await _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).FirstOrDefaultAsync(p=>p.Id==id);
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
            post.Category = oldPost.Category;
            post.User = oldPost.User;
            
            if (ModelState.IsValid)
            {
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
                
                try
                {
                    post.Tags = _context.Tags.Where(t => tags.Contains(t.Id)).ToList();
                    _context.Entry(oldPost).State = EntityState.Detached;
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
