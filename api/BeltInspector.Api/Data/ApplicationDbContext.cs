using BeltInspector.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BeltInspector.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<FileRecord> Files => Set<FileRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(50);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.Property(x => x.UpdatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        });

        modelBuilder.Entity<FileRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(200);
            entity.Property(x => x.StorageKey).IsRequired();
            entity.HasOne(x => x.Inspection)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
