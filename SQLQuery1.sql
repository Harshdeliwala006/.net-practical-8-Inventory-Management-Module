USE InventoryDB;
GO

DROP TABLE Products;
GO

CREATE TABLE Products
(
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName VARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL
);
GO