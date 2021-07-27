using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _db;


        public HomeController(ApplicationContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;

            var pageSize = 5;

            var pagedProjects = await _db.Projects
                .OrderBy(p => p.Id)
                .Include(p => p.Authors)
                .GetPagedAsync(page, pageSize);


            return View(pagedProjects);
        }

        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> Authors(int page = 1)
        {
            if (page < 1) page = 1;

            var pageSize = 5;

            var pagedAuthors = await _db.Authors
                .OrderBy(a => a.Id)
                .GetPagedAsync(page, pageSize);


            return View(pagedAuthors);
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
