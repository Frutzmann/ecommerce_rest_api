using quest_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace quest_web.Repository
{
    public interface IUserRepository
    {
        public UserRepository.QueryResult AddUser(User user);
        public UserDetails GetUserDetails(string username);
    }
    public class UserRepository : IUserRepository
    {
        private readonly APIDbContext _context;
        public UserRepository(APIDbContext dbContext)
        {
              _context = dbContext; 
        }

        public enum QueryResult
        {
            ALL_FIELDS_NOT_COMPLETE,
            USERNAME_TAKEN,
            SUCCESS
        }

        public QueryResult AddUser(User user)
        {
            User _user = new() { Username = user.Username, Password = user.Password, Role = user.Role };
            _user.updatedDate = DateTime.Now; 

            // Default to role user
            if (string.IsNullOrEmpty(_user.Role.ToString()) == true)
            {
                _user.Role = UserRole.ROLE_USER;
            }

            // Check if already used
            var isUsed = _context.User.FirstOrDefault(u => u.Username == user.Username);
            if (string.IsNullOrEmpty(_user.Username)  == true || string.IsNullOrEmpty(_user.Password) == true)
            {
                return QueryResult.ALL_FIELDS_NOT_COMPLETE;
            } else if(isUsed != null)
            {
                return QueryResult.USERNAME_TAKEN;
            } else
            {

                _user.Password = BCrypt.Net.BCrypt.HashPassword(_user.Password);
                _context.User.Add(_user);
                _context.SaveChanges();
                return QueryResult.SUCCESS;
            }
        }

        public List<User> GetUsers()
        {
            return _context.User.ToList();
        }

        public User GetUserById(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.Id == id);

            return user;
        }

        public User UpdateUser(User user)
        {
            var currentUser = _context.User.FirstOrDefault(u => u.Id == user.Id);
            if (currentUser == null)
            {
                return null;
            }

            if (user.Role == null || user.Username == null)
            {
                return null;
            }

            currentUser.Role = user.Role;
            currentUser.Username = user.Username;

            _context.Update(currentUser);
            _context.SaveChanges();
            return currentUser;
        }

        public bool DeleteUser(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            _context.Remove(user);
            _context.SaveChanges();

            return true;
        }

        public UserDetails GetUserDetails(string username)
        {
            User userFound = _context.User.FirstOrDefault(u => u.Username == username);
            UserDetails infoToReturn = new UserDetails { Id = userFound.Id, Username = userFound.Username, Role = userFound.Role };
            return infoToReturn;
        }
    }
}
