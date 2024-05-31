using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationsController : Controller
    {
        private readonly PublicationAPIDbContext dbContext;
        private readonly MessageAPIDbContext _contextMessage;


        public PublicationsController(PublicationAPIDbContext dbContext, MessageAPIDbContext contextMessage)
        {
            this.dbContext = dbContext;
            _contextMessage = contextMessage;
        }


        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetPublications()
        {
            var publications = await dbContext.Publications
                .Include(p => p.Evento)  
                .Include(p => p.User)
                .Include(p => p.Messages)
                .ToListAsync();

            var publicationsConNombres = publications.Select(p => new
            {
                p.Id,
                p.Description,
                p.EventoId,
                p.UserId,
                p.Evento.Title,
                p.User.Username,
                Messages = p.Messages?.Select(m => new
                {
                    m.Id,
                    m.UserId,
                    m.Description
                }).ToList()
            });

            return Ok(publicationsConNombres);
        }

        


        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetPublication([FromRoute] long id)
        {
            var publication = await dbContext.Publications
                .Include(p => p.Evento)
                .Include(p => p.User)
                .Include(p => p.Messages)

                .FirstOrDefaultAsync(p => p.Id == id);

            if (publication == null)
            {
                return NotFound();
            }

            var publicationConNombres = new
            {
                publication.Id,
                publication.Description,
                publication.EventoId,
                publication.UserId,
                publication.Evento.Title,
                publication.User.Username,
                Messages = publication.Messages?.Select(m => new
                {
                    m.Id,
                    m.UserId,
                    m.Description
                }).ToList()
            };

            return Ok(publicationConNombres);
        }


        [HttpPost]
        public async Task<IActionResult> AddPublication(AddPublicationRequest addPublicationRequest)
        {


            var publication = new Publication()
            {

                Description = addPublicationRequest.Description,
                EventoId = addPublicationRequest.EventoId,
                UserId = addPublicationRequest.UserId,

            };

            await dbContext.Publications.AddAsync(publication);
            await dbContext.SaveChangesAsync();

            return Ok(publication);
        }



        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePublication([FromRoute] long id, UpdatePublicationRequest updatePublicationRequest)
        {
            var publication = await dbContext.Publications.FindAsync(id);
            if (publication != null)
            {

                publication.Description = updatePublicationRequest.Description;
                publication.EventoId = updatePublicationRequest.EventoId;
                publication.UserId = updatePublicationRequest.UserId;


                await dbContext.SaveChangesAsync();
                return Ok(publication);

            }

            return NotFound();

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletePublication([FromRoute] long id)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var publication = await dbContext.Publications.FindAsync(id);
                if (publication == null)
                {
                    return NotFound();
                }

                // Buscar los mensajes relacionados con la publicación
                var messages = _contextMessage.Messages.Where(m => m.PublicationId == id);

                // Eliminar los mensajes relacionados
                _contextMessage.Messages.RemoveRange(messages);

                // Guardar cambios en la base de datos para los mensajes
                await _contextMessage.SaveChangesAsync();

                // Eliminar la publicación
                dbContext.Publications.Remove(publication);

                // Guardar cambios en la base de datos para la publicación
                await dbContext.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(publication);
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





