using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AaCTraveling.API.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AaCTraveling.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // set up seeds data
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute {
            //    Id = Guid.NewGuid(),
            //    Title = "TestTesttestRoute1",
            //    Description = "TestRouteDescription1",
            //    OriginalPrice = 10086,
            //    CreateTime = DateTime.UtcNow
            //});
            var touristRouteJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles)
                 .WithOne().HasForeignKey(ur => ur.UserId).IsRequired());

            //var adminRoleID = "guid";
            //modelBuilder.Entity<IdentityROle>



            base.OnModelCreating(modelBuilder);
        }
    }
}
