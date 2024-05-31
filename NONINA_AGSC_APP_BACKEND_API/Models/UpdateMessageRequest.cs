using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;
public partial class UpdateMessageRequest
{

    public long UserId { get; set; }

    public long PublicationId { get; set; }

    public string Description { get; set; } = null!;

}