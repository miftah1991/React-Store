using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class StoreContext :DbContext
    {
       public StoreContext(DbContextOptions opetions) :base(opetions)
       {
       }
       public DbSet<Product> Products { get; set; }
       public DbSet<Basket> Baskets { get; set; }
    }
}
