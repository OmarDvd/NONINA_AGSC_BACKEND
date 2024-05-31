using System;
using System.Collections.Generic;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class AddUserRequest
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int? Age { get; set; }

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
    public string Salt { get; set; } // Nuevo campo para la sal

    public bool Admin { get; set; }

    public bool Owner { get; set; }


}
