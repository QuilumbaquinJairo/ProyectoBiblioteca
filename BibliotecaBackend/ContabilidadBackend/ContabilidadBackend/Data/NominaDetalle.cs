using System;
using System.Collections.Generic;

namespace ContabilidadBackend.Data;

public partial class NominaDetalle
{
    public int IdNominaDetalle { get; set; }

    public int NumeroNomina { get; set; }

    public int CodigoMotivo { get; set; }

    public decimal Valor { get; set; }

    public virtual MotivoNomina CodigoMotivoNavigation { get; set; } = null!;

    public virtual NominaCabecera NumeroNominaNavigation { get; set; } = null!;
}
