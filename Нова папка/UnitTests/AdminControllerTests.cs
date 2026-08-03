using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NaskoCuts.Controllers;
using NaskoCuts.Data;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Tests
{
    public class MockSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();
        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _store.Keys;
        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }

    public class AdminControllerTests
    {
        private ApplicationDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AdminController CreateController(ApplicationDbContext db)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "AdminSettings:AdminPanelPassword", "testpass123" }
                })
                .Build();

            var httpContext = new DefaultHttpContext();
            var session = new MockSession();
            session.SetString("IsAdmin", "true");
            httpContext.Session = session;

            var controller = new AdminController(db, config);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            return controller;
        }

        private async Task<(Service service, Barber barber)> SeedServiceAndBarber(ApplicationDbContext db)
        {
            var service = new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 };
            var barber = new Barber { FullName = "Наско", IsActive = true };
            db.Services.Add(service);
            db.Barbers.Add(barber);
            await db.SaveChangesAsync();
            return (service, barber);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            var result = await CreateController(CreateDb()).Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsAllAppointments()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            db.Appointments.AddRange(
                new Appointment { ClientName = "А", ClientEmail = "a@a.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now.AddDays(1), ConfirmationCode = "NC-001" },
                new Appointment { ClientName = "Б", ClientEmail = "b@b.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now.AddDays(2), ConfirmationCode = "NC-002" }
            );
            await db.SaveChangesAsync();

            var result = await CreateController(db).Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Appointment>>(view.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Delete_ExistingAppointment_RemovesIt()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            var appt = new Appointment { ClientName = "Иван", ClientEmail = "i@i.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now.AddDays(1), ConfirmationCode = "NC-001" };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            await CreateController(db).Delete(appt.Id);

            Assert.Equal(0, await db.Appointments.CountAsync());
        }

        [Fact]
        public async Task Delete_ExistingAppointment_RedirectsToIndex()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            var appt = new Appointment { ClientName = "Иван", ClientEmail = "i@i.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now.AddDays(1), ConfirmationCode = "NC-001" };
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();

            var result = await CreateController(db).Delete(appt.Id);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_NonExistingId_ReturnsRedirect()
        {
            var result = await CreateController(CreateDb()).Delete(999);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task ManageServices_ReturnsViewResult()
        {
            var result = await CreateController(CreateDb()).ManageServices();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ManageServices_ReturnsAllServices()
        {
            var db = CreateDb();
            db.Services.AddRange(
                new Service { Name = "Fade", Price = 20, IsActive = true, DurationMinutes = 30 },
                new Service { Name = "Beard", Price = 10, IsActive = false, DurationMinutes = 20 }
            );
            await db.SaveChangesAsync();

            var result = await CreateController(db).ManageServices();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Service>>(view.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task CreateService_AddsServiceToDb()
        {
            var db = CreateDb();
            await CreateController(db).CreateService("Нова", "Описание", 25, 45, "");
            Assert.Equal(1, await db.Services.CountAsync());
        }

        [Fact]
        public async Task CreateService_RedirectsToManageServices()
        {
            var result = await CreateController(CreateDb()).CreateService("Нова", "Описание", 25, 45, "");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageServices", redirect.ActionName);
        }

        [Fact]
        public async Task CreateService_SetsIsActiveTrue()
        {
            var db = CreateDb();
            await CreateController(db).CreateService("Нова", "", 25, 45, "");
            var service = await db.Services.FirstAsync();
            Assert.True(service.IsActive);
        }

        [Fact]
        public async Task DeleteService_RemovesServiceFromDb()
        {
            var db = CreateDb();
            var service = new Service { Name = "Test", Price = 10, IsActive = true, DurationMinutes = 30 };
            db.Services.Add(service);
            await db.SaveChangesAsync();

            await CreateController(db).DeleteService(service.Id);

            Assert.Equal(0, await db.Services.CountAsync());
        }

        [Fact]
        public async Task DeleteService_NonExistingId_ReturnsRedirect()
        {
            var result = await CreateController(CreateDb()).DeleteService(999);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task ToggleService_ActiveToInactive()
        {
            var db = CreateDb();
            var service = new Service { Name = "Test", Price = 10, IsActive = true, DurationMinutes = 30 };
            db.Services.Add(service);
            await db.SaveChangesAsync();

            await CreateController(db).ToggleService(service.Id);

            var updated = await db.Services.FindAsync(service.Id);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task ToggleService_InactiveToActive()
        {
            var db = CreateDb();
            var service = new Service { Name = "Test", Price = 10, IsActive = false, DurationMinutes = 30 };
            db.Services.Add(service);
            await db.SaveChangesAsync();

            await CreateController(db).ToggleService(service.Id);

            var updated = await db.Services.FindAsync(service.Id);
            Assert.True(updated!.IsActive);
        }

        [Fact]
        public async Task ManageBarbers_ReturnsViewResult()
        {
            var result = await CreateController(CreateDb()).ManageBarbers();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ManageBarbers_ReturnsAllBarbers()
        {
            var db = CreateDb();
            db.Barbers.AddRange(
                new Barber { FullName = "Наско", IsActive = true },
                new Barber { FullName = "Алекс", IsActive = true }
            );
            await db.SaveChangesAsync();

            var result = await CreateController(db).ManageBarbers();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Barber>>(view.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task CreateBarber_AddsBarberToDb()
        {
            var db = CreateDb();
            await CreateController(db).CreateBarber("Наско Димитров", "Senior", "Описание", "", "");
            Assert.Equal(1, await db.Barbers.CountAsync());
        }

        [Fact]
        public async Task CreateBarber_RedirectsToManageBarbers()
        {
            var result = await CreateController(CreateDb()).CreateBarber("Наско", "Senior", "", "", "");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageBarbers", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteBarber_RemovesBarberFromDb()
        {
            var db = CreateDb();
            var barber = new Barber { FullName = "Наско", IsActive = true };
            db.Barbers.Add(barber);
            await db.SaveChangesAsync();

            await CreateController(db).DeleteBarber(barber.Id);

            Assert.Equal(0, await db.Barbers.CountAsync());
        }

        [Fact]
        public async Task ToggleBarber_ActiveToInactive()
        {
            var db = CreateDb();
            var barber = new Barber { FullName = "Наско", IsActive = true };
            db.Barbers.Add(barber);
            await db.SaveChangesAsync();

            await CreateController(db).ToggleBarber(barber.Id);

            var updated = await db.Barbers.FindAsync(barber.Id);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task ToggleBarber_InactiveToActive()
        {
            var db = CreateDb();
            var barber = new Barber { FullName = "Наско", IsActive = false };
            db.Barbers.Add(barber);
            await db.SaveChangesAsync();

            await CreateController(db).ToggleBarber(barber.Id);

            var updated = await db.Barbers.FindAsync(barber.Id);
            Assert.True(updated!.IsActive);
        }

        [Fact]
        public async Task Statistics_ReturnsViewResult()
        {
            var result = await CreateController(CreateDb()).Statistics();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Statistics_CorrectTotalCount()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            db.Appointments.AddRange(
                new Appointment { ClientName = "А", ClientEmail = "a@a.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "1", Status = AppointmentStatus.Pending },
                new Appointment { ClientName = "Б", ClientEmail = "b@b.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "2", Status = AppointmentStatus.Completed }
            );
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            await controller.Statistics();

            Assert.Equal(2, controller.ViewBag.Total);
        }

        [Fact]
        public async Task Statistics_CorrectPendingCount()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            db.Appointments.AddRange(
                new Appointment { ClientName = "А", ClientEmail = "a@a.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "1", Status = AppointmentStatus.Pending },
                new Appointment { ClientName = "Б", ClientEmail = "b@b.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "2", Status = AppointmentStatus.Confirmed }
            );
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            await controller.Statistics();

            Assert.Equal(1, controller.ViewBag.Pending);
        }

        [Fact]
        public async Task Statistics_RevenueOnlyFromCompleted()
        {
            var db = CreateDb();
            var (service, barber) = await SeedServiceAndBarber(db);
            db.Appointments.AddRange(
                new Appointment { ClientName = "А", ClientEmail = "a@a.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "1", Status = AppointmentStatus.Completed },
                new Appointment { ClientName = "Б", ClientEmail = "b@b.com", ClientPhone = "0888", ServiceId = service.Id, BarberId = barber.Id, AppointmentDate = DateTime.Now, ConfirmationCode = "2", Status = AppointmentStatus.Pending }
            );
            await db.SaveChangesAsync();

            var controller = CreateController(db);
            await controller.Statistics();

            Assert.Equal(20m, controller.ViewBag.Revenue);
        }
    }
}