using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;
public partial class UpdatePublicationRequest
{

    public long UserId { get; set; }

    public long EventoId { get; set; }

    public string Description { get; set; } = null!;


}
