using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quest_web.Models;

namespace quest_web_tests.MockData
{
    internal class MockedOrders
    {
        public List<Order> MyOrders;

        public MockedOrders()
        {
        
            MyOrders = new List<Order>() {
                new Order { id = 1, totalPrice = 1000, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18")},
                new Order { id = 2, totalPrice = 1500, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18")},
                new Order { id = 3, totalPrice = 2000, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18")},
            };

        }

    }
}
