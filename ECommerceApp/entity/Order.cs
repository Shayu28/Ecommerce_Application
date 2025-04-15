using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApp.entity
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }

        public Order() { }

        public Order(int customerId, decimal totalPrice, string shippingAddress)
        {
            CustomerId = customerId;
            TotalPrice = totalPrice;
            ShippingAddress = shippingAddress;
            OrderDate = DateTime.Now;
        }
    }
}
