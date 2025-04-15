using ECommerceApp.dao;
using ECommerceApp.entity;
using ECommerceApp.exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ECommerceApp.Tests
{
    [TestClass]
    public class ECommerceSystemTests
    {
        private IOrderProcessorRepository _repository;
        private Customer _testCustomer;
        private Product _testProduct;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new OrderProcessorRepositoryImpl();

            // Create test customer
            _testCustomer = new Customer("Test User", "test@example.com", "password123");
            _repository.CreateCustomer(_testCustomer);

            // Create test product
            _testProduct = new Product("Test Product", 29.99m, "Test Description", 50);
            _repository.CreateProduct(_testProduct);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { _repository.DeleteCustomer(_testCustomer.CustomerId); } catch { }
            try { _repository.DeleteProduct(_testProduct.ProductId); } catch { }
        }

        // Test Case 1: Product created successfully
        [TestMethod]
        public void CreateProduct_ValidProduct_ReturnsTrue()
        {
            // Arrange
            var newProduct = new Product("New Product", 19.99m, "Description", 100);

            // Act
            bool result = _repository.CreateProduct(newProduct);

            // Assert
            Assert.IsTrue(result, "Product should be created successfully");

            // Cleanup
            _repository.DeleteProduct(newProduct.ProductId);
        }

        // Test Case 2: Product added to cart successfully
        [TestMethod]
        public void AddToCart_ValidProduct_ReturnsTrue()
        {
            // Arrange
            int quantity = 2;

            // Act
            bool result = _repository.AddToCart(_testCustomer, _testProduct, quantity);

            // Assert
            Assert.IsTrue(result, "Product should be added to cart successfully");
        }

        // Test Case 3: Product ordered successfully
        [TestMethod]
        public void PlaceOrder_ValidOrder_ReturnsTrue()
        {
            // Arrange
            _repository.AddToCart(_testCustomer, _testProduct, 1);
            var products = new Dictionary<Product, int> { { _testProduct, 1 } };
            string address = "123 Test St";

            // Act
            bool result = _repository.PlaceOrder(_testCustomer, products, address);

            // Assert
            Assert.IsTrue(result, "Order should be placed successfully");
        }

        // Test Case 4: Exception thrown for invalid customer
        [TestMethod]
        [ExpectedException(typeof(CustomerNotFoundException))]
        public void AddToCart_InvalidCustomer_ThrowsException()
        {
            // Arrange
            var invalidCustomer = new Customer { CustomerId = 99999 };

            // Act
            _repository.AddToCart(invalidCustomer, _testProduct, 1);
        }

        // Test Case 5: Exception thrown for invalid product
        [TestMethod]
        [ExpectedException(typeof(ProductNotFoundException))]
        public void AddToCart_InvalidProduct_ThrowsException()
        {
            // Arrange
            var invalidProduct = new Product { ProductId = 99999 };

            // Act
            _repository.AddToCart(_testCustomer, invalidProduct, 1);
        }

        // Additional test case: Get orders by customer
        [TestMethod]
        public void GetOrdersByCustomer_ValidCustomer_ReturnsOrders()
        {
            // Arrange
            _repository.AddToCart(_testCustomer, _testProduct, 2);
            var products = new Dictionary<Product, int> { { _testProduct, 2 } };
            _repository.PlaceOrder(_testCustomer, products, "123 Test St");

            // Act
            var orders = _repository.GetOrdersByCustomer(_testCustomer.CustomerId);

            // Assert
            Assert.IsTrue(orders.Count > 0, "Should return at least one order");
        }

        // Additional test case: Remove from cart
        [TestMethod]
        public void RemoveFromCart_ValidProduct_ReturnsTrue()
        {
            // Arrange
            _repository.AddToCart(_testCustomer, _testProduct, 1);

            // Act
            bool result = _repository.RemoveFromCart(_testCustomer, _testProduct);

            // Assert
            Assert.IsTrue(result, "Product should be removed from cart");
        }
    }
}