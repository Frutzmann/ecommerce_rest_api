using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quest_web.Models;

namespace quest_web_tests.MockData
{
    internal class MockedAddresses
    {
        public List<Address> MyAddresses;

        public MockedAddresses()
        {
            MyAddresses = new List<Address>
            {
                new Address { id = 1, road = "6 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France",creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Address { id = 2, road = "7 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Address { id = 3, road = "8 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Address { id = 4, road = "9 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France",creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Address { id = 5, road = "10 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new Address { id = 6, road = "11 Rue Claudius Perillat", postalCode = "73200", city = "Albertville", country = "France", creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
            };

        }

    }
}
