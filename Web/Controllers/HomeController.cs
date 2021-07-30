using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Infrastructure.Data.Pagination;
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

        public async Task<IActionResult> Index(int page = 1, string search = "", string order = "")
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(order) ? "name_d" : "";
            ViewData["DateSort"] = order == "date" ? "date_d" : "date";
            ViewData["CurrentSort"] = order;
            ViewData["CurrentSearch"] = search;

            (ViewData["NameSortSymbol"], ViewData["DateSortSymbol"]) = order switch
            {
                "name_d" => ("▼", ""),
                "date" => ("", "▲"),
                "date_d" => ("", "▼"),
                _ => ("▲", "")
            };

            if (page < 1) page = 1;

            var pageSize = 2;

            IQueryable<Project> query = null;

            if (!string.IsNullOrWhiteSpace(search)) query = _db.Projects.Where(p => p.Name.Contains(search));

            query = query == null
                ? _db.Projects.Include(p => p.Authors)
                : query.Include(p => p.Authors);

            query = order switch
            {
                "date" => query.OrderBy(p => p.CreatedAt),
                "date_d" => query.OrderByDescending(p => p.CreatedAt),
                "name_d" => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var pagedProjects = await query.GetPagedAsync(page, pageSize);


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
