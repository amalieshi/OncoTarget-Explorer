using Microsoft.EntityFrameworkCore;

namespace OncoTargetExplorer.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ShortlistItem> ShortlistItems => Set<ShortlistItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortlistItem>()
            .HasIndex(item => item.Accession)
            .IsUnique();
    }
}
