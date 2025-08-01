USE Biblioteca;
GO

-- Insertar otro empleado
INSERT INTO Empleado (Cedula, Nombre, FechaIngreso, Sueldo)
VALUES ('0911122235', 'María González', '2021-10-01', 1800.00);

-- Insertar nuevo motivo (bonificación)
INSERT INTO MotivoNomina (NombreMotivo, Tipo)
VALUES ('Bonificación anual', 'E');

-- Insertar cabecera y capturar ID
DECLARE @Nomina2 INT;

INSERT INTO NominaCabecera (Fecha, CedulaEmpleado)
VALUES ('2025-07-30', '0911122233');

SET @Nomina2 = SCOPE_IDENTITY();  -- Captura inmediata del ID generado

-- Insertar 2 detalles en una sola operación
INSERT INTO NominaDetalle (NumeroNomina, CodigoMotivo, Valor)
VALUES 
    (@Nomina2, 1, 1800.00),   -- Sueldo mensual (ya creado)
    (@Nomina2, 2, 500.00);    -- Bonificación anual (nuevo)

-- Verificar asiento contable generado por el trigger
SELECT *
FROM AsientoContable
WHERE TipoOperacion = 'Nomina' AND Referencia = @Nomina2;
