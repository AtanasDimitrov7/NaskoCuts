using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Classic Fade", Description = "Clean and timeless fade.", Price = 20, DurationMinutes = 30, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1503951914875-452162b0f3f1?auto=format&fit=crop&w=800&q=80" },
                new Service { Id = 2, Name = "Undercut", Description = "Bold sides, stylish top.", Price = 25, DurationMinutes = 40, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1585747860715-2ba37e788b70?auto=format&fit=crop&w=800&q=80" },
                new Service { Id = 3, Name = "Pompadour", Description = "Volume and flair.", Price = 30, DurationMinutes = 45, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1621605815971-fbc98d665033?auto=format&fit=crop&w=800&q=80" },
                new Service { Id = 4, Name = "Beard Trim & Shape", Description = "Perfectly shaped beard.", Price = 15, DurationMinutes = 20, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1599351431202-1e0f0137899a?auto=format&fit=crop&w=800&q=80" },
                new Service { Id = 5, Name = "Hot Towel Shave", Description = "Luxury hot towel experience.", Price = 35, DurationMinutes = 50, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1606041008023-472dfb5e530f?auto=format&fit=crop&w=800&q=80" },
                new Service { Id = 6, Name = "Kids Cut", Description = "Fun cut for the little ones.", Price = 12, DurationMinutes = 25, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1519345182560-3f2917c472ef?auto=format&fit=crop&w=800&q=80" }
            );

            builder.Entity<Barber>().HasData(
                new Barber { Id = 1, FullName = "Nasko Dimitrov", Role = "Founder & Master Barber", IsActive = true },
                new Barber { Id = 2, FullName = "Alex Petrov", Role = "Senior Barber", IsActive = true },
                new Barber { Id = 3, FullName = "Martin Ivanov", Role = "Fade Specialist", IsActive = true },
                new Barber { Id = 4, FullName = "Georgi Kolev", Role = "Beard & Shave Expert", IsActive = true }
            );
        }
    }
}