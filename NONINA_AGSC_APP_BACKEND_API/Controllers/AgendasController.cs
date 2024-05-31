using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AgendasController : Controller
    {
        private readonly AgendaAPIDbContext dbContext;
        private readonly UserAPIDbContext _contextUser;

        public AgendasController(AgendaAPIDbContext dbContext, UserAPIDbContext contextUser)
        {
            this.dbContext = dbContext;
            _contextUser = contextUser;
        }

        [HttpPost]
        [Route("getAgenda")]
        [Authorize]

        public async Task<IActionResult> GetAgendaIsolated([FromBody] AddAgendaRequest addAgendaRequest) {


            var userId = await _contextUser.Users
                                         .Where(u => u.Username == addAgendaRequest.Username)
                                         .Select(u => u.Id)
                                         .FirstOrDefaultAsync();

            var agendasQuery = await dbContext.Agendas
    .Where(a => a.UserId == userId && a.EventoId == addAgendaRequest.EventoId)
    .FirstOrDefaultAsync();

            if (agendasQuery == null)
            {
                return NotFound(); // Devuelve 404 si no se encuentra la agenda
            }
            else
            {
                return Ok(true); // Devuelve un resultado Ok con valor true si se encuentra la agenda
            }
        }


    [HttpGet]
        [Route("allUserPeople")]
        public async Task<IActionResult> GetAgendasUser([FromQuery] long? userId)
        {
            var agendasQuery = dbContext.Agendas
                .Include(a => a.Evento)
                .AsQueryable();


            if (userId.HasValue)
            {
                agendasQuery = agendasQuery.Where(a => a.UserId == userId);
            }
            
            var eventos = await agendasQuery
                .Select(a => new
                {
                    a.Id,
                    a.Evento.Title,
                    a.Evento.Description,
                    a.Evento.PlaceLabel,
                    a.Evento.PlaceCoordinates,
                    a.Evento.Date,
                    a.Evento.Time,
                    a.Evento.CategoryId,
                    a.Evento.MunicipalityId,
                    a.Evento.UserId,
                    CategoryName = a.Evento.Category.Name,
                    MunicipalityName = a.Evento.Municipality.Name,
                    UserName = a.Evento.User.Username
                })
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpPost]
        [Route("AddAgenda")]
        [Authorize]

        public async Task<IActionResult> AddAgenda(AddAgendaRequest addAgendaRequest)
        {

            var userId = await _contextUser.Users
                                         .Where(u => u.Username == addAgendaRequest.Username)
                                         .Select(u => u.Id)
                                         .FirstOrDefaultAsync();


            var agenda = new Agenda()
            {

                UserId = userId,
                EventoId = addAgendaRequest.EventoId,

            };

            await dbContext.Agendas.AddAsync(agenda);
            await dbContext.SaveChangesAsync();

            return Ok(agenda);
        }



        [HttpDelete]
        [Route("DeleteAgenda")]
        [Authorize]
        public async Task<IActionResult> DeleteAgenda([FromBody] AddAgendaRequest addAgendaRequest)
        {
            // Validación del addAgendaRequest
            if (addAgendaRequest == null || string.IsNullOrEmpty(addAgendaRequest.Username) || addAgendaRequest.EventoId == 0)
            {
                return BadRequest("Datos de solicitud inválidos.");
            }

            // Obtención del userId
            var userId = await _contextUser.Users
                                           .Where(u => u.Username == addAgendaRequest.Username)
                                           .Select(u => u.Id)
                                           .FirstOrDefaultAsync();

            if (userId == default)
            {
                return NotFound("Usuario no encontrado.");
            }

            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // Obtención de la agenda a eliminar
                var agenda = await dbContext.Agendas
                                            .Where(a => a.UserId == userId && a.EventoId == addAgendaRequest.EventoId)
                                            .FirstOrDefaultAsync();

                // Verificación de que la agenda exista
                if (agenda == null)
                {
                    return NotFound("Agenda no encontrada.");
                }

                // Eliminación de la agenda
                dbContext.Agendas.Remove(agenda);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(agenda);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar la agenda: {ex.Message}");
            }
        }




    }


}
