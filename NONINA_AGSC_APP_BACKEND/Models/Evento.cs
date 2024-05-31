using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NONINA_AGSC_APP_BACKEND.Models;
[Table("Eventos")]

public partial class Evento
{

    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PlaceLabel { get; set; } = null!;

    public string PlaceCoordinates { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    [Column("Municipality_Id")] // Especifica el nombre de la columna en la base de datos
    public long MunicipalityId { get; set; }

    [Column("Category_Id")] // Especifica el nombre de la columna en la base de datos
    public long CategoryId { get; set; }

    [Column("User_Id")] // Especifica el nombre de la columna en la base de datos
    public long UserId { get; set; }
    [Column("ImageEvento")]
    public string ImageEvento { get; set; } = string.Empty;



    public virtual ICollection<Agenda>? Agenda { get; set; } = new List<Agenda>();

    public virtual Category? Category { get; set; } = null!;

    public virtual Municipality? Municipality { get; set; } = null!;

    public virtual ICollection<Publication>? Publications { get; set; } = new List<Publication>();

    public virtual User? User { get; set; } = null!;
}
