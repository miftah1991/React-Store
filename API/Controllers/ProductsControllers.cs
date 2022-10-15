using API.Data;
using API.Entities;
using API.Extensions;
using API.RequestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    
    public class ProductsControllers : BaseApiController
    {
        private readonly StoreContext _context;
        public ProductsControllers(StoreContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts(
           [FromQuery]ProductParams productParams)
        {
            var query =  _context.Products
                .Sort(productParams.OrderBy)
                .Search(productParams.SearchTerm)
                .Filter(productParams.Brands,productParams.Types)
                .AsQueryable();
            var products = await PagedList<Product>.ToPagedList(query,
                productParams.PageNumber,
                productParams.PageSize);
            Response.AddPaginationHeader(products.MetaData);
            return products;
        }
        //public ActionResult<List<Product>> GetProducts()
        //{
        //    var products = context.Products.ToList();
        //    return Ok(products);
        //}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return product;
        }
        //public ActionResult<Product> GetProduct(int id)
        //{
        //    return context.Products.Find(id);
        //}
 

    }
}
