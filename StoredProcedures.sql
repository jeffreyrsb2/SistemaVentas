USE SistemaVentasDb;
GO

-- SP para obtener todos los productos
CREATE OR ALTER PROCEDURE sp_ObtenerProductos
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos;
END
GO

-- SP para obtener un producto por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerProductoPorId
    @Id INT
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos WHERE Id = @Id;
END
GO

-- SP para crear un nuevo producto
CREATE OR ALTER PROCEDURE sp_CrearProducto
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(500),
    @Precio DECIMAL(18, 2),
    @Stock INT
AS
BEGIN
    INSERT INTO Productos (Nombre, Descripcion, Precio, Stock)
    VALUES (@Nombre, @Descripcion, @Precio, @Stock);
    SELECT CAST(SCOPE_IDENTITY() AS INT); -- Devuelve el ID del nuevo producto
END
GO

-- SP para actualizar un producto existente
CREATE OR ALTER PROCEDURE sp_ActualizarProducto
    @Id INT,
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(500),
    @Precio DECIMAL(18, 2),
    @Stock INT
AS
BEGIN
    UPDATE Productos
    SET Nombre = @Nombre,
        Descripcion = @Descripcion,
        Precio = @Precio,
        Stock = @Stock
    WHERE Id = @Id;
END
GO

-- SP para eliminar un producto
CREATE OR ALTER PROCEDURE sp_EliminarProducto
    @Id INT
AS
BEGIN
    DELETE FROM Productos WHERE Id = @Id;
END
GO

-- SP para obtener productos con stock menor a un valor dado
CREATE OR ALTER PROCEDURE sp_ObtenerProductosBajoStock
    @StockMinimo INT
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock 
    FROM Productos 
    WHERE Stock < @StockMinimo;
END
GO

