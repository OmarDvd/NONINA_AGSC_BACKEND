using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class UpdateAgendaRequest
{

    public long UserId { get; set; }
    public long EventoId { get; set; }

}
