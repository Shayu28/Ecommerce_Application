using ECommerceApp.dao;
using ECommerceApp.entity;
using ECommerceApp.exception;
using ECommerceApp.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace ECommerceApp.main
{
    public class EcomApp
    {
        private static IOrderProcessorRepository orderProcessor = new OrderProcessorRepositoryImpl();

        public static IOrderProcessorRepository OrderProcessor { get => orderProcessor; set => orderProcessor = value; }

        public static void Main(string[] args)
        {
            
            try
            {
                bool running = true;
                while (running)
                {
                    Console.WriteLine("Application started");
                    Console.WriteLine("\n===== E-Commerce Application Menu =====");
                    Console.WriteLine("1. Register Customer");
                    Console.WriteLine("2. Create Product");
                    Console.WriteLine("3. Delete Product");
                    Console.WriteLine("4. Add to Cart");
                    Console.WriteLine("5. View Cart");
                    Console.WriteLine("6. Place Order");
                    Console.WriteLine("7. View Customer Orders");
                    Console.WriteLine("8. Delete Customer");
                    Console.WriteLine("9. Exit");
                    Console.Write("Enter your choice: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            RegisterCustomer();
                            break;
                        case 2:
                            CreateProduct();
                            break;
                        case 3:
                            DeleteProduct();
                            break;
                        case 4:
                            AddToCart();
                            break;
                        case 5:
                            ViewCart();
                            break;
                        case 6:
                            PlaceOrder();
                            break;
                        case 7:
                            ViewCustomerOrders();
                            break;
                        case 8:
                            DeleteCustomer();
                            break;
                        case 9:
                            running = false;
                            Console.WriteLine("Exiting application...");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Details: {ex.InnerException.Message}");
                }
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        private static void RegisterCustomer()
        {
            Console.WriteLine("\n--- Register Customer ---");
            Console.Write("Enter name: ");
            string name = Console.ReadLine();

            Console.Write("Enter email: ");
            string email = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            Customer customer = new Customer(name, email, password);
            bool success = OrderProcessor.CreateCustomer(customer);

            if (success)
            {
                Console.WriteLine("Customer registered successfully!");
            }
            else
            {
                Console.WriteLine("Failed to register customer.");
            }
        }

        private static void CreateProduct()
        {
            Console.WriteLine("\n--- Create Product ---");
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();

            Console.Write("Enter price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price. Please enter a valid number.");
                return;
            }

            Console.Write("Enter description (optional): ");
            string description = Console.ReadLine();

            Console.Write("Enter stock quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int stockQuantity))
            {
                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                return;
            }

            Product product = new Product(name, price, description, stockQuantity);
            bool success = OrderProcessor.CreateProduct(product);

            if (success)
            {
                Console.WriteLine("Product created successfully!");
            }
            else
            {
                Console.WriteLine("Failed to create product.");
            }
        }

        private static void DeleteProduct()
        {
            Console.WriteLine("\n--- Delete Product ---");
            Console.Write("Enter product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID. Please enter a valid number.");
                return;
            }

            try
            {
                bool success = OrderProcessor.DeleteProduct(productId);
                if (success)
                {
                    Console.WriteLine("Product deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to delete product.");
                }
            }
            catch (ProductNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void AddToCart()
        {
            Console.WriteLine("\n--- Add to Cart ---");
            Console.Write("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            Console.Write("Enter product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID. Please enter a valid number.");
                return;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                return;
            }

            try
            {
                Customer customer = new Customer { CustomerId = customerId };
                Product product = new Product { ProductId = productId };
                bool success = OrderProcessor.AddToCart(customer, product, quantity);

                if (success)
                {
                    Console.WriteLine("Product added to cart successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to add product to cart.");
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (ProductNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ViewCart()
        {
            Console.WriteLine("\n--- View Cart ---");
            Console.Write("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            try
            {
                Customer customer = new Customer { CustomerId = customerId };
                List<Product> cartItems = OrderProcessor.GetAllFromCart(customer);

                if (cartItems.Count == 0)
                {
                    Console.WriteLine("Your cart is empty.");
                    return;
                }

                Console.WriteLine("\nItems in your cart:");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("ID\tName\t\tPrice\t\tDescription");
                Console.WriteLine("------------------------------------------------------------");

                foreach (Product product in cartItems)
                {
                    Console.WriteLine($"{product.ProductId}\t{product.Name}\t{product.Price:C}\t{product.Description}");
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void PlaceOrder()
        {
            Console.WriteLine("\n--- Place Order ---");
            Console.Write("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            try
            {
                Customer customer = new Customer { CustomerId = customerId };
                List<Product> cartItems = OrderProcessor.GetAllFromCart(customer);

                if (cartItems.Count == 0)
                {
                    Console.WriteLine("Your cart is empty. Cannot place order.");
                    return;
                }

                Console.WriteLine("\nYour cart items:");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("ID\tName\t\tPrice\t\tDescription");
                Console.WriteLine("------------------------------------------------------------");

                foreach (Product product in cartItems)
                {
                    Console.WriteLine($"{product.ProductId}\t{product.Name}\t{product.Price:C}\t{product.Description}");
                }

                Console.Write("\nEnter shipping address: ");
                string shippingAddress = Console.ReadLine();

                // Create product-quantity dictionary (assuming quantity is 1 for each)
                // Note: In a real application, you'd need to track quantities properly
                Dictionary<Product, int> products = new Dictionary<Product, int>();
                foreach (Product product in cartItems)
                {
                    products.Add(product, 1); // Default quantity to 1
                }

                bool success = OrderProcessor.PlaceOrder(customer, products, shippingAddress);
                if (success)
                {
                    Console.WriteLine("Order placed successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to place order.");
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing order: {ex.Message}");
            }
        }

        private static void ViewCustomerOrders()
        {
            Console.WriteLine("\n--- View Customer Orders ---");
            Console.Write("Enter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            try
            {
                List<Dictionary<Product, int>> orders = OrderProcessor.GetOrdersByCustomer(customerId);

                if (orders.Count == 0)
                {
                    Console.WriteLine("No orders found for this customer.");
                    return;
                }

                Console.WriteLine($"\nOrders for customer ID {customerId}:");
                for (int i = 0; i < orders.Count; i++)
                {
                    Console.WriteLine($"\nOrder #{i + 1}:");
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine("Product\t\tQuantity\tPrice\t\tTotal");
                    Console.WriteLine("------------------------------------------------------------");

                    decimal orderTotal = 0;
                    foreach (var item in orders[i])
                    {
                        decimal itemTotal = item.Key.Price * item.Value;
                        orderTotal += itemTotal;
                        Console.WriteLine($"{item.Key.Name}\t{item.Value}\t\t{item.Key.Price:C}\t{itemTotal:C}");
                    }

                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine($"Order Total: {orderTotal:C}");
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteCustomer()
        {
            Console.WriteLine("\n--- Delete Customer ---");
            Console.Write("Enter customer ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                return;
            }

            try
            {
                bool success = OrderProcessor.DeleteCustomer(customerId);
                if (success)
                {
                    Console.WriteLine("Customer deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to delete customer.");
                }
            }
            catch (CustomerNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
