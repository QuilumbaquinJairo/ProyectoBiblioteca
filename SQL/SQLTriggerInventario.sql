USE Biblioteca;
GO

-- Crear un art�culo con saldo 10
INSERT INTO Articulo (NombreArticulo, SaldoInventario)
VALUES ('Libro de SQL', 10);

-- Insertar cliente
INSERT INTO Cliente (RUC, Nombre, Direccion)
VALUES ('1234567890001', 'Cliente Demo', 'Av. Siempre Viva');

-- Insertar ciudad
INSERT INTO CiudadEntrega (NombreCiudad)
VALUES ('Quito');

-- Insertar factura cabecera
INSERT INTO FacturaCabecera (Fecha, CodigoCiudadEntrega, RUCCliente)
VALUES (GETDATE(), 1, '1234567890001');

-- Obtener n�mero de factura
DECLARE @FacturaID INT = SCOPE_IDENTITY();

--  Intentar vender 5 unidades (v�lido)
INSERT INTO FacturaDetalle (NumeroFactura, CodigoArticulo, Cantidad, Precio)
VALUES (@FacturaID, 1, 5, 12.00); -- Esto debe funcionar

-- Intentar vender 100 unidades (inv�lido)
INSERT INTO FacturaDetalle (NumeroFactura, CodigoArticulo, Cantidad, Precio)
VALUES (@FacturaID, 1, 100, 12.00); -- Esto debe fallar con error
