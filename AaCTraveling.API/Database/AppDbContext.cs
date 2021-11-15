using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AaCTraveling.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AaCTraveling.API.Database {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute {
                Id = Guid.NewGuid(),
                Title = "TestTesttestRoute1",
                Description = "TestRouteDescription1",
                OriginalPrice = 10086,
                CreateTime = DateTime.UtcNow
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
