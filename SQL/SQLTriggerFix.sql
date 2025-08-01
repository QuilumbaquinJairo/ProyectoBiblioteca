-- Primero eliminamos el anterior si existe
IF OBJECT_ID('trg_AsientoNomina', 'TR') IS NOT NULL
    DROP TRIGGER trg_AsientoNomina;
GO

-- Creamos el nuevo trigger en NominaDetalle
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
