using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels;
using Web.ViewModels.Pagination;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;


        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var pageSize = 3;

            var allProjects = _context.Projects.Include(p => p.Authors);

            var projectsCount = await allProjects.CountAsync();

            var pagedProjects = await allProjects
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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
