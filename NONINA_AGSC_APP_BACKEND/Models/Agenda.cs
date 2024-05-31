using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class Agenda
{
    public long Id { get; set; }
    [Column("UserId")]
    public long UserId { get; set; }
    [Column("EventoId")]
    public long EventoId { get; set; }

    public virtual Evento? Evento { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
