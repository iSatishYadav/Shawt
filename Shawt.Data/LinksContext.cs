using Microsoft.EntityFrameworkCore;

namespace Shawt.Data
{
    public partial class LinksContext : DbContext
    {
        public LinksContext()
        {
        }

        public LinksContext(DbContextOptions<LinksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationUsers> ApplicationUsers { get; set; }
        public virtual DbSet<Link> Link { get; set; }
        public virtual DbSet<Log> Log { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUsers>(entity =>
            {
                entity.HasKey(e => new { e.ApplicationId, e.UserName });

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<Link>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.OriginalLink).IsRequired();
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Browser).HasMaxLength(100);

                entity.Property(e => e.Device).HasMaxLength(100);

                entity.Property(e => e.IpAddress).HasMaxLength(50);

                entity.Property(e => e.Os).HasMaxLength(50);

                entity.HasOne(d => d.Link)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.LinkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Log_Link");
            });
        }

    }
}
