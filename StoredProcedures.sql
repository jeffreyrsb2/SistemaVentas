USE SistemaVentasDb;
GO

-- SP para obtener todos los productos
CREATE OR ALTER PROCEDURE sp_ObtenerProductos
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos WHERE Eliminado = 0;
END
GO

-- SP para obtener un producto por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerProductoPorId
    @Id INT
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock FROM Productos WHERE Id = @Id AND Eliminado = 0;
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
    UPDATE Productos SET Eliminado = 1 WHERE Id = @Id;
END
GO

-- SP para obtener productos con stock menor a un valor dado
CREATE OR ALTER PROCEDURE sp_ObtenerProductosBajoStock
    @StockMinimo INT
AS
BEGIN
    SELECT Id, Nombre, Descripcion, Precio, Stock 
    FROM Productos 
    WHERE Stock < @StockMinimo AND Eliminado = 0;
END
GO

-- SP para obtener un usuario por su nombre de usuario
CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorNombre
    @NombreUsuario NVARCHAR(100)
AS
BEGIN
    -- También traemos el nombre del Rol para usarlo en el token
    SELECT u.Id, u.NombreUsuario, u.PasswordHash, u.RolId, r.Nombre as RolNombre 
    FROM Usuarios u
    INNER JOIN Roles r ON u.RolId = r.Id
    WHERE u.NombreUsuario = @NombreUsuario;
END
GO

-- Tipo de tabla que usará el SP de CrearVenta para el parámetro de detalles
IF NOT EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = 'DetalleVentaType')
BEGIN
    CREATE TYPE dbo.DetalleVentaType AS TABLE
    (
        ProductoId INT,
        Cantidad INT,
        PrecioUnitario DECIMAL(18, 2)
    );
END
GO

-- SP para crear una Venta y sus Detalles en una transacción
CREATE OR ALTER PROCEDURE sp_CrearVenta
    @UsuarioId INT,
    @ClienteId INT,
    @Total DECIMAL(18, 2),
    @Detalles AS dbo.DetalleVentaType READONLY -- Usaremos un Tipo de Tabla
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Insertar la cabecera de la venta
        INSERT INTO Ventas (UsuarioId, ClienteId, Total, FechaVenta)
        VALUES (@UsuarioId, @ClienteId, @Total, GETDATE());

        DECLARE @VentaId INT = SCOPE_IDENTITY();

        -- Insertar los detalles
        INSERT INTO DetalleVentas (VentaId, ProductoId, Cantidad, PrecioUnitario)
        SELECT @VentaId, ProductoId, Cantidad, PrecioUnitario FROM @Detalles;

        -- Actualizar el stock de los productos
        UPDATE p
        SET p.Stock = p.Stock - d.Cantidad
        FROM Productos p
        INNER JOIN @Detalles d ON p.Id = d.ProductoId;

        COMMIT TRANSACTION;
        SELECT @VentaId AS VentaId; -- Devolver el ID de la nueva venta
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Relanzar el error para que la aplicación lo capture
        THROW;
    END CATCH
END
GO

-- SP para listar todas las ventas con nombres de cliente y usuario
CREATE OR ALTER PROCEDURE sp_ObtenerVentas
AS
BEGIN
    SELECT 
        v.Id, 
        v.FechaVenta, 
        v.Total, 
        v.ClienteId, 
        v.UsuarioId, 
        c.NombreCompleto AS ClienteNombre, 
        u.NombreUsuario AS UsuarioNombre
    FROM Ventas v
    INNER JOIN Clientes c ON v.ClienteId = c.Id
    INNER JOIN Usuarios u ON v.UsuarioId = u.Id
    WHERE v.Eliminado = 0 ORDER BY v.FechaVenta DESC;
END
GO

-- SP para obtener los detalles de una venta por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerClientes
AS
BEGIN
    SELECT Id, NombreCompleto, DocumentoIdentidad FROM Clientes WHERE Eliminado = 0;
END
GO

-- SP para obtener un cliente por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerClientePorId
    @Id INT
AS
BEGIN
    SELECT Id, NombreCompleto, DocumentoIdentidad FROM Clientes WHERE Id = @Id AND Eliminado = 0;
END
GO

-- SP para obtener un usuario por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorId
    @Id INT
AS
BEGIN
    SELECT u.Id, u.NombreUsuario, u.PasswordHash, u.RolId, r.Nombre as RolNombre 
    FROM Usuarios u
    INNER JOIN Roles r ON u.RolId = r.Id
    WHERE u.Id = @Id;
END
GO

-- SP para obtener una venta específica con sus detalles por su ID
CREATE OR ALTER PROCEDURE sp_ObtenerVentaPorId
    @VentaId INT
AS
BEGIN
    -- Primero, la cabecera
    SELECT 
        v.Id, v.FechaVenta, v.Total, v.ClienteId, v.UsuarioId,
        c.NombreCompleto AS ClienteNombre, 
        u.NombreUsuario AS UsuarioNombre
    FROM Ventas v
    INNER JOIN Clientes c ON v.ClienteId = c.Id
    INNER JOIN Usuarios u ON v.UsuarioId = u.Id
    WHERE v.Id = @VentaId AND v.Eliminado = 0;

    -- Segundo, los detalles
    SELECT 
        d.ProductoId, d.Cantidad, d.PrecioUnitario,
        p.Nombre AS ProductoNombre
    FROM DetalleVentas d
    INNER JOIN Productos p ON d.ProductoId = p.Id
    WHERE d.VentaId = @VentaId;
END
GO

-- SP para anular (eliminar lógicamente) una venta
CREATE OR ALTER PROCEDURE sp_AnularVenta
    @VentaId INT
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Marcar la venta como eliminada
        UPDATE Ventas SET Eliminado = 1 WHERE Id = @VentaId;

        -- Devolver el stock a los productos
        UPDATE p
        SET p.Stock = p.Stock + d.Cantidad
        FROM Productos p
        INNER JOIN DetalleVentas d ON p.Id = d.ProductoId
        WHERE d.VentaId = @VentaId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
