using GPSTrucks.Simulator.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPSTrucks.Simulator.Infrastructure.Contexts
{
    public class LoggingDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
    }
}
