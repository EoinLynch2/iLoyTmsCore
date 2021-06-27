using iLoyTmsCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLoyTmsCore.Repo
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext>options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new TmsTaskMap(modelBuilder.Entity<TmsTask>());
        }

        //public DbSet<TmsTask> TmsTasks { get; set; }

    }
}
