using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class Cliente
{
    public string Ruc { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public virtual ICollection<FacturaCabecera> FacturaCabeceras { get; set; } = new List<FacturaCabecera>();
}
