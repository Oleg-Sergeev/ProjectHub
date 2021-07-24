using System;
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
    public class ProjectController : Controller
    {
        private readonly ApplicationContext _db;


        public ProjectController(ApplicationContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> About(int id)
        {
            return View(await _db.Projects.GetProjectWithProjectsAsync(id));
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = new List<SelectListItem>(await _db.Authors
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FullName
                }).ToListAsync());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectViewModel projectVM)
        {
            var project = new Project()
            {
                Name = projectVM.Name,
                Authors = await _db.Authors
                .Where(a => projectVM.AuthorsId.Contains(a.Id))
                .ToListAsync()
            };

            await _db.Projects.AddAsync(project);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { project.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _db.Projects.GetProjectWithProjectsAsync(id);

            var projectVM = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                AuthorsId = project.Authors.Select(a => a.Id)
            };

            ViewBag.Authors = new List<SelectListItem>(await _db.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName,
                    Selected = projectVM.AuthorsId.Contains(a.Id)
                }).ToListAsync());

            return View(projectVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProjectViewModel editedProjectVM)
        {
            var project = await _db.Projects.GetProjectWithProjectsAsync(id);

            project.Name = editedProjectVM.Name;
            project.Description = editedProjectVM.Description;
            project.Authors = await _db.Authors
                .Where(a => editedProjectVM.AuthorsId.Contains(a.Id))
                .ToListAsync();

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About), new { id });
        }


        public async Task<IActionResult> Delete(int id)
        {
            return View(await _db.Projects.GetProjectWithProjectsAsync(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var project = await _db.Projects.FindAsync(id);

            _db.Projects.Remove(project);

            await _db.SaveChangesAsync();

            return Redirect("~/");
        }
    }
}
