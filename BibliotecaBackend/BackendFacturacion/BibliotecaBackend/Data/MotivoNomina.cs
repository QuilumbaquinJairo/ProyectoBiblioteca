using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class MotivoNomina
{
    public int CodigoMotivo { get; set; }

    public string NombreMotivo { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public virtual ICollection<NominaDetalle> NominaDetalles { get; set; } = new List<NominaDetalle>();
}
