using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quest_web.Models;

namespace quest_web_tests.MockData
{
    internal class MockedProducts
    {
        public List<Product> MyProducts;

        public MockedProducts()
        {
            MyProducts = new List<Product>
            {
                new Product { id = 1, title = "Iphone 14 Pro Max", description = "Infiniment Pro", price = 1500, currency = "€",creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Product { id = 2, title = "Iphone 13 Pro Max", description = "Definition du Pro", price =1200, currency = "€", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Product { id = 3, title = "Iphone 12 Pro Max", description = "Pro", price = 900, currency = "€", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
              
            };

        }

    }
}
