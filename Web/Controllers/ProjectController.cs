using System;
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
    public class ProjectController : Controller
    {
        private readonly ApplicationContext _db;


        public ProjectController(ApplicationContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> About(int id)
        {
            return View(
                await _db.Projects
                .Include(p => p.Authors)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync()
                );
        }

        public IActionResult Create()
        {
            ViewBag.Authors = new List<SelectListItem>(_db.Authors.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FullName }).ToList());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectViewModel projectVM)
        {
            var project = new Project()
            {
                Name = projectVM.Name,
                Authors = _db.Authors.Where(a => projectVM.AuthorsId.Contains(a.Id)).ToList()
            };

            await _db.Projects.AddAsync(project);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { project.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _db.Projects
                .Include(p => p.Authors)
                .Where(p => p.Id == id)
                .SingleAsync();

            var projectVM = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                AuthorsId = project.Authors.Select(a => a.Id)
            };

            ViewBag.Authors = new List<SelectListItem>(_db.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName,
                    Selected = projectVM.AuthorsId.Contains(a.Id)
                }).ToList());

            return View(projectVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProjectViewModel editedProjectVM)
        {
            var project = await _db.Projects
                .Include(p => p.Authors)
                .Where(p => p.Id == id)
                .SingleAsync();

            project.Name = editedProjectVM.Name;
            project.Description = editedProjectVM.Description;
            project.Authors = _db.Authors.Where(a => editedProjectVM.AuthorsId.Contains(a.Id)).ToList();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }


        public async Task<IActionResult> Delete(int id)
        {
            return View(
                await _db.Projects
                .Include(p => p.Authors)
                .Where(p => p.Id == id)
                .SingleAsync()
                );
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var project = await _db.Projects.FindAsync(id);

            if (project is null) return NotFound();

            _db.Projects.Remove(project);

            await _db.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
