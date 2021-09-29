using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Data;
using MyWritingPlatform.Models;
using MyWritingPlatform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public PostsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostsViewModel>>> GetPosts()
        {
            var posts =  await _context.Posts.Include(p => p.Category).Include(p => p.Tags).Include(p => p.User).Include(p => p.Comments)
                .OrderByDescending(p => p.Published).Select(p => new PostsViewModel
                {
                    Id = p.Id,

                    ImgPost = p.ImgPost,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Published = p.Published.ToLongDateString(),
                    Censor = p.Censor,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    CategoryId = p.CategoryId,
                    CategoriesName = _context.Categories.Select(c => c.Name).ToList(),
                    ComCount = p.Comments.Count,
                    TagsName = _context.Tags.Select(t => t.Name).ToList(),
                    Tags = p.Tags,
                    Category = p.Category
                }).ToListAsync();
            return posts;
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<PostsGetIdViewModel>>> GetPost(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var comm = _context.Comments.Include(c=>c.User).Include(c=>c.Post).Where(c=>c.Post.Id == id).OrderByDescending(p=>p.Published).ToList();
            List<CommentViewModel> commentViewModels = new List<CommentViewModel>();
            foreach (var it in comm)
            {
                CommentViewModel commentView = new CommentViewModel
                {
                    Id = it.Id,
                    Description = it.Description,
                    Published = it.Published.ToLongDateString(),
                    Avatar = it.User.Avatar,
                    UserName = it.User.FirstName + " " + it.User.LastName
                };
                commentViewModels.Add(commentView);
            }
            
            var post = await _context.Posts.Where(p => p.Id == id).Include(p => p.User).Include(p => p.Comments)
                .Select(p => new PostsGetIdViewModel
                {
                    Id = p.Id,
                    UserUpId = currentUser.Id,
                    ImgPost = p.ImgPost,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Published = p.Published.ToLongDateString(),
                    Censor = p.Censor,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    Comments = commentViewModels
                }).ToListAsync();

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
