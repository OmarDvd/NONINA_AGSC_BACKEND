using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : Controller
    {
        private readonly AgendaAPIDbContext _contextAgenda;
        private readonly PublicationAPIDbContext _contextPublication;
        private readonly MessageAPIDbContext _contextMessage;
        private readonly EventoAPIDbContext dbContext;
        private readonly IWebHostEnvironment? _hostingEnvironment;

        public EventoController(EventoAPIDbContext dbContext, IWebHostEnvironment? hostingEnvironment, AgendaAPIDbContext contextAgenda, PublicationAPIDbContext contextPublication, MessageAPIDbContext contextMessage)
        {
            this.dbContext = dbContext;
            this._hostingEnvironment = hostingEnvironment;
            _contextAgenda = contextAgenda;
            _contextPublication = contextPublication;
            _contextMessage = contextMessage;
        }


        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetEventos()
        {
            var eventos = await dbContext.Eventos
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Municipality)
                .ToListAsync();

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var imageBaseUrl = $"{baseUrl}/images/"; 

            var eventosConNombres = eventos.Select(e => new
            {
                e.Id,
                e.Title,
                e.Description,
                e.PlaceLabel,
                e.PlaceCoordinates,
                e.Date,
                e.Time,
                e.CategoryId,
                e.MunicipalityId,
                e.UserId,
                ImageEvento = $"{imageBaseUrl}{Path.GetFileName(e.ImageEvento)}", 
                CategoryName = e.Category.Name,
                MunicipalityName = e.Municipality.Name,
                UserName = e.User.Username
            });

            return Ok(eventosConNombres);
        }







        [HttpGet]
        [Route("allDepend")]
        public async Task<IActionResult> GetEventosDepend([FromQuery] long? categoryId, [FromQuery] long? municipalityId, [FromQuery] String? date, [FromQuery] int? limit, [FromQuery] int? offset)
        {
            var eventosQuery = dbContext.Eventos
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Municipality)
                .AsQueryable(); 

            
            if (categoryId.HasValue && categoryId>0)
            {
                eventosQuery = eventosQuery.Where(e => e.CategoryId == categoryId);
            }
            if (municipalityId.HasValue && municipalityId > 0)
            {
                eventosQuery = eventosQuery.Where(e => e.MunicipalityId == municipalityId);
            }



            if (date != null)
            {
                try
                {
                    var fechaConsulta = DateOnly.ParseExact(date, "yyyy-MM-dd", null);
                    eventosQuery = eventosQuery.Where(e => e.Date.Equals(fechaConsulta));
                }
                catch (FormatException)
                {
                    // Manejo de error si la cadena de fecha no está en el formato esperado
                }
            }



            if (limit.HasValue)
            {
                eventosQuery = eventosQuery.Skip(offset ?? 0).Take(limit.Value);
            }


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var imageBaseUrl = $"{baseUrl}/images/";

            var eventos = await eventosQuery
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.PlaceLabel,
                    e.PlaceCoordinates,
                    e.Date,
                    e.Time,
                    e.CategoryId,
                    e.MunicipalityId,
                    e.UserId,
                    ImageEvento = $"{imageBaseUrl}{Path.GetFileName(e.ImageEvento)}",
                    CategoryName = e.Category.Name,
                    MunicipalityName = e.Municipality.Name,
                    UserName = e.User.Username
                })
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpGet]
        [Route("allUserOwner")]
        [Authorize]

        public async Task<IActionResult> GetEventosUser([FromQuery] long? userId)
        {
            var eventosQuery = dbContext.Eventos
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Municipality)
                .AsQueryable();


            if (userId.HasValue)
            {
                eventosQuery = eventosQuery.Where(e => e.UserId == userId);
            }


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var imageBaseUrl = $"{baseUrl}/images/";

            var eventos = await eventosQuery
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.PlaceLabel,
                    e.PlaceCoordinates,
                    e.Date,
                    e.Time,
                    e.CategoryId,
                    e.MunicipalityId,
                    e.UserId,
                    ImageEvento = $"{imageBaseUrl}{Path.GetFileName(e.ImageEvento)}",
                    CategoryName = e.Category.Name,
                    MunicipalityName = e.Municipality.Name,
                    UserName = e.User.Username
                })
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpGet]
        [Route("allsearch")]
        public async Task<IActionResult> GetEventosSearch([FromQuery] string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest("El texto de búsqueda no puede estar vacío.");
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var imageBaseUrl = $"{baseUrl}/images/";

            var eventos = await dbContext.Eventos
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Municipality)
                .Where(e => e.Title.Contains(searchText))
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.PlaceLabel,
                    e.PlaceCoordinates,
                    e.Date,
                    e.Time,
                    e.CategoryId,
                    e.MunicipalityId,
                    e.UserId,
                    ImageEvento = $"{imageBaseUrl}{Path.GetFileName(e.ImageEvento)}",
                    CategoryName = e.Category.Name,
                    MunicipalityName = e.Municipality.Name,
                    UserName = e.User.Username
                })
                .ToListAsync();

            return Ok(eventos);
        }


        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetEvento([FromRoute] long id)
        {
            var evento = await dbContext.Eventos
                .Include(e => e.Category)
                .Include(e => e.User)
                .Include(e => e.Municipality)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var imageBaseUrl = $"{baseUrl}/images/";


            var eventoConNombres = new
            {
                evento.Id,
                evento.Title,
                evento.Description,
                evento.PlaceLabel,
                evento.PlaceCoordinates,
                evento.Date,
                evento.Time,
                evento.CategoryId,
                evento.MunicipalityId,
                evento.UserId,
                ImageEvento = $"{imageBaseUrl}{Path.GetFileName(evento.ImageEvento)}",
                CategoryName = evento.Category.Name,
                MunicipalityName = evento.Municipality.Name,
                UserName = evento.User.Username
            };

            return Ok(eventoConNombres);
        }



      

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> AddEvento(AddEventoRequest addEventoRequest)
        {
            try
            {
                // Verificar si se proporcionó una imagen
                if (addEventoRequest.ImageFile == null)
                {
                    return BadRequest("No se ha proporcionado ninguna imagen para cargar.");
                }

                // Verificar la longitud del archivo
                if (addEventoRequest.ImageFile.Length == 0)
                {
                    return BadRequest("El archivo de imagen proporcionado está vacío.");
                }

                // Generar un nombre único para la imagen
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addEventoRequest.ImageFile.FileName);
                var folderPath = Path.Combine("wwwroot", "images");
                var filePath = Path.Combine(folderPath, fileName);




                // Guardar la imagen en el sistema de archivos
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addEventoRequest.ImageFile.CopyToAsync(stream);
                }

                // Crear el evento con la ruta de la imagen
                var evento = new Evento()
                {
                    Title = addEventoRequest.Title,
                    Description = addEventoRequest.Description,
                    PlaceLabel = addEventoRequest.PlaceLabel,
                    PlaceCoordinates = addEventoRequest.PlaceCoordinates,
                    Date = addEventoRequest.Date,
                    Time = addEventoRequest.Time,
                    MunicipalityId = addEventoRequest.MunicipalityId,
                    CategoryId = addEventoRequest.CategoryId,
                    UserId = addEventoRequest.UserId,
                    ImageEvento = filePath // Guardar la ruta de la imagen en la base de datos
                };

                // Guardar el evento en la base de datos
                await dbContext.Eventos.AddAsync(evento);
                await dbContext.SaveChangesAsync();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                // Aquí registras la excepción original
                Console.WriteLine($"Error al agregar el evento: {ex}");
                return StatusCode(500, $"Error al agregar el evento: {ex.Message}");
            }
        
    }





    [HttpPut]
        [Route("{id:int}")]
        [Authorize]

        public async Task<IActionResult> UpdateEvento([FromRoute] long id, UpdateEventoRequest updateEventoRequest)
        {
            var evento = await dbContext.Eventos.FindAsync(id);
            if (evento != null)
            {

                evento.Title = updateEventoRequest.Title;
                evento.Description = updateEventoRequest.Description;
                evento.PlaceLabel = updateEventoRequest.PlaceLabel;
                evento.PlaceCoordinates = updateEventoRequest.PlaceCoordinates;
                evento.Date = updateEventoRequest.Date;
                evento.Time = updateEventoRequest.Time;
                evento.MunicipalityId = updateEventoRequest.MunicipalityId;
                evento.CategoryId = updateEventoRequest.CategoryId;
                evento.UserId = updateEventoRequest.UserId;
                evento.ImageEvento = updateEventoRequest.ImageEvento;

                await dbContext.SaveChangesAsync();
                return Ok(evento);

            }

            return NotFound();

        }

        [HttpDelete]
        [Route("{id:int}")]
            [Authorize]

        public async Task<IActionResult> DeleteEvento([FromRoute] long id)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var evento = await dbContext.Eventos.FindAsync(id);
                if (evento == null)
                {
                    return NotFound();
                }

                // Buscar las agendas relacionadas con el evento
                var agendas = _contextAgenda.Agendas.Where(a => a.EventoId == id).ToList();
                if (agendas.Any())
                {
                    // Eliminar las agendas relacionadas
                    _contextAgenda.Agendas.RemoveRange(agendas);
                    await _contextAgenda.SaveChangesAsync();
                }
                // Buscar las publicaciones relacionadas con el evento
                var publications = _contextPublication.Publications.Where(p => p.EventoId == id).ToList();

                // Buscar y eliminar los mensajes relacionados con las publicaciones
                foreach (var publication in publications)
                {
                    var messages = _contextMessage.Messages.Where(m => m.PublicationId == publication.Id).ToList();
                    if (messages.Any())
                    {
                        _contextMessage.Messages.RemoveRange(messages);
                        await _contextMessage.SaveChangesAsync();

                    }
                }
                if (publications.Any())
                {                // Eliminar las publicaciones relacionadas
                     _contextPublication.Publications.RemoveRange(publications);
                    await _contextPublication.SaveChangesAsync();

                }

                // Eliminar el evento
                dbContext.Eventos.Remove(evento);

                // Guardar cambios en todas las entidades
                await dbContext.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                // Revertir la transacción en caso de error
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}