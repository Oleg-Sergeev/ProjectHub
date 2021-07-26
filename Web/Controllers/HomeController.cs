using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;
using Web.ViewModels.Pagination;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IProjectRepository _projectRepository;


        public HomeController(IProjectRepository projectRepository, IAuthorRepository authorRepository)
        {
            _projectRepository = projectRepository;
            _authorRepository = authorRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;

            var pageSize = 3;

            var projectsCount = await _projectRepository.CountAsync();

            var pagedProjects = _projectRepository
                .WithAuthors()
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            PageViewModel pageVM = new(page, projectsCount, pageSize);

            IndexViewModel indexVM = new(pagedProjects, pageVM);

            ViewBag.Page = page;

            return View(indexVM);
        }

        public async Task<IActionResult> Authors()
        {
            var authors = await _authorRepository.GetAllListAsync(false);

            return View(authors);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var evm = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            return View(evm);
        }
    }
}
