USE ECommerceDB;
GO

--customers table
CREATE TABLE customers 
(
    customer_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL
);

--products table
CREATE TABLE products 
(
    product_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    description VARCHAR(500),
    stockQuantity INT NOT NULL
);

--cart table
CREATE TABLE cart 
(
    cart_id INT PRIMARY KEY IDENTITY(1,1),
    customer_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

--orders table
CREATE TABLE orders 
(
    order_id INT PRIMARY KEY IDENTITY(1,1),
    customer_id INT NOT NULL,
    order_date DATE DEFAULT GETDATE(),
    total_price DECIMAL(10, 2) NOT NULL,
    shipping_address VARCHAR(200) NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
);

--order_items table
CREATE TABLE order_items 
(
    order_item_id INT PRIMARY KEY IDENTITY(1,1),
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);


SELECT * FROM customers
SELECT * FROM products
SELECT * FROM cart
SELECT * FROM orders
SELECT * FROM order_items

-- Create new login
CREATE LOGIN [ecom_app] WITH PASSWORD = 'ecom';
GO

-- Grant permissions
USE [ECommerceDB];
GO
CREATE USER [ecom_app] FOR LOGIN [ecom_app];
GO
ALTER ROLE [db_owner] ADD MEMBER [ecom_app];
GO

-- Run in SSMS
SELECT name, is_disabled FROM sys.sql_logins 
WHERE name IN ('sa', 'ecom_app');