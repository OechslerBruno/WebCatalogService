using Microsoft.EntityFrameworkCore;
using WebCatalogService.Models;

namespace WebCatalogService.Infra
{
    public class CatalogContext : DbContext
    {
        public  DbSet<Category> Categories { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {

        }
    }
}
