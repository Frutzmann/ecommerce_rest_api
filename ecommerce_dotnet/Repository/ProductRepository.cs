using quest_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace quest_web.Repository
{
    public interface IProductRepository
    {
        public List<Product> GetProducts();
        public Product GetProductById(int id);
        public bool DeleteProduct(int id);
        public Product CreateProduct(Product product); 
        public Product UpdateProduct(Product product);

        
    }
    public class ProductRepository : IProductRepository
    {
        private readonly APIDbContext _context;
        public ProductRepository(APIDbContext context)
        {
            _context = context;
        }
        public Product CreateProduct(Product product)
        {
            try
            {
                var newProduct = new Product() { title = product.title, description = product.description, link = product.link, price = product.price, currency = product.currency };
                _context.Product.Add(newProduct);
                _context.SaveChanges();

                return newProduct;
            } catch (Exception e)
            {
                return null;
            }
           
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var product = _context.Product.FirstOrDefault(x => x.id == id);
                if (product == null)
                    return false;
                _context.Product.Remove(product);
                _context.SaveChanges();

            } catch (Exception e)
            {
                return false;
            }
            

            

            return true;
        }

        public Product GetProductById(int id)
        {
            var product = _context.Product.FirstOrDefault(x => x.id == id);
            return product;
        }

        public List<Product> GetProducts()
        {
            var temp2 = _context.Product;
            var temp = _context.Product.ToList();
            return temp;    
        }

        public Product UpdateProduct(Product product)
        {
            var currentProduct = _context.Product.FirstOrDefault(x => x.id == product.id);
            if(currentProduct == null)
                return null;
            
            if(product.title != null && currentProduct.title != product.title)
                currentProduct.title = product.title;

            if(product.description != null && currentProduct.description != product.description)
                currentProduct.description = product.description;

            if(product.price != 0 && currentProduct.price != product.price)
                currentProduct.price = product.price;

            if(product.currency != null && currentProduct.currency != product.currency)
                currentProduct.currency = product.currency;

            if(product.link != null && currentProduct.link != product.link)
                currentProduct.link = product.link;

            _context.Product.Update(currentProduct);
            _context.SaveChanges();

            return currentProduct;

            
        }
    }
}
