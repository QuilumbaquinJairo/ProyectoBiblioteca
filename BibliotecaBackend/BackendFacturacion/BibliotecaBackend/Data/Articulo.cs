using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class Articulo
{
    public int CodigoArticulo { get; set; }

    public string NombreArticulo { get; set; } = null!;

    public int SaldoInventario { get; set; }
    public decimal Precio { get; set; }

    public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
}
