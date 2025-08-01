-- 1. Eliminar el trigger anterior en FacturaCabecera si existe
USE Biblioteca;
IF OBJECT_ID('trg_AsientoFactura', 'TR') IS NOT NULL
    DROP TRIGGER trg_AsientoFactura;
GO

-- 2. Crear el nuevo trigger en FacturaDetalle
CREATE TRIGGER trg_AsientoFactura
ON FacturaDetalle
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Previene duplicados si ya existe asiento para esta factura
    IF EXISTS (
        SELECT 1 FROM AsientoContable
        WHERE TipoOperacion = 'Factura'
          AND Referencia IN (SELECT NumeroFactura FROM inserted)
    )
        RETURN;

    INSERT INTO AsientoContable (Fecha, TipoOperacion, Referencia, CuentaDebito, CuentaCredito, Monto)
    SELECT 
        fc.Fecha,
        'Factura',
        i.NumeroFactura,
        '1101-Cuentas por Cobrar',
        '4101-Ventas',
        SUM(i.Cantidad * i.Precio)
    FROM inserted i
    JOIN FacturaCabecera fc ON fc.NumeroFactura = i.NumeroFactura
    GROUP BY i.NumeroFactura, fc.Fecha;
END;
GO
