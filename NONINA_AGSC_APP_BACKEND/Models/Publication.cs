using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;
[Table("Publications")]

public partial class Publication
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EventoId { get; set; }

    public string Description { get; set; } = null!;

    public virtual Evento? Evento { get; set; } = null!;

    public virtual ICollection<Message>? Messages { get; set; } = new List<Message>();

    public virtual User? User { get; set; } = null!;
}
