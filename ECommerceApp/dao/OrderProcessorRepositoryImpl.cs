using ECommerceApp.entity;
using ECommerceApp.exception;
using ECommerceApp.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ECommerceApp.dao
{
    public class OrderProcessorRepositoryImpl : IOrderProcessorRepository
    {
        private string connectionString;

        public OrderProcessorRepositoryImpl()
        {
            connectionString = DBPropertyUtil.GetConnectionString("db.properties");
        }

        public bool CreateProduct(Product product)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                string query = "INSERT INTO products (name, price, description, stockQuantity) VALUES (@Name, @Price, @Description, @StockQuantity)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool CreateCustomer(Customer customer)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                string query = "INSERT INTO customers (name, email, password) VALUES (@Name, @Email, @Password)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", customer.Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Password", customer.Password);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteProduct(int productId)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // First check if product exists
                string checkQuery = "SELECT COUNT(*) FROM products WHERE product_id = @ProductId";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@ProductId", productId);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        throw new ProductNotFoundException($"Product with ID {productId} not found.");
                    }
                }

                string query = "DELETE FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteCustomer(int customerId)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // First check if customer exists
                string checkQuery = "SELECT COUNT(*) FROM customers WHERE customer_id = @CustomerId";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CustomerId", customerId);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        throw new CustomerNotFoundException($"Customer with ID {customerId} not found.");
                    }
                }

                string query = "DELETE FROM customers WHERE customer_id = @CustomerId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool AddToCart(Customer customer, Product product, int quantity)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // Check if customer exists
                if (!CustomerExists(customer.CustomerId))
                {
                    throw new CustomerNotFoundException($"Customer with ID {customer.CustomerId} not found.");
                }

                // Check if product exists
                if (!ProductExists(product.ProductId))
                {
                    throw new ProductNotFoundException($"Product with ID {product.ProductId} not found.");
                }

                // Check if product is already in cart
                string checkQuery = "SELECT COUNT(*) FROM cart WHERE customer_id = @CustomerId AND product_id = @ProductId";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    checkCommand.Parameters.AddWithValue("@ProductId", product.ProductId);
                    int count = (int)checkCommand.ExecuteScalar();

                    string query;
                    if (count > 0)
                    {
                        // Update quantity if product already in cart
                        query = "UPDATE cart SET quantity = quantity + @Quantity WHERE customer_id = @CustomerId AND product_id = @ProductId";
                    }
                    else
                    {
                        // Insert new cart item
                        query = "INSERT INTO cart (customer_id, product_id, quantity) VALUES (@CustomerId, @ProductId, @Quantity)";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                        command.Parameters.AddWithValue("@ProductId", product.ProductId);
                        command.Parameters.AddWithValue("@Quantity", quantity);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
        }

        public bool RemoveFromCart(Customer customer, Product product)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // Check if customer exists
                if (!CustomerExists(customer.CustomerId))
                {
                    throw new CustomerNotFoundException($"Customer with ID {customer.CustomerId} not found.");
                }

                // Check if product exists
                if (!ProductExists(product.ProductId))
                {
                    throw new ProductNotFoundException($"Product with ID {product.ProductId} not found.");
                }

                string query = "DELETE FROM cart WHERE customer_id = @CustomerId AND product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    command.Parameters.AddWithValue("@ProductId", product.ProductId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new ProductNotFoundException($"Product with ID {product.ProductId} not found in customer's cart.");
                    }
                    return rowsAffected > 0;
                }
            }
        }

        public List<Product> GetAllFromCart(Customer customer)
        {
            List<Product> cartProducts = new List<Product>();

            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // Check if customer exists
                if (!CustomerExists(customer.CustomerId))
                {
                    throw new CustomerNotFoundException($"Customer with ID {customer.CustomerId} not found.");
                }

                string query = @"SELECT p.product_id, p.name, p.price, p.description, p.stockQuantity, c.quantity 
                               FROM cart c 
                               JOIN products p ON c.product_id = p.product_id 
                               WHERE c.customer_id = @CustomerId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                Name = reader["name"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                Description = reader["description"].ToString(),
                                StockQuantity = Convert.ToInt32(reader["stockQuantity"])
                            };
                            cartProducts.Add(product);
                        }
                    }
                }
            }

            return cartProducts;
        }

        public bool PlaceOrder(Customer customer, Dictionary<Product, int> products, string shippingAddress)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // Check if customer exists
                if (!CustomerExists(customer.CustomerId))
                {
                    throw new CustomerNotFoundException($"Customer with ID {customer.CustomerId} not found.");
                }

                // Calculate total price
                decimal totalPrice = 0;
                foreach (var item in products)
                {
                    totalPrice += item.Key.Price * item.Value;
                }

                // Start transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // 1. Insert into orders table
                    string orderQuery = @"INSERT INTO orders (customer_id, total_price, shipping_address) 
                                        VALUES (@CustomerId, @TotalPrice, @ShippingAddress);
                                        SELECT SCOPE_IDENTITY();";

                    int orderId;
                    using (SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction))
                    {
                        orderCommand.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                        orderCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        orderCommand.Parameters.AddWithValue("@ShippingAddress", shippingAddress);

                        orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                    }

                    // 2. Insert into order_items table
                    string orderItemsQuery = @"INSERT INTO order_items (order_id, product_id, quantity) 
                                              VALUES (@OrderId, @ProductId, @Quantity)";

                    foreach (var item in products)
                    {
                        using (SqlCommand orderItemsCommand = new SqlCommand(orderItemsQuery, connection, transaction))
                        {
                            orderItemsCommand.Parameters.AddWithValue("@OrderId", orderId);
                            orderItemsCommand.Parameters.AddWithValue("@ProductId", item.Key.ProductId);
                            orderItemsCommand.Parameters.AddWithValue("@Quantity", item.Value);
                            orderItemsCommand.ExecuteNonQuery();
                        }

                        // 3. Update product stock
                        string updateStockQuery = "UPDATE products SET stockQuantity = stockQuantity - @Quantity WHERE product_id = @ProductId";
                        using (SqlCommand updateStockCommand = new SqlCommand(updateStockQuery, connection, transaction))
                        {
                            updateStockCommand.Parameters.AddWithValue("@ProductId", item.Key.ProductId);
                            updateStockCommand.Parameters.AddWithValue("@Quantity", item.Value);
                            updateStockCommand.ExecuteNonQuery();
                        }
                    }

                    // 4. Clear the cart
                    string clearCartQuery = "DELETE FROM cart WHERE customer_id = @CustomerId";
                    using (SqlCommand clearCartCommand = new SqlCommand(clearCartQuery, connection, transaction))
                    {
                        clearCartCommand.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                        clearCartCommand.ExecuteNonQuery();
                    }

                    // Commit transaction
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<Dictionary<Product, int>> GetOrdersByCustomer(int customerId)
        {
            List<Dictionary<Product, int>> orders = new List<Dictionary<Product, int>>();

            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                // Check if customer exists
                if (!CustomerExists(customerId))
                {
                    throw new CustomerNotFoundException($"Customer with ID {customerId} not found.");
                }

                // Get all orders for the customer
                string ordersQuery = "SELECT order_id FROM orders WHERE customer_id = @CustomerId ORDER BY order_date DESC";
                List<int> orderIds = new List<int>();

                using (SqlCommand ordersCommand = new SqlCommand(ordersQuery, connection))
                {
                    ordersCommand.Parameters.AddWithValue("@CustomerId", customerId);

                    using (SqlDataReader ordersReader = ordersCommand.ExecuteReader())
                    {
                        while (ordersReader.Read())
                        {
                            orderIds.Add(Convert.ToInt32(ordersReader["order_id"]));
                        }
                    }
                }

                // For each order, get the products
                string itemsQuery = @"SELECT p.product_id, p.name, p.price, p.description, p.stockQuantity, oi.quantity 
                                    FROM order_items oi 
                                    JOIN products p ON oi.product_id = p.product_id 
                                    WHERE oi.order_id = @OrderId";

                foreach (int orderId in orderIds)
                {
                    Dictionary<Product, int> orderItems = new Dictionary<Product, int>();

                    using (SqlCommand itemsCommand = new SqlCommand(itemsQuery, connection))
                    {
                        itemsCommand.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataReader itemsReader = itemsCommand.ExecuteReader())
                        {
                            while (itemsReader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductId = Convert.ToInt32(itemsReader["product_id"]),
                                    Name = itemsReader["name"].ToString(),
                                    Price = Convert.ToDecimal(itemsReader["price"]),
                                    Description = itemsReader["description"].ToString(),
                                    StockQuantity = Convert.ToInt32(itemsReader["stockQuantity"])
                                };
                                int quantity = Convert.ToInt32(itemsReader["quantity"]);
                                orderItems.Add(product, quantity);
                            }
                        }
                    }

                    if (orderItems.Count > 0)
                    {
                        orders.Add(orderItems);
                    }
                }
            }

            return orders;
        }

        private bool CustomerExists(int customerId)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM customers WHERE customer_id = @CustomerId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", customerId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        private bool ProductExists(int productId)
        {
            using (SqlConnection connection = DBConnUtil.GetConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }
    }
}