using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IProjectRepository _projectRepository;


        public AuthorController(IAuthorRepository authorRepository, IProjectRepository projectRepository)
        {
            _authorRepository = authorRepository;
            _projectRepository = projectRepository;
        }


        public async Task<IActionResult> About(int id)
        {
            var author = await _authorRepository.GetByIdWithProjectsAsync(id);

            if (author is null) return NotFound();


            return View(author);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Projects = await _projectRepository.CreateMultiSelectListAsync("Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorViewModel authorVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();


            var author = new Author()
            {
                FirstName = authorVM.FirstName,
                LastName = authorVM.LastName,
                Projects = await _projectRepository.WhereToListAsync(p => authorVM.ProjectsId.Contains(p.Id))
            };

            await _authorRepository.AddAsync(author);

            return RedirectToAction(nameof(About), new { author.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorRepository.GetByIdWithProjectsAsync(id);

            if (author is null) return NotFound();


            var authorVM = new AuthorViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                ProjectsId = author.Projects.Select(p => p.Id).ToList()
            };

            ViewBag.Projects = await _projectRepository.CreateMultiSelectListAsync("Id", "Name", authorVM.ProjectsId);

            return View(authorVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AuthorViewModel editedAuthorVM)
        {
            if (!ModelState.IsValid) return RedirectToAction();

            var author = await _authorRepository.GetByIdWithProjectsAsync(id);

            if (author is null) return NotFound();


            author.FirstName = editedAuthorVM.FirstName;
            author.LastName = editedAuthorVM.LastName;
            author.Projects = await _projectRepository.WhereToListAsync(p => editedAuthorVM.ProjectsId.Contains(p.Id));

            await _authorRepository.UpdateAsync(author);

            return RedirectToAction(nameof(About), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorRepository.GetByIdWithProjectsAsync(id);

            if (author is null) return NotFound();


            return View(author);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author is null) return NotFound();


            await _authorRepository.RemoveAsync(author);

            return Redirect("~/");
        }
    }
}
