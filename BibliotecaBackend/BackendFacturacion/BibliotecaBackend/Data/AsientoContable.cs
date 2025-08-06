using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class AsientoContable
{
    public Guid IdAsientoContable { get; set; }

    public DateTime Fecha { get; set; }

    public string TipoOperacion { get; set; } = null!;

    public int Referencia { get; set; }

    public string CuentaDebito { get; set; } = null!;

    public string CuentaCredito { get; set; } = null!;

    public decimal Monto { get; set; }
}
