using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class FacturaDetalle
{
    public int IdFacturaDetalle { get; set; }

    public int NumeroFactura { get; set; }

    public int CodigoArticulo { get; set; }

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public virtual Articulo CodigoArticuloNavigation { get; set; } = null!;

    public virtual FacturaCabecera NumeroFacturaNavigation { get; set; } = null!;
}
