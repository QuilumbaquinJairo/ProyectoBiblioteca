--  1. Trigger para validar que no se venda más del saldo de inventario
USE Biblioteca;
GO

CREATE TRIGGER trg_ValidarInventario
ON FacturaDetalle
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar que la cantidad no exceda el inventario
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Articulo a ON i.CodigoArticulo = a.CodigoArticulo
        WHERE i.Cantidad > a.SaldoInventario
    )
    BEGIN
        RAISERROR('No se puede vender más de lo disponible en inventario.', 16, 1);
        RETURN;
    END

    -- Insertar si la cantidad es válida
    INSERT INTO FacturaDetalle (NumeroFactura, CodigoArticulo, Cantidad, Precio)
    SELECT NumeroFactura, CodigoArticulo, Cantidad, Precio
    FROM inserted;

    -- Actualizar inventario
    UPDATE a
    SET a.SaldoInventario = a.SaldoInventario - i.Cantidad
    FROM Articulo a
    JOIN inserted i ON a.CodigoArticulo = i.CodigoArticulo;
END;
GO

-- 2. Trigger para generar asiento contable al crear una factura
CREATE TRIGGER trg_AsientoFactura
ON FacturaCabecera
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AsientoContable (Fecha, TipoOperacion, Referencia, CuentaDebito, CuentaCredito, Monto)
    SELECT 
        i.Fecha,
        'Factura',
        i.NumeroFactura,
        '1101-Cuentas por Cobrar',
        '4101-Ventas',
        SUM(fd.Cantidad * fd.Precio)
    FROM inserted i
    JOIN FacturaDetalle fd ON fd.NumeroFactura = i.NumeroFactura
    GROUP BY i.NumeroFactura, i.Fecha;
END;
GO

-- 3. Creamos el nuevo trigger en NominaDetalle
CREATE TRIGGER trg_AsientoNomina
ON NominaDetalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AsientoContable (Fecha, TipoOperacion, Referencia, CuentaDebito, CuentaCredito, Monto)
    SELECT 
        nc.Fecha,
        'Nomina',
        i.NumeroNomina,
        '5101-Gastos de Personal',
        '1102-Banco',
        SUM(i.Valor)
    FROM inserted i
    JOIN NominaCabecera nc ON nc.NumeroNomina = i.NumeroNomina
    GROUP BY i.NumeroNomina, nc.Fecha;
END;
GO
