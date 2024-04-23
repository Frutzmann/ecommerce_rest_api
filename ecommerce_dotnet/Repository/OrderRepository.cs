using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using quest_web.Models;

namespace quest_web.Repository
{
    public interface IOrderRepository
    {
        public Order CreateOrder(Order order);
        public bool DeleteOrder(int id);

        public List<Order> GetOrdersByUser(User user);

        public Order GetOrderById(int id);

        public List<Order> GetOrders();
    }
    public class OrderRepository : IOrderRepository
    {
        private readonly APIDbContext _context;

        public OrderRepository(APIDbContext context)
        {
            _context = context; 
        }
        public Order CreateOrder(Order order)
        {
            try
            {
                var checkAddress = _context.Address.Where(x => x.id == order.address.id).FirstOrDefault();
                if (checkAddress == null) return null;

                var newOrder = new Order()
                {
                    user = order.user,
                    address = checkAddress,
                    orderNumber = order.orderNumber,
                    totalPrice = order.totalPrice,
                };
                _context.Order.Add(newOrder);
                _context.SaveChanges();
                return newOrder;
            } catch (System.Exception e) {
                return null;
            }
            
        }

        public bool DeleteOrder(int id)
        {
            try
            {
                var order = _context.Order.FirstOrDefault(x => x.id == id);
                if (order == null)
                    return false;

                _context.Order.Remove(order);
                _context.SaveChanges();
                return true;

            }catch(Exception e)
            {
                return false;
            }
           

        }

        public List<Order> GetOrders()
        {
            return _context.Order.Include(x => x.address).ToList();
        }

        public List<Order> GetOrdersByUser(User usr)
        {
            return _context.Order.Where(x => x.user == usr).Include(x => x.address).ToList();
        }

        public Order GetOrderById(int id) {
            var _order = _context.Order.Where(x => x.id == id).Include(x => x.address).FirstOrDefault();

            if (_order == null)
                return null;
            return _order;
            
        }
    }
}
