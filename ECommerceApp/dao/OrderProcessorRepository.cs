using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.entity;

namespace ECommerceApp.dao
{
    public interface IOrderProcessorRepository
    {
        bool CreateProduct(Product product);
        bool CreateCustomer(Customer customer);
        bool DeleteProduct(int productId);
        bool DeleteCustomer(int customerId);
        bool AddToCart(Customer customer, Product product, int quantity);
        bool RemoveFromCart(Customer customer, Product product);
        List<Product> GetAllFromCart(Customer customer);
        bool PlaceOrder(Customer customer, Dictionary<Product, int> products, string shippingAddress);
        List<Dictionary<Product, int>> GetOrdersByCustomer(int customerId);
    }
}
