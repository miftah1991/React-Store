using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet(Name ="GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetriveBasket();

            if (basket == null) return NotFound();

            return MapBasketToDto(basket);
        }

        private BasketDto MapBasketToDto(Basket basket)
        {
             return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
             }).ToList(),
            };
        }
        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            // get basket || create basekt
            var basket = await RetriveBasket();

            // create basket
            if (basket == null) basket = CreateBasket();
          //  get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = "Product not found" });
           // add item
            basket.AddItem(product, quantity);
            //save changes
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetBasket", MapBasketToDto(basket));

            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });

        }

        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOption = new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.Now.AddDays(30),
                //  HttpOnly=true,
            };
            Response.Cookies.Append("buyerId", buyerId, cookieOption);
            var basket = new Basket { BuyerId = buyerId };
            _context.Add(basket);
            return basket;

        }


        private async Task<Basket> RetriveBasket()
        {
            return await _context.Baskets
              .Include(i => i.Items)
              .ThenInclude(p => p.Product)
              .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }


        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItems(int productId, int quantity)
        {
            //get basket
            var basket= await RetriveBasket();
            if (basket == null) return NotFound();
            //remove item or reduce quanitty
            basket.RemoveItem(productId, quantity);
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Error in removing item" });
        }

    }
}
