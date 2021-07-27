using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IProjectRepository _projectRepository;


        public ProjectController(IAuthorRepository authorRepository, IProjectRepository projectRepository)
        {
            _authorRepository = authorRepository;
            _projectRepository = projectRepository;
        }


        public async Task<IActionResult> About(int id)
        {
            var project = await _projectRepository.GetByIdWithAuthorsAsync(id);

            if (project is null) return NotFound();


            return View(project);
        }

        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _authorRepository.CreateMultiSelectListAsync("Id", "FullName");

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
                Authors = await _authorRepository.WhereToListAsync(a => projectVM.AuthorsId.Contains(a.Id))
            };

            await _projectRepository.AddAsync(project);

            return RedirectToAction(nameof(About), new { project.Id });
        }

        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectRepository.GetByIdWithAuthorsAsync(id);

            if (project is null) return NotFound();


            var projectVM = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                AuthorsId = project.Authors.Select(a => a.Id).ToList()
            };

            ViewBag.Authors = await _authorRepository.CreateMultiSelectListAsync("Id", "FullName", projectVM.AuthorsId);

            return View(projectVM);
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Edit(int id, ProjectViewModel editedProjectVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();

            var project = await _projectRepository.GetByIdWithAuthorsAsync(id);

            if (project is null) return NotFound();


            project.Name = editedProjectVM.Name;
            project.Description = editedProjectVM.Description;
            project.CreatedAt = editedProjectVM.CreatedAt;
            project.Authors = await _authorRepository.WhereToListAsync(a => editedProjectVM.AuthorsId.Contains(a.Id));

            await _projectRepository.UpdateAsync(project);

            return RedirectToAction(nameof(About), new { id });
        }


        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectRepository.GetByIdWithAuthorsAsync(id);

            if (project is null) return NotFound();


            return View(project);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project is null) return NotFound();


            await _projectRepository.RemoveAsync(project);


            return RedirectToAction("Index", "Home");
        }
    }
}
