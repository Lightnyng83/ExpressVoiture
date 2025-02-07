using ExpressVoitures.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpressVoitures.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<CarBrand> CarBrands { get; set; }

        public virtual DbSet<CarBrandModel> CarBrandModels { get; set; }

        public virtual DbSet<CarModel> CarModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-ExpressVoitures-cea89d45-b133-413b-a3d4-a3572252c30d;Trusted_Connection=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasOne(d => d.CarBrandModel).WithMany(p => p.Cars)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Car_CarBrandModelId");
            });

            modelBuilder.Entity<CarBrandModel>(entity =>
            {
                entity.HasKey(e => e.CarBrandModelId).HasName("PK_CarBrandModelId_1");

                entity.HasOne(d => d.CarBrand).WithMany(p => p.CarBrandModels)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBrandModelId_CarBrand1");

                entity.HasOne(d => d.CarModel).WithMany(p => p.CarBrandModels)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CarBrandModelId_CarModel1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
