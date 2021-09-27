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
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentsGetIdViewModel>>> GetСomments()
        {
            return await _context.Comments.Include(c => c.User).Select(c => new CommentsGetIdViewModel
            {
                Id = c.Id,
                Description = c.Description,
                Published = c.Published.ToLongDateString(),
                User = c.User
            }).ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentsGetIdViewModel>> GetComment(int id)
        {
            var comment = await _context.Comments.Include(c => c.User).Select(c=> new CommentsGetIdViewModel
            {
                Id = c.Id,
                Description = c.Description,
                Published = c.Published.ToLongDateString(),
                User = c.User
            }).FirstOrDefaultAsync(p => p.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CommentsPostIdViewModel com)
        {
            Post post = _context.Posts.Include(p => p.User).Include(p => p.Comments).FirstOrDefault(p => p.Id == com.PostId);
            Comment comment = new Comment
            {
                Id = post.Comments.Count + 1,
                Description = com.Description,
                Published = DateTime.Now,
                User = _context.Users.FirstOrDefault(p => p.Id == com.UserUpId),
                Post = post
            };

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
