using CadflairEntityFrameworkDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CadflairEntityFrameworkDataAccess.Data;

public class CadflairEntityFrameworkDataAccessContext : DbContext
{
    public DbSet<Attachment> Attachments { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Column> Columns { get; set; } = null!;
    public DbSet<TableValue> TableValues { get; set; } = null!;
    public DbSet<Row> Rows { get; set; } = null!;
    public DbSet<ProductDefinition> ProductDefinitions { get; set; } = null!;
    public DbSet<ProductTable> ProductTables { get; set; } = null!;
    public DbSet<Thumbnail> Thumbnails { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // remove plural table names
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            if (entity.BaseType == null)
                entity.SetTableName(entity.DisplayName());
        }
    }

}
