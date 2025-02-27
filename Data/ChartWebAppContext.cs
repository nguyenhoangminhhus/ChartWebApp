using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChartWebApp.Models;

namespace ChartWebApp.Data
{
    public class ChartWebAppContext : DbContext
    {
        public ChartWebAppContext (DbContextOptions<ChartWebAppContext> options)
            : base(options)
        {
        }

        public DbSet<ChartWebApp.Models.ChartData> Data { get; set; } = default!;
    }
}
