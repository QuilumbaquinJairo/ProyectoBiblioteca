using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class FacturaCabecera
{
    public int NumeroFactura { get; set; }

    public DateTime Fecha { get; set; }

    public int CodigoCiudadEntrega { get; set; }

    public string Ruccliente { get; set; } = null!;

    public virtual CiudadEntrega CodigoCiudadEntregaNavigation { get; set; } = null!;

    public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();

    public virtual Cliente RucclienteNavigation { get; set; } = null!;
}
