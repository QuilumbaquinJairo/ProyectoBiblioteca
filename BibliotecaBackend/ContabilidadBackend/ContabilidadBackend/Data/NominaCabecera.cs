using System;
using System.Collections.Generic;

namespace ContabilidadBackend.Data;

public partial class NominaCabecera
{
    public int NumeroNomina { get; set; }

    public DateOnly Fecha { get; set; }

    public string CedulaEmpleado { get; set; } = null!;

    public virtual Empleado CedulaEmpleadoNavigation { get; set; } = null!;

    public virtual ICollection<NominaDetalle> NominaDetalles { get; set; } = new List<NominaDetalle>();
}
