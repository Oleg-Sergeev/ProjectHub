using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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


        [TestMethod]
        public async Task CreatePOST_ReturnCreateViewIfModelIsNotValid()
        {
            var authorMock = new Mock<IAuthorRepository>();
            var projectMock = new Mock<IProjectRepository>();

            var authorVM = new AuthorViewModel()
            {
                Id = 1,
                FirstName = "",
                LastName = null,
                ProjectsId = null
            };

            projectMock.Setup(x => x.WhereToListAsync(p => authorVM.ProjectsId.Contains(p.Id), default)).ReturnsAsync(default(List<Project>));
            authorMock.Setup(x => x.AddAsync(It.IsAny<Author>(), default)).ThrowsAsync(new DbUpdateException("Sql exception"));

            AuthorController controller = new(authorMock.Object, projectMock.Object);

            try
            {
                var result = await controller.Create(authorVM);

                Assert.IsInstanceOfType(result, typeof(ViewResult));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
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
