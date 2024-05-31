using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace NONINA_AGSC_APP_BACKEND.Models;

public partial class AddEventoRequest
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PlaceLabel { get; set; } = null!;

    public string PlaceCoordinates { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public long MunicipalityId { get; set; }

    public long CategoryId { get; set; }

    public long UserId { get; set; }
    public string ImageEvento { get; set; } = string.Empty;

    public IFormFile? ImageFile { get; set; }

}
