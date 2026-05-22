using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Controllers;
using NaskoCuts.Data;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Tests
{
    public class ServicesControllerTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var result = await new ServicesController(CreateDb()).Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsOnlyActiveServices()
        {
            var db = CreateDb();
            db.Services.AddRange(
                new Service { Name = "Активна", Price = 20, IsActive = true, DurationMinutes = 30 },
                new Service { Name = "Неактивна", Price = 10, IsActive = false, DurationMinutes = 20 }
            );
            await db.SaveChangesAsync();

            var result = await new ServicesController(db).Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Service>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_EmptyDb_ReturnsEmptyList()
        {
            var result = await new ServicesController(CreateDb()).Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Service>>(view.Model);
            Assert.Empty(model);
        }

        [Fact]
        public async Task Index_MultipleActiveServices_ReturnsAll()
        {
            var db = CreateDb();
            db.Services.AddRange(
                new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 },
                new Service { Name = "Beard", Price = 15, IsActive = true, DurationMinutes = 20 },
                new Service { Name = "Shave", Price = 35, IsActive = true, DurationMinutes = 50 }
            );
            await db.SaveChangesAsync();

            var result = await new ServicesController(db).Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Service>>(view.Model);
            Assert.Equal(3, model.Count());
        }
    }
}