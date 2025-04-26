using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using FMInatorul.Controllers;
using FMInatorul.Data;
using FMInatorul.Models;
using Assert = NUnit.Framework.Assert;

namespace FMInatorul.Tests.NUnitTests
{
    [TestFixture]
    public class CriticalFunctionsUnitTests
    {
        private StudentsController _controller;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private ApplicationDbContext _mockDbContext;
        private SqliteConnection _connection;

        [SetUp]
        public void SetUp()
        {
            // Create an in-memory SQLite database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _mockDbContext = new ApplicationDbContext(options);

            // Ensure the database is created
            _mockDbContext.Database.EnsureCreated();

            // Mock dependencies
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            // Initialize controller with mock dependencies and mocked user context
            _controller = new StudentsController(_mockDbContext, _mockUserManager.Object, _mockRoleManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "admin"),
                        }, "mock")),
                    },
                },
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the SQLite connection
            _mockDbContext.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task UploadPdf_QuizNotNullAsync()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write("Sample PDF Content");
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("sample.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            // Act
            var actionResult = await _controller.UploadPdf(fileMock.Object);

            // Assert
            Assert.That(actionResult, Is.Not.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UploadPdf_QuizHasQuestionsAsync()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write("Sample PDF Content");
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("sample.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            // Act
            IActionResult actionResult = await _controller.UploadPdf(fileMock.Object);

            // Assert
            Assert.That(actionResult, Is.Not.Null);

            if (actionResult is ViewResult viewResult)
            {
                var quiz = viewResult.Model as QuizModel;
                Assert.That(quiz, Is.Not.Null);
                Assert.That(quiz.Questions, Is.Not.Null);
                Assert.That(quiz.Questions, Is.Not.Empty);
            }
            else if (actionResult is ObjectResult objectResult && objectResult.StatusCode == 400)
            {
                Assert.That(objectResult.Value, Is.EqualTo("Invalid file format. Only PDF files allowed!"));
            }
        }

        [Test]
        public async Task UploadPdf_QuestionsHaveText()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write("Sample PDF Content");
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("sample.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            // Act
            IActionResult actionResult = await _controller.UploadPdf(fileMock.Object);

            // Assert
            Assert.That(actionResult, Is.Not.Null);
            if (actionResult is ViewResult viewResult)
            {
                var quiz = viewResult.Model as QuizModel;
                foreach (var question in quiz.Questions)
                {
                    Assert.That(string.IsNullOrWhiteSpace(question.Question), Is.False);
                }
            }
        }
    }
}