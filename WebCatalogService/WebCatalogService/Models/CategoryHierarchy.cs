using System.Collections.Generic;

namespace WebCatalogService.Models
{
    public class CategoryHierarchy
    {
        public Category Parent { get; set; }
        public List<Category> Childrem { get; set; }
    }
}
