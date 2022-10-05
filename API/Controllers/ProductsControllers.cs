using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsControllers : ControllerBase
    {
        private readonly StoreContext _context;
        public ProductsControllers(StoreContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return  await _context.Products.ToListAsync();
        }
        //public ActionResult<List<Product>> GetProducts()
        //{
        //    var products = context.Products.ToList();
        //    return Ok(products);
        //}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        //public ActionResult<Product> GetProduct(int id)
        //{
        //    return context.Products.Find(id);
        //}
    }
}
