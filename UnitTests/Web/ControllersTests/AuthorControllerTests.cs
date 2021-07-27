using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Web.Controllers;
using Web.ViewModels;

namespace UnitTests.Web.ControllersTests
{
    [TestClass]
    public class AuthorControllerTests
    {
        [TestMethod]
        public void About_ReturnNotFoundIfAuthorNotFound()
        {
            var mock = new Mock<IAuthorRepository>();

            int id = -123;

            mock.Setup(x => x.GetByIdWithProjectsAsync(id, default)).ReturnsAsync(default(Author));

            AuthorController controller = new(mock.Object, null);

            IActionResult result = controller.About(id).Result;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public void About_ReturnViewIfAuthorFound()
        {
            var mock = new Mock<IAuthorRepository>();

            int id = 1;

            mock.Setup(x => x.GetByIdWithProjectsAsync(id, default)).ReturnsAsync(new Author { Id = 1, FirstName = "Oleg", LastName = "Sergeev" });

            AuthorController controller = new(mock.Object, null);

            IActionResult result = controller.About(id).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [DataTestMethod]
        [DynamicData(nameof(GetAuthorViewModels), DynamicDataSourceType.Method)]
        public async Task CreatePOST_ReturnCreateViewIfModelIsNotValid(AuthorViewModel authorViewModel)
        {
            var authorMock = new Mock<IAuthorRepository>();
            var projectMock = new Mock<IProjectRepository>();

            projectMock.Setup(x => x.WhereToListAsync(p => authorViewModel.ProjectsId.Contains(p.Id), default)).ReturnsAsync(new List<Project>());


            authorMock.Setup(x => x.AddAsync(It.IsAny<Author>(), default)).ThrowsAsync(new DbUpdateException("Sql exception"));

            AuthorController controller = new(authorMock.Object, projectMock.Object);
            controller.ModelState.AddModelError("error", "invalid model");

            try
            {
                var result = await controller.Create(authorViewModel);

                Assert.IsInstanceOfType(result, typeof(ViewResult));
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
            var authorMock = new Mock<IAuthorRepository>();
            var projectMock = new Mock<IProjectRepository>();

            var authorVM = new AuthorViewModel()
            {
                Id = 1,
                FirstName = "Oleg",
                LastName = "Sergeev",
                ProjectsId = null
            };

            projectMock.Setup(x => x.WhereToListAsync(p => authorVM.ProjectsId.Contains(p.Id), default));

            AuthorController controller = new(authorMock.Object, projectMock.Object);


            await controller.Create(authorVM);
        }
    }
}
