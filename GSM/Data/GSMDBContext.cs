using GSM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GSM.Data
{
    public class GSMDBContext : IdentityDbContext<ApplicationUser>
    {
        public GSMDBContext(DbContextOptions<GSMDBContext> options) : base(options) { }

        public DbSet<Gadget> Gadgets { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Gadget>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Customer)
                    .WithMany(x => x.Gadgets)
                    .HasForeignKey(x => x.CustomerId);
            });
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();
                entity.HasOne(x => x.Gadget)
                    .WithMany(x => x.Contracts)
                    .HasForeignKey(x => x.GadgetId);
                entity.HasOne(x => x.Employee)
                    .WithMany(x => x.Contracts)
                    .HasForeignKey(x => x.EmployeeId);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
