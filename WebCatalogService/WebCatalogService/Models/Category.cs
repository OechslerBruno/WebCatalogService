using System;
using System.ComponentModel.DataAnnotations;

namespace WebCatalogService.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Parent { get; set; }
    }
}
