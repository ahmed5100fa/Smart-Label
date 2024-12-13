using Microsoft.AspNetCore.Authorization;
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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Product
        [HttpGet]
        [Authorize]  // Authorization for all authenticated users
        public async Task<IActionResult> GetProducts()
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        [Authorize]  // Authorization for all authenticated users
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only Admin role can add products
        public async Task<IActionResult> AddProduct([FromForm] ProductModel productModel)
        {
            if (productModel == null)
                return BadRequest("Product data is null");

            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == productModel.CategoryId);
            if (!categoryExists)
                return NotFound("Category not found");

            // Map ProductModel to Product
            var product = new Product
            {
                Name = productModel.Name,
                Price = productModel.Price,
                Discount = productModel.Discount,
                ExpirationDate = productModel.ExpirationDate,
                CategoryId = productModel.CategoryId
            };

            if (productModel.Image != null)
            {
                var fileName = Path.GetFileName(productModel.Image.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                // Ensure the images directory exists
                Directory.CreateDirectory(_imagePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productModel.Image.CopyToAsync(fileStream);
                }

                // Save the file path or file name in the Product model
                product.ImagePath = filePath; // You can save only the filename if you prefer
            }

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin role can update products
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductModel productModel)
        {
            if (productModel == null)
                return BadRequest("Product data is null");

            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");

            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == productModel.CategoryId);
            if (!categoryExists)
                return NotFound("Category not found");

            // Update the product properties
            product.Name = productModel.Name;
            product.Price = productModel.Price;
            product.Discount = productModel.Discount;
            product.ExpirationDate = productModel.ExpirationDate;
            product.CategoryId = productModel.CategoryId;

            if (productModel.Image != null)
            {
                var fileName = Path.GetFileName(productModel.Image.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                // Ensure the images directory exists
                Directory.CreateDirectory(_imagePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productModel.Image.CopyToAsync(fileStream);
                }

                // Save the file path or file name in the Product model
                product.ImagePath = filePath; // You can save only the filename if you prefer
            }

            try
            {
                _db.Products.Update(product);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Products.Any(p => p.Id == id))
                    return NotFound("Product not found");
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin role can delete products
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
                return NotFound("Product not found");

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
