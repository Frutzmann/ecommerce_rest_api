using System;
using System.Collections.Generic;
using System.Linq;
using quest_web.Models;

namespace quest_web.Repository
{
    public interface IAddressRepository
    {
        public List<Address> GetAddress();
        public Address GetAddressById(int id);

        public List<Address> GetAddressByUser(User user);
        public bool DeleteAddress(int id);
        public Address UpdateAddress(Address address);
        public Address CreateAddress(Address address);
    }
    public class AddressRepository : IAddressRepository
    {
        private readonly APIDbContext _context;
        
        public AddressRepository(APIDbContext context)
        {
            _context = context; 
        }

        public List<Address> GetAddress() 
        {
            return _context.Address.ToList();
        }

        public Address GetAddressById(int id)
        {
            var address = _context.Address.FirstOrDefault(a => a.id == id);
            return address; 
        }

        public bool DeleteAddress(int id)
        {
            var address = _context.Address.FirstOrDefault(a => a.id == id);

            if (address == null)
                return false;

            _context.Remove(address);
            _context.SaveChanges();

            return true; 
            }

        public Address UpdateAddress(Address address)
        {
            var currentAddress = _context.Address.FirstOrDefault(a => a.id == address.id);
            if (currentAddress == null)
                return null;

            if(address.road != null && currentAddress.road != address.road)
                currentAddress.road = address.road;

            if(address.postalCode != null && currentAddress.postalCode != address.postalCode)
                currentAddress.postalCode = address.postalCode;

            if(address.city != null && currentAddress.city != address.city)
                currentAddress.city = address.city;

            if(address.country != null && currentAddress.country != address.country)
                currentAddress.country = address.country;

            _context.Update(currentAddress);
            _context.SaveChanges();
            return currentAddress; 
        }

        public Address CreateAddress(Address address)
        {
            var _address = new Address { User = address.User, road = address.road, postalCode = address.postalCode, city = address.city, country = address.country };
            _context.Address.Add((_address));
            _context.SaveChanges();

            return _address; 
        }

        public List<Address> GetAddressByUser(User user)
        {
            var _address = _context.Address.Where(x => x.User == user).ToList();

            if (_address == null)
                return null;

            return _address;
        }

    }
}
