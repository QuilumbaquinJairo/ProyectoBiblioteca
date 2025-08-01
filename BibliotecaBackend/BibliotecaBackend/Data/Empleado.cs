using System;
using System.Collections.Generic;

namespace BibliotecaBackend.Data;

public partial class Empleado
{
    public string Cedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public DateOnly FechaIngreso { get; set; }

    public decimal Sueldo { get; set; }

    public virtual ICollection<NominaCabecera> NominaCabeceras { get; set; } = new List<NominaCabecera>();
}
