﻿using AaCTraveling.API.Database;
using AaCTraveling.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Services {
    public class TouristRouteRepository : ITouristRouteRepository {
        private readonly AppDbContext _context;
        public TouristRouteRepository(AppDbContext context) {
            _context = context;
        }
        public TouristRoute GetTouristRoute(Guid touristRouteId) {
            return _context.TouristRoutes.FirstOrDefault(n => n.Id == touristRouteId);
        }

        public IEnumerable<TouristRoute> GetTouristRoutes() {
            return _context.TouristRoutes;
        }
    }
}
