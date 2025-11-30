USE master;
GO

IF DB_ID('SistemaVentasDb') IS NOT NULL
BEGIN
-- Eliminar la base de datos si ya existe
-- Se pone en modo de usuario único para forzar la desconexión de cualquier conexión activa
    ALTER DATABASE SistemaVentasDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaVentasDb;
END
GO

-- 1. Crear la base de datos
CREATE DATABASE SistemaVentasDb;
GO

-- 2. Usar la nueva base de datos
USE SistemaVentasDb;
GO

-- 3. Creación de Tablas

CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY,
    NombreUsuario NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RolId INT NOT NULL,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(Id)
);
GO

CREATE TABLE Clientes (
    Id INT PRIMARY KEY IDENTITY,
    NombreCompleto NVARCHAR(200) NOT NULL,
    DocumentoIdentidad NVARCHAR(20) UNIQUE
);
GO

CREATE TABLE Productos (
    Id INT PRIMARY KEY IDENTITY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(500),
    Precio DECIMAL(18, 2) NOT NULL,
    Stock INT NOT NULL
);
GO

CREATE TABLE Ventas (
    Id INT PRIMARY KEY IDENTITY,
    FechaVenta DATETIME2 NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(18, 2) NOT NULL,
    UsuarioId INT NOT NULL,
    ClienteId INT NOT NULL,
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);
GO

CREATE TABLE DetalleVentas (
    Id INT PRIMARY KEY IDENTITY,
    VentaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_DetalleVentas_Ventas FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
    CONSTRAINT FK_DetalleVentas_Productos FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
GO

-- 4. Inserción de Datos de Prueba (Seed Data)

INSERT INTO Roles (Nombre) VALUES ('Administrador'), ('Vendedor');
GO

-- Contraseña para ambos es "123". En el backend la hashearemos con BCrypt.
-- Este es el hash para "123": $2a$11$G.A.s.J0M3hJ.d6lRwB66e/Jd0/5T9.2j2.5Qz1n2pX.bU9.yY9lW
INSERT INTO Usuarios (NombreUsuario, PasswordHash, RolId) VALUES 
('admin', '$2a$11$G.A.s.J0M3hJ.d6lRwB66e/Jd0/5T9.2j2.5Qz1n2pX.bU9.yY9lW', 1),
('vendedor1', '$2a$11$G.A.s.J0M3hJ.d6lRwB66e/Jd0/5T9.2j2.5Qz1n2pX.bU9.yY9lW', 2);
GO

INSERT INTO Clientes (NombreCompleto, DocumentoIdentidad) VALUES
('Cliente General', '00000000'),
('Juan Perez', '12345678');
GO

INSERT INTO Productos (Nombre, Descripcion, Precio, Stock) VALUES
('Laptop Pro 15"', 'Laptop de alto rendimiento', 1500.00, 10),
('Mouse Inalámbrico', 'Mouse ergonómico recargable', 25.50, 50),
('Teclado Mecánico RGB', 'Teclado para gaming y productividad', 80.00, 20),
('Monitor 27" 4K', 'Monitor UHD para diseño gráfico', 400.00, 15),
('Impresora Multifuncional', 'Impresora, escáner y copiadora', 120.00, 8),
('Auriculares Bluetooth', 'Auriculares con cancelación de ruido', 60.00, 30),
('Disco Duro Externo 1TB', 'Almacenamiento portátil USB 3.0', 70.00, 25),
('Smartphone X200', 'Teléfono inteligente de última generación', 800.00, 12),
('Tablet 10"', 'Tablet para entretenimiento y trabajo', 300.00, 18),
('Cámara Digital 20MP', 'Cámara compacta para fotografía', 250.00, 14),
('Router WiFi AC1200', 'Router de alta velocidad para el hogar', 90.00, 22),
('Smartwatch FitPro', 'Reloj inteligente con monitor de actividad', 150.00, 16),
('Altavoz Bluetooth Portátil', 'Altavoz resistente al agua', 45.00, 40),
('Proyector HD Mini', 'Proyector portátil para presentaciones', 200.00, 10),
('Memoria RAM 16GB DDR4', 'Módulo de memoria para computadoras', 75.00, 35),
('Tarjeta Gráfica GTX 1660', 'Tarjeta gráfica para gaming y diseño', 300.00, 7),
('Fuente de Poder 600W', 'Fuente de alimentación para PC', 65.00, 28),
('Placa Madre ATX', 'Placa base compatible con procesadores Intel', 120.00, 9),
('Procesador Intel i7-10700K', 'Procesador de alto rendimiento para PC', 350.00, 11),
('SSD NVMe 500GB', 'Unidad de estado sólido para almacenamiento rápido', 100.00, 20);
GO
