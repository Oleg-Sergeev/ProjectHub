using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;

namespace Web.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationContext _db;


        public AuthorController(ApplicationContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> About(int id)
        {
            return View(await _db.Authors
                .Include(a => a.Projects)
                .Where(a => a.Id == id)
                .SingleAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _db.Authors.FindAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author editedAuthor)
        {
            var author = await _db.Authors.FindAsync(id);

            if (author is null) return NotFound();

            author.FirstName = editedAuthor.FirstName;
            author.LastName = editedAuthor.LastName;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View(await _db.Authors.FindAsync(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var author = await _db.Authors.FindAsync(id);

            if (author is null) return NotFound();

            _db.Authors.Remove(author);

            await _db.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
