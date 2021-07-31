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
    public class ProjectController : Controller
    {
        private readonly ApplicationContext _db;


        public ProjectController(ApplicationContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> About(int id, int page = 1)
        {
            var project = await _db.Projects
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project is null) return NotFound();

            ViewData["page"] = page;
            return View(project);
        }


        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Create()
        {
            var items = await _db.Authors.ToListAsync();

            ViewBag.Authors = new MultiSelectList(items, "Id", "FullName");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Create(ProjectViewModel projectVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();


            var project = new Project()
            {
                Name = projectVM.Name,
                CreatedAt = projectVM.CreatedAt,
                Authors = await _db.Authors.Where(p => projectVM.AuthorsId.Contains(p.Id)).ToListAsync()
            };

            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { project.Id });
        }


        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _db.Projects
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project is null) return NotFound();


            var projectVM = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                AuthorsId = project.Authors.Select(a => a.Id).ToList()
            };

            var items = await _db.Authors.ToListAsync();

            ViewBag.Authors = new MultiSelectList(items, "Id", "FullName", projectVM.AuthorsId);

            return View(projectVM);
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id, ProjectViewModel editedProjectVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();

            var project = await _db.Projects
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project is null) return NotFound();


            project.Name = editedProjectVM.Name;
            project.Description = editedProjectVM.Description;
            project.CreatedAt = editedProjectVM.CreatedAt;
            project.Authors = await _db.Authors.Where(p => editedProjectVM.AuthorsId.Contains(p.Id)).ToListAsync();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }


        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _db.Projects
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project is null) return NotFound();


            return View(project);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project is null) return NotFound();


            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();


            return RedirectToAction("Index", "Home");
        }
    }
}
