using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using jokesWebApp.Data;
using jokesWebApp.Models;
using System.Security.Claims;

namespace jokesWebApp.Controllers;

[Route("[controller]")]
public class JokesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public JokesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userJokes = await _context.Joke
            .Include(j => j.Likes)
            .Where(j => j.UserId == userId)
            .ToListAsync();
        return View(userJokes);
    }

    [HttpGet("search")]
    [Authorize]
    public IActionResult ShowSearchForm()
    {
        return View();
    }

    [HttpGet("search/results")]
    [Authorize]
    public async Task<IActionResult> ShowSearchResults(string? searchPhrase)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var query = _context.Joke.Include(j => j.Likes).Where(j => j.UserId == userId).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(j => j.JokeQuestion.Contains(searchPhrase) || j.JokeAnswer.Contains(searchPhrase));
        }
        return View("SearchResults", await query.ToListAsync());
    }

    [HttpGet("myprofile")]
    public IActionResult MyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.Identity?.Name ?? "Not logged in";

        ViewBag.UserId = userId;
        ViewBag.Email = userEmail;
        return View();
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Joke joke)
    {
        if (!ModelState.IsValid)
        {
            return View(joke);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Check if joke with same question and answer already exists
        var existingJoke = await _context.Joke.FirstOrDefaultAsync(j =>
            j.JokeQuestion.ToLower() == joke.JokeQuestion.ToLower() &&
            j.JokeAnswer.ToLower() == joke.JokeAnswer.ToLower());

        if (existingJoke != null)
        {
            ModelState.AddModelError("", "This joke already exists. Please enter a different joke.");
            return View(joke);
        }

        joke.UserId = userId;
        joke.CreatedAt = DateTime.UtcNow;

        _context.Add(joke);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var joke = await _context.Joke
            .Include(j => j.Comments.OrderByDescending(c => c.CreatedAt))
            .FirstOrDefaultAsync(j => j.Id == id);
        if (joke == null)
        {
            return NotFound();
        }

        // Load user emails for comments
        var commentEmails = new Dictionary<string, string>();
        foreach (var comment in joke.Comments)
        {
            if (!commentEmails.ContainsKey(comment.UserId))
            {
                var user = await _userManager.FindByIdAsync(comment.UserId);
                commentEmails[comment.UserId] = user?.Email ?? "Unknown User";
            }
        }

        ViewBag.CommentEmails = commentEmails;
        return View(joke);
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var joke = await _context.Joke.FindAsync(id);
        if (joke == null)
        {
            return NotFound();
        }

        return View(joke);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Joke joke)
    {
        if (id != joke.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(joke);
        }

        try
        {
            _context.Update(joke);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Joke.Any(j => j.Id == joke.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var joke = await _context.Joke.FirstOrDefaultAsync(j => j.Id == id);
        if (joke == null)
        {
            return NotFound();
        }

        return View(joke);
    }

    [HttpPost("delete/{id:int}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var joke = await _context.Joke.FindAsync(id);
        if (joke == null)
        {
            return NotFound();
        }

        _context.Joke.Remove(joke);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("like/{id:int}")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var joke = await _context.Joke.FindAsync(id);
        if (joke == null)
        {
            return NotFound();
        }

        // Check if user already liked this joke
        var existingLike = await _context.JokeLike.FirstOrDefaultAsync(
            l => l.JokeId == id && l.UserId == userId);

        if (existingLike != null)
        {
            // Unlike the joke
            _context.JokeLike.Remove(existingLike);
        }
        else
        {
            // Like the joke
            var like = new JokeLike { JokeId = id, UserId = userId };
            _context.JokeLike.Add(like);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("comment/add")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int jokeId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return BadRequest("Comment cannot be empty");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var joke = await _context.Joke.FindAsync(jokeId);
        if (joke == null)
        {
            return NotFound();
        }

        var comment = new JokeComment
        {
            JokeId = jokeId,
            UserId = userId,
            CommentText = commentText,
            CreatedAt = DateTime.UtcNow
        };

        _context.JokeComment.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = jokeId });
    }

    [HttpPost("comment/delete/{id:int}")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.JokeComment.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comment.UserId != userId)
        {
            return Forbid();
        }

        var jokeId = comment.JokeId;
        _context.JokeComment.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = jokeId });
    }

    [HttpPost("comment/edit/{id:int}")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditComment(int id, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return BadRequest("Comment cannot be empty");
        }

        var comment = await _context.JokeComment.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (comment.UserId != userId)
        {
            return Forbid();
        }

        comment.CommentText = commentText;
        _context.JokeComment.Update(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = comment.JokeId });
    }
}
