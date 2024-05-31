using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly CategoryAPIDbContext dbContext;
        public CategoriesController(CategoryAPIDbContext dbContext)
        {
            this.dbContext = dbContext;

        }


        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await dbContext.Categories.ToListAsync());
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetCategories([FromRoute] long id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryRequest addCategoryRequest)
        {
            var category = new Category()
            {

                Name = addCategoryRequest.Name,
  
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return Ok(category);
        }



        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] long id, UpdateCategoryRequest updateCategoryRequest)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category != null)
            {

                category.Name = updateCategoryRequest.Name;
 

                await dbContext.SaveChangesAsync();
                return Ok(category);

            }

            return NotFound();

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] long id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            dbContext.Remove(category);
            await dbContext.SaveChangesAsync();
            return Ok(category);
        }

    }


}
