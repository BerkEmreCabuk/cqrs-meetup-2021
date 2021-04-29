using CQRSMeetup.WriteDomain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Infrastructures.Database
{
    public class CQRSMeetupDbContext : DbContext
    {
        public CQRSMeetupDbContext(DbContextOptions<CQRSMeetupDbContext> options)
           : base(options)
        {
        }
        public virtual DbSet<StockEntity> StockEntities { get; set; }
        public virtual DbSet<LocationEntity> LocationEntities { get; set; }
        public virtual DbSet<ProductEntity> ProductEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<LocationEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });
        }
    }
}
