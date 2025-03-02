using Microsoft.EntityFrameworkCore;

using Edam.Data.CatalogModel;

namespace Edam.Data.CatalogDb;

public class CatalogContext : DbContext
{

   public DbSet<ContentTypeInfo> ContentTypes { get; set; }
   public DbSet<ContainerInfo> Containers { get; set; }
   public DbSet<ItemInfo> Items { get; set; }
   public DbSet<ItemDataInfo> DataItems { get; set; }

   public string ConnectionString { get; }

   public CatalogContext()
   {
      //ConnectionString = Configuration["Connnectionstrings:MyConnection"];
      //ConnectionString = LexiconContextHelper.GetConnectionString();
   }

   public CatalogContext(string connectionString)
   {
      ConnectionString = connectionString;
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      // Configure default schema
      modelBuilder.HasDefaultSchema("Catalog");
   }

   // Configures EF to create an SQL database using given connection string
   protected override void OnConfiguring(DbContextOptionsBuilder options)
   {
      options.UseSqlServer(ConnectionString);
   }

}
