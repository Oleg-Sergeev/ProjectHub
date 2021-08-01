using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Entities;
using Infrastructure.Data.Entities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private const int PageSize = 3;


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

            IQueryable<Project> query = null;

            if (!string.IsNullOrWhiteSpace(search)) query = _db.Projects.Where(p => p.Name.Contains(search));

            query = query is null
                ? _db.Projects.Include(p => p.Authors)
                : query.Include(p => p.Authors);

            query = order switch
            {
                "date" => query.OrderBy(p => p.CreatedAt),
                "date_d" => query.OrderByDescending(p => p.CreatedAt),
                "name_d" => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var pagedProjects = await query.GetPagedAsync(page, PageSize);

            var indexVM = new IndexViewModel()
            {
                Items = pagedProjects,
                PaginationInfo = new(page, PageSize, await query.CountAsync())
            };

            return View(indexVM);
        }

        [Authorize(Roles = Constants.AdminRoleName)]
        public async Task<IActionResult> AuthorsList(int page = 1)
        {
            if (page < 1) page = 1;


            var pagedAuthors = await _db.Authors
                .OrderBy(a => a.LastName)
                .GetPagedAsync(page, PageSize);

            var authorsListVM = new AuthorsListViewModel()
            {
                Items = pagedAuthors,
                PaginationInfo = new(page, PageSize, await _db.Authors.CountAsync())
            };

            return View(authorsListVM);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var evm = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            return View(evm);
        }
    }
}
