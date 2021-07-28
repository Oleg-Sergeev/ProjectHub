using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Controllers;
using Web.ViewModels;

namespace UnitTests.Web.ControllersTests
{
    [TestClass]
    public class AuthorControllerTests
    {
        private readonly ApplicationContext _db;


        public AuthorControllerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestApplication")
                .Options;

            _db = new(dbOptions);

            _db.SeedAsync().Wait();
        }

        [TestMethod]
        public void About_ReturnNotFoundIfAuthorNotFound()
        {
            int id = -123;

            AuthorController controller = new(_db);

            IActionResult result = controller.About(id).Result;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public void About_ReturnViewIfAuthorFound()
        {
            int id = 1;

            AuthorController controller = new(_db);

            IActionResult result = controller.About(id).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [DataTestMethod]
        [DynamicData(nameof(GetAuthorViewModels), DynamicDataSourceType.Method)]
        public async Task CreatePOST_ReturnRedirectToActionIfModelIsNotValid(AuthorViewModel authorViewModel)
        {
            AuthorController controller = new(_db);
            controller.ModelState.AddModelError("error", "invalid model");

            try
            {
                var result = await controller.Create(authorViewModel);

                Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            }
            catch (Exception e)
            {
                Assert.Fail($"{e.Message}\n{e.StackTrace}");
            }
        }

        private static IEnumerable<object[]> GetAuthorViewModels()
        {
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 1,
                FirstName = null,
                LastName = null
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 2,
                FirstName = "",
                LastName = ""
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 3,
                FirstName = "   ",
                LastName = "   "
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 4,
                FirstName = "Oleg",
                LastName = "    "
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 5,
                FirstName = "O",
                LastName = "Sergeev"
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 6,
                FirstName = "Oleg",
                LastName = "S"
            }};
            yield return new object[]{ new AuthorViewModel()
            {
                Id = 7,
                FirstName = "O   ",
                LastName = "S    "
            }};
        }


        [TestMethod]
        public async Task CreatePOST_NotThrowIfAuthorHasNotProjects()
        {
            var authorVM = new AuthorViewModel()
            {
                Id = 1,
                FirstName = "Oleg",
                LastName = "Sergeev",
                ProjectsId = null
            };

            AuthorController controller = new(_db);


            await controller.Create(authorVM);
        }
    }
}
