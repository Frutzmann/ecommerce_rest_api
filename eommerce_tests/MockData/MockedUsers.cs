using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quest_web.Models;

namespace quest_web_tests.MockData
{
    internal class MockedUsers
    {
        public List<User> MyUsers;

        public MockedUsers()
        {
            MyUsers = new List<User>()
            {
                new User { Id = 1, Username = "BryanSimon", Password = "Bryan", Role = UserRole.ROLE_ADMIN, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new User { Id = 2, Username = "BryanSimon2", Password = "Bryan", Role = UserRole.ROLE_ADMIN, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new User { Id = 3, Username = "BryanSimon3", Password = "Bryan", Role = UserRole.ROLE_USER, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new User { Id = 4, Username = "BryanSimon4", Password = "Bryan", Role = UserRole.ROLE_USER, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },
                new User { Id = 5, Username = "BryanSimon5", Password = "Bryan", Role = UserRole.ROLE_ADMIN, creationDate = DateTime.Parse("2022-05-18"), updatedDate = DateTime.Parse("2022-05-18") },

        };
        }
    }
}
