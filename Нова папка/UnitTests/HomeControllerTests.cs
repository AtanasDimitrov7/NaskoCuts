using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NaskoCuts.Controllers;
using NaskoCuts.Data;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Tests
{
    public class HomeControllerTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private HomeController CreateController(ApplicationDbContext db)
        {
            var logger = Mock.Of<ILogger<HomeController>>();
            var controller = new HomeController(logger, db);
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            return controller;
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            var controller = CreateController(CreateDb());
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            var controller = CreateController(CreateDb());
            var result = controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task BookAppointment_Get_ReturnsView()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var result = await CreateController(db).BookAppointment();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task BookAppointment_Post_EmptyName_ReturnsView()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            var result = await controller.BookAppointment(
                "", "ivan@test.com", "0888000000", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), null);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task BookAppointment_Post_InvalidEmail_ReturnsView()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            var result = await controller.BookAppointment(
                "Иван Иванов", "невалиденимейл", "0888000000", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), null);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task BookAppointment_Post_EmptyPhone_ReturnsView()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            var result = await controller.BookAppointment(
                "Иван Иванов", "ivan@test.com", "", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), null);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task BookAppointment_Post_PastDate_ReturnsView()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            var result = await controller.BookAppointment(
                "Иван Иванов", "ivan@test.com", "0888000000", 1, 1,
                DateTime.Now.AddDays(-2).ToString("yyyy-MM-ddTHH:mm"), null);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task BookAppointment_Post_ValidData_RedirectsToConfirmed()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Id = 1, Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { Id = 1, FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            var result = await controller.BookAppointment(
                "Иван Иванов", "ivan@test.com", "0888000000", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), "Без бележки");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AppointmentConfirmed", redirect.ActionName);
        }

        [Fact]
        public async Task BookAppointment_Post_ValidData_SavesAppointmentInDb()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Id = 1, Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { Id = 1, FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            await controller.BookAppointment(
                "Иван Иванов", "ivan@test.com", "0888000000", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), null);

            Assert.Equal(1, await db.Appointments.CountAsync());
        }

        [Fact]
        public async Task BookAppointment_Post_ValidData_SetsStatusPending()
        {
            var db = CreateDb();
            db.Services.Add(new Service { Id = 1, Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 });
            db.Barbers.Add(new Barber { Id = 1, FullName = "Наско", IsActive = true });
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            await controller.BookAppointment(
                "Иван Иванов", "ivan@test.com", "0888000000", 1, 1,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm"), null);

            var saved = await db.Appointments.FirstAsync();
            Assert.Equal(AppointmentStatus.Pending, saved.Status);
        }

        [Fact]
        public void AppointmentConfirmed_NoTempData_RedirectsToIndex()
        {
            var controller = CreateController(CreateDb());
            var result = controller.AppointmentConfirmed();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public void AppointmentConfirmed_WithTempData_ReturnsView()
        {
            var controller = CreateController(CreateDb());
            controller.TempData["Confirmation"] = "NC-20240101-123";

            var result = controller.AppointmentConfirmed();
            Assert.IsType<ViewResult>(result);
        }
    }
}