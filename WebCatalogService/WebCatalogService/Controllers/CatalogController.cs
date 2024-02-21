using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCatalogService.Infra;
using WebCatalogService.Models;

namespace WebCatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogController(CatalogContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory(string jsonCategory)
        {
            //var postcardsA = JsonConvert.DeserializeObject(jsonCategory);
            //Dictionary<string, string> postcards = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonCategory);
            Dictionary<string, List<string>> postcards = ParseJsonWithDuplicateKeys(jsonCategory);

            Category newCategory;

            foreach (var kvp in postcards)
            {
                var category = await FindCategoryByName(kvp.Key);

                //Se categoria pai nao existe, inclui
                if (category == null)
                {
                    _context.Categories.Add(new Category()
                    {
                        Id = Guid.NewGuid(),
                        Name = kvp.Key
                    }) ;
                    _context.SaveChanges();
                }

                //Garantindo que ctegoria pai existe, inclui categoria filho
                foreach (var item in kvp.Value)
                {
                    var result = await FindCategoryByName(item);

                    if(result == null)
                    {
                        newCategory = new Category()
                        {
                            Id = Guid.NewGuid(),
                            Name = item,
                            Parent = kvp.Key
                        };
                        _context.Categories.Add(newCategory);
                    }
                    
                }
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesHierarchy()
        {
            var result = await _context.Categories.AsNoTracking().OrderBy(x => x.Parent).ToListAsync();

            List<CategoryHierarchy> categoriesHierarchy = new List<CategoryHierarchy>();

            foreach (var item in result)
            {
                var children = result.FindAll(x => x.Parent == item.Name);
                CategoryHierarchy categoryHierarchy = new CategoryHierarchy();
                categoryHierarchy.Parent = item;
                categoryHierarchy.Childrem = children;

                categoriesHierarchy.Add(categoryHierarchy);
            }
            
            var jsonResult = JsonConvert.SerializeObject(categoriesHierarchy);
            return Ok(jsonResult);
        }

        [NonAction]
        public static Dictionary<string, List<string>> ParseJsonWithDuplicateKeys(string json)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            string pattern = @"""(.*?)"":\s*""(.*?)""";

            MatchCollection matches = Regex.Matches(json, pattern);

            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = new List<string>();
                }

                dictionary[key].Add(value);
            }

            return dictionary;
        }

        [NonAction]
        public async Task<Category> FindCategoryByName(string categoryName)
        {
            var result = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == categoryName);

            return result;
        }
    }
}
