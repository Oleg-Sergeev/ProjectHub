using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;

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
            ViewBag.Projects = new List<SelectListItem>(_db.Projects.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorViewModel authorVM)
        {
            var author = new Author()
            {
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
                Projects = _db.Projects.Where(p => authorVM.ProjectsId.Contains(p.Id)).ToList()
            };

            await _db.Authors.AddAsync(author);

            await _db.SaveChangesAsync();

            return Redirect("~/");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _db.Authors
                .Include(a => a.Projects)
                .Where(a => a.Id == id)
                .SingleAsync();

            var authorVM = new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                ProjectsId = author.Projects.Select(p => p.Id)
            };

            ViewBag.Projects = new List<SelectListItem>(_db.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = authorVM.ProjectsId.Contains(p.Id)
                }).ToList());

            return View(authorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AuthorViewModel editedAuthorVM)
        {
            var author = await _db.Authors
                .Include(a => a.Projects)
                .Where(a => a.Id == id)
                .SingleAsync();

            author.FirstName = editedAuthorVM.FirstName;
            author.LastName = editedAuthorVM.LastName;
            author.Projects = _db.Projects.Where(p => editedAuthorVM.ProjectsId.Contains(p.Id)).ToList();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _db.Authors
                .Include(a => a.Projects)
                .Where(a => a.Id == id)
                .SingleAsync());
        }

        [HttpPost]
        [ActionName("Delete")]
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
