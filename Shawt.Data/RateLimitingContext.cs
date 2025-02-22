using Microsoft.EntityFrameworkCore;

namespace Shawt.Data
{
    public partial class RateLimitingContext : DbContext
    {
        public RateLimitingContext()
        {
        }

        public RateLimitingContext(DbContextOptions<RateLimitingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RateLimitRules> RateLimitRules { get; set; }
        public virtual DbSet<RateLimitCounters> RateLimitCounters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RateLimitCounters>(entity =>
            {
                entity.ToTable("RateLimitCounters", "RateLimiting");

                entity.HasAnnotation("SqlServer:MemoryOptimized", true);

                entity.Property(e => e.Id).HasMaxLength(200);
            });

            modelBuilder.Entity<RateLimitRules>(entity =>
            {
                entity.ToTable("RateLimitRules", "RateLimiting");

                entity.HasAnnotation("SqlServer:MemoryOptimized", true);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Endpoint).IsRequired();

                entity.Property(e => e.Period)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
