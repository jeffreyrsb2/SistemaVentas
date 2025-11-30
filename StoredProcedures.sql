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

-- (Crearemos los SP para Actualizar y Eliminar más adelante)
