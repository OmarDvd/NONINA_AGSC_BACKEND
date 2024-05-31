using System;
using System.Collections.Generic;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class AddCategoryRequest
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;


}
