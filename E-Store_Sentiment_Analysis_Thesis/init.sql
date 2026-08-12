CREATE DATABASE store;
GO

USE store;
GO

CREATE TABLE categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL
);

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    email NVARCHAR(100) NOT NULL,
    password_hash NVARCHAR(256) NOT NULL,
    first_name NVARCHAR(50) NULL,
    last_name NVARCHAR(50) NULL,
    role NVARCHAR(50) NOT NULL DEFAULT 'Client',
    created_at DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NULL,
    category_id INT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    description NVARCHAR(500) NULL,
    stock INT NULL,
    CONSTRAINT FK_products_categories FOREIGN KEY (category_id) REFERENCES categories(id)
);

CREATE TABLE orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    order_date DATETIME2 NOT NULL DEFAULT GETDATE(),
    total_amount DECIMAL(10,2) NOT NULL,
    status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_orders_users FOREIGN KEY (customer_id) REFERENCES users(id)
);

CREATE TABLE products_orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_productsorders_orders FOREIGN KEY (order_id) REFERENCES orders(id),
    CONSTRAINT FK_productsorders_products FOREIGN KEY (product_id) REFERENCES products(id)
);

CREATE TABLE reviews (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NULL,
    user_id INT NULL,
    text NVARCHAR(500) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_reviews_products FOREIGN KEY (product_id) REFERENCES products(id),
    CONSTRAINT FK_reviews_users FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE review_analysis (
    id INT IDENTITY(1,1) PRIMARY KEY,
    review_id INT NOT NULL,
    price_score DECIMAL(2,1) NULL CHECK (price_score BETWEEN 1.0 AND 5.0),
    quality_score DECIMAL(2,1) NULL CHECK (quality_score BETWEEN 1.0 AND 5.0),
    delivery_score DECIMAL(2,1) NULL CHECK (delivery_score BETWEEN 1.0 AND 5.0),
    service_score DECIMAL(2,1) NULL CHECK (service_score BETWEEN 1.0 AND 5.0),
    overall_score DECIMAL(2,1) NULL CHECK (overall_score BETWEEN 1.0 AND 5.0),
    is_urgent BIT NULL,
    CONSTRAINT FK_reviewanalysis_reviews FOREIGN KEY (review_id) REFERENCES reviews(id)
);
GO