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
        private readonly IProjectRepository _projectRepository;


        public HomeController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
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
