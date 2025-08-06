using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class CiudadEntrega
{
    public int CodigoCiudad { get; set; }

    public string NombreCiudad { get; set; } = null!;

    public virtual ICollection<FacturaCabecera> FacturaCabeceras { get; set; } = new List<FacturaCabecera>();
}
