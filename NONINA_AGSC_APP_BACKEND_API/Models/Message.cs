using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;
[Table("Messages")]

public partial class Message
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PublicationId { get; set; }

    public string Description { get; set; } = null!;

    public virtual Publication? Publication { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
