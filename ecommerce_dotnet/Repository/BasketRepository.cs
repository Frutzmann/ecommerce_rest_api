using Microsoft.EntityFrameworkCore;
using quest_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace quest_web.Repository
{
    public interface IBasketRepository
    {
        public List<Basket> GetBasketByUserId(User id); 
        public Basket UpdateBasket(Basket basket);
        public bool DeleteBasket(int id);
        public Basket CreateBasket(Basket basket);
        public Basket GetBasketById(int id);
        public bool SetBasketOrderNumber(Basket basket);

        public List<Basket> GetBasketByOrderNumber(int number); 
    }
    public class BasketRepository : IBasketRepository
    {
        private readonly APIDbContext _context;

        public BasketRepository(APIDbContext context)
        {
            _context = context;
        }
        public Basket CreateBasket(Basket basket)
        {
            try
            {
                
                var checkProduct = _context.Product.Where(x => x.id == basket.Product.id).FirstOrDefault();
                if(checkProduct == null)
                {
                    return null; 
                }
                var _basket = new Basket()
                {
                    User = basket.User,
                    Product = checkProduct,
                    quantity = basket.quantity
                };

                _context.Basket.Add(_basket);
                _context.SaveChanges();

                return basket;
            } catch (Exception e)
            {
                return null;
            }
          
        }

        public bool DeleteBasket(int id)
        {
            var basket = _context.Basket.FirstOrDefault(x => x.id == id);

            if (basket == null)
                return false;

            _context.Basket.Remove(basket);
            _context.SaveChanges();
            return true; 

        }

        public List<Basket> GetBasketByUserId(User user)
        {
            var basket = _context.Basket.Where(x => x.User == user && x.orderNumber == 0).Include(x => x.Product).ToList();
            if (basket == null)
                return null;
            return basket;
        }

        public Basket UpdateBasket(Basket basket)
        {
            var currentBasket = _context.Basket.FirstOrDefault(x => x.id == basket.id);
            _context.Basket.Update(currentBasket);
            _context.SaveChanges();

            return currentBasket;
        }
        
        public Basket GetBasketById(int id)
        {
            var _basket = _context.Basket.FirstOrDefault(x => x.id == id);
            if (_basket == null)
                return null;
            return _basket;
        }

        public bool SetBasketOrderNumber(Basket basket)
        {
            var _currentBaskets = _context.Basket.Where(x => x.id ==basket.id).Include(x => x.Product).FirstOrDefault();
            if (_currentBaskets == null)
                return false;

            _currentBaskets.orderNumber = basket.orderNumber;
            _context.Basket.Update(_currentBaskets);
            _context.SaveChanges();

            return true;
        }

        public List<Basket> GetBasketByOrderNumber(int number)
        {
            var basket = _context.Basket.Where(x => x.orderNumber == number).Include(x => x.Product).ToList();
            if (basket.Count == 0)
                return null;
            return basket;
        }

    }
}
