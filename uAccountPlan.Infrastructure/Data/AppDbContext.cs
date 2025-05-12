using Microsoft.EntityFrameworkCore;
using uAccountPlan.Domain.Entities;

namespace uAccountPlan.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AccountPlan> AccountPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountPlan>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Code).IsRequired();
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.Type).IsRequired();
            entity.Property(a => a.AcceptsLaunches).IsRequired();

            entity.HasOne<AccountPlan>()
                .WithMany()
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}