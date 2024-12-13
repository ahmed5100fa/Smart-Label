using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLabel.labelData;
using SmartLabel.models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLabel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public CategoriesController(AppDbContext db)
        {
            _db = db;
        }

        // POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryModel catModel)
        {
            if (catModel == null)
            {
                return BadRequest("Category data is required.");
            }

            var category = new Category
            {
                Name = catModel.Name,
                Descount = 0, // You can set a default value if Descount is not provided in CategoryModel
            };

            if (catModel.Image != null)
            {
                var fileName = Path.GetFileName(catModel.Image.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                // Ensure the images directory exists
                Directory.CreateDirectory(_imagePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await catModel.Image.CopyToAsync(fileStream);
                }

                // Save the file path or file name in the Category model
                category.ImagePath = filePath; // You can save only the filename if you prefer
            }

            if (ModelState.IsValid)
            {
                await _db.Categories.AddAsync(category);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // GET: api/Category
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _db.Categories.ToList();

            if (categories == null || !categories.Any())
            {
                return NotFound("No Categories found.");
            }

            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return Ok(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryModel catModel)
        {
            if (catModel == null || id <= 0)
            {
                return BadRequest("Category data is invalid.");
            }

            var existingCategory = await _db.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            existingCategory.Name = catModel.Name;
            existingCategory.Descount = 0; // Default value for Descount if not provided in CategoryModel

            // Handle image update (optional)
            if (catModel.Image != null)
            {
                var fileName = Path.GetFileName(catModel.Image.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                // Ensure the images directory exists
                Directory.CreateDirectory(_imagePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await catModel.Image.CopyToAsync(fileStream);
                }

                // Save the file path or file name in the Category model
                existingCategory.ImagePath = filePath; // You can store only the filename if you prefer
            }

            try
            {
                _db.Categories.Update(existingCategory);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Categories.Any(c => c.Id == id))
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
