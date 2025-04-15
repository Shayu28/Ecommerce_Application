using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.exception
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() : base("Product not found.") { }
        public ProductNotFoundException(string message) : base(message) { }
    }
}
