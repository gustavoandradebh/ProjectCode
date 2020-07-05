using Microsoft.EntityFrameworkCore;
using ProjectCode.Domain.DataTransferObject;

namespace ProjectCode.Infraestructure.Repository
{
    public class ApiContext : DbContext
    {
        public DbSet<Project> Project { get; set; }
        public DbSet<Sdlc_System> SdlcSystem { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.id);
                entity.HasIndex(p => new { p.sdlcSystemId, p.externalId }).IsUnique();
                entity.HasOne(typeof(Sdlc_System)).WithMany().HasForeignKey("sdlcSystemId");
            });

            modelBuilder.Entity<Sdlc_System>(entity =>
            {
                entity.HasKey(p => p.id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
