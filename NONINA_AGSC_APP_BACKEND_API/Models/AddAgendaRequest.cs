using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class AddAgendaRequest
{
    public long Id { get; set; }

    public string Username { get; set; }
    public long EventoId { get; set; }

}
