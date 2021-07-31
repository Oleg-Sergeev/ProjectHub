using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Entities;
using Infrastructure.Data.Entities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly ApplicationContext _db;


        public AuthorController(ApplicationContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> About(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Projects)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();


            return View(author);
        }


        public async Task<IActionResult> Create()
        {
            var items = await _db.Projects.ToListAsync();

            ViewBag.Projects = new MultiSelectList(items, "Id", "Name");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Create(AuthorViewModel authorVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();


            var author = new Author()
            {
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
                Projects = await _db.Projects.Where(p => authorVM.ProjectsId.Contains(p.Id)).ToListAsync()
            };

            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { author.Id });
        }


        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Projects)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();


            var authorVM = new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                ProjectsId = author.Projects.Select(p => p.Id).ToList()
            };

            var items = await _db.Projects.ToListAsync();

            ViewBag.Projects = new MultiSelectList(items, "Id", "Name", authorVM.ProjectsId);


            return View(authorVM);
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id, AuthorViewModel editedAuthorVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();

            var author = await _db.Authors
                .Include(a => a.Projects)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();


            author.FirstName = editedAuthorVM.FirstName;
            author.LastName = editedAuthorVM.LastName;
            author.Projects = await _db.Projects.Where(p => editedAuthorVM.ProjectsId.Contains(p.Id)).ToListAsync();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }


        [HttpGet]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Projects)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();


            return View(author);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var author = await _db.Authors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author is null) return NotFound();


            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
