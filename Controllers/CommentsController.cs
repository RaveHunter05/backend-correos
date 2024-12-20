using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


using Microsoft.AspNetCore.Authorization;

using correos_backend.Services;

namespace correos_backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentsController : ControllerBase

	{
		private readonly CurrentTimeService _currentTimeService;
		private readonly CorreosContext _context;

		private readonly UserManager<IdentityUser> _userManager;

		public CommentsController(CorreosContext context, CurrentTimeService currentTimeService, UserManager<IdentityUser> userManager)
		{
			_context = context;
			_currentTimeService = currentTimeService;
			_userManager = userManager;
		}

		// GET: api/Comments
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
		{
			if (_context.Comments == null)
			{
				return NotFound();
			}
			return await _context.Comments.ToListAsync();
		}

		// GET: api/Comments/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Comment>> GetComment(int id)
		{
			if (_context.Comments == null)
			{
				return NotFound();
			}
			var comment = await _context.Comments.FindAsync(id);

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
			if (id != comment.CommentId)
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
		public async Task<ActionResult<Comment>> PostComment(Comment comment)
		{
			if (_context.Comments == null)
			{
				return Problem("Entity set 'CorreosContext.Comments'  is null.");
			}

			if(comment.Reason == null)
			{
				return BadRequest("Reason is required");
			}

			var creatorID = User.Identity.Name;
			comment.CreatedById = creatorID;
			var creator = await _userManager.FindByIdAsync(creatorID);

			comment.CreatorUsername = creator.UserName;

			comment.Date = _currentTimeService.GetCurrentTime();

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetComment", new { id = comment.CommentId }, comment);
		}

		// DELETE: api/Comments/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteComment(int id)
		{
			if (_context.Comments == null)
			{
				return NotFound();
			}
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
			return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
		}
	}
}
