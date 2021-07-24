using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly ApplicationContext _db;


        public AuthorController(ApplicationContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> About(int id)
        {
            return View(await _db.Authors.GetAuthorWithProjectsAsync(id));
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Projects = new List<SelectListItem>(await _db.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToListAsync());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorViewModel authorVM)
        {
            var author = new Author()
            {
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
                Projects = await _db.Projects
                .Where(p => authorVM.ProjectsId.Contains(p.Id))
                .ToListAsync()
            };

            await _db.Authors.AddAsync(author);

            await _db.SaveChangesAsync();

            return Redirect("~/");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _db.Authors.GetAuthorWithProjectsAsync(id);

            var authorVM = new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                ProjectsId = author.Projects.Select(p => p.Id)
            };

            ViewBag.Projects = new List<SelectListItem>(await _db.Projects
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = authorVM.ProjectsId.Contains(p.Id)
                }).ToListAsync());

            return View(authorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AuthorViewModel editedAuthorVM)
        {
            var author = await _db.Authors.GetAuthorWithProjectsAsync(id);

            author.FirstName = editedAuthorVM.FirstName;
            author.LastName = editedAuthorVM.LastName;
            author.Projects = await _db.Projects
                .Where(p => editedAuthorVM.ProjectsId.Contains(p.Id))
                .ToListAsync();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _db.Authors.GetAuthorWithProjectsAsync(id));
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
