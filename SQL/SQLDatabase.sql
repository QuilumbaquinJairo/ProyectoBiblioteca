-- Tabla de Ciudades (Catálogo)
CREATE TABLE CiudadEntrega (
    CodigoCiudad INT IDENTITY(1,1) PRIMARY KEY,
    NombreCiudad NVARCHAR(100) NOT NULL
);

-- Tabla de Clientes
CREATE TABLE Cliente (
    RUC CHAR(13) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Direccion NVARCHAR(200) NOT NULL
);

-- Tabla de Artículos (requiere para facturación)
CREATE TABLE Articulo (
    CodigoArticulo INT IDENTITY(1,1) PRIMARY KEY,
    NombreArticulo NVARCHAR(100) NOT NULL,
    SaldoInventario INT NOT NULL
);

-- Cabecera de Facturas
CREATE TABLE FacturaCabecera (
    NumeroFactura INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATE NOT NULL,
    CodigoCiudadEntrega INT NOT NULL,
    RUCCliente CHAR(13) NOT NULL,
    CONSTRAINT FK_FacturaCab_Ciudad FOREIGN KEY (CodigoCiudadEntrega) 
        REFERENCES CiudadEntrega(CodigoCiudad) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaCab_Cliente FOREIGN KEY (RUCCliente)
        REFERENCES Cliente(RUC) ON DELETE CASCADE
);

-- Detalle de Facturas
CREATE TABLE FacturaDetalle (
    IdFacturaDetalle INT IDENTITY(1,1) PRIMARY KEY,
    NumeroFactura INT NOT NULL,
    CodigoArticulo INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    Precio DECIMAL(18,2) NOT NULL CHECK (Precio >= 0),
    CONSTRAINT FK_FacturaDet_Cabecera FOREIGN KEY (NumeroFactura)
        REFERENCES FacturaCabecera(NumeroFactura) ON DELETE CASCADE,
    CONSTRAINT FK_FacturaDet_Articulo FOREIGN KEY (CodigoArticulo)
        REFERENCES Articulo(CodigoArticulo)
);

-- Tabla Motivos de Nómina
CREATE TABLE MotivoNomina (
    CodigoMotivo INT IDENTITY(1,1) PRIMARY KEY,
    NombreMotivo NVARCHAR(100) NOT NULL,
    Tipo CHAR(1) NOT NULL CHECK (Tipo IN ('I', 'E')) -- I: Ingreso, E: Egreso
);

-- Tabla de Empleados
CREATE TABLE Empleado (
    Cedula CHAR(10) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    FechaIngreso DATE NOT NULL,
    Sueldo DECIMAL(18,2) NOT NULL CHECK (Sueldo >= 0)
);

-- Cabecera de Nómina
CREATE TABLE NominaCabecera (
    NumeroNomina INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATE NOT NULL,
    CedulaEmpleado CHAR(10) NOT NULL,
    CONSTRAINT FK_NominaCab_Empleado FOREIGN KEY (CedulaEmpleado)
        REFERENCES Empleado(Cedula) ON DELETE CASCADE
);

-- Detalle de Nómina
CREATE TABLE NominaDetalle (
    IdNominaDetalle INT IDENTITY(1,1) PRIMARY KEY,
    NumeroNomina INT NOT NULL,
    CodigoMotivo INT NOT NULL,
    Valor DECIMAL(18,2) NOT NULL CHECK (Valor > 0),
    CONSTRAINT FK_NominaDet_Cabecera FOREIGN KEY (NumeroNomina)
        REFERENCES NominaCabecera(NumeroNomina) ON DELETE CASCADE,
    CONSTRAINT FK_NominaDet_Motivo FOREIGN KEY (CodigoMotivo)
        REFERENCES MotivoNomina(CodigoMotivo)
);

-- Tabla Asientos Contables Automáticos
CREATE TABLE AsientoContable (
    IdAsientoContable UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Fecha DATE NOT NULL,
    TipoOperacion NVARCHAR(50) NOT NULL, -- Ej: 'Factura', 'Nómina'
    Referencia INT NOT NULL,
    CuentaDebito NVARCHAR(50) NOT NULL,
    CuentaCredito NVARCHAR(50) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL CHECK (Monto > 0)
);
