using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AgendaAPIDbContext _contextAgenda;
        private readonly PublicationAPIDbContext _contextPublication;
        private readonly MessageAPIDbContext _contextMessage;
        private readonly EventoAPIDbContext _contextEvento;
        private readonly UserAPIDbContext dbContext;
        public UsersController(UserAPIDbContext dbContext, AgendaAPIDbContext contextAgenda, PublicationAPIDbContext contextPublication, MessageAPIDbContext contextMessage, EventoAPIDbContext contextEvento)
        {
            this.dbContext = dbContext;

            _contextAgenda = contextAgenda;
            _contextPublication = contextPublication;
            _contextMessage = contextMessage;
            _contextEvento = contextEvento;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await dbContext.Users.ToListAsync());
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetUser([FromRoute] long id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserRequest addUserRequest)
        {
            // Generar la sal aleatoria
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // Concatenar la sal con la contraseña del usuario
            string hashedPassword = HashPassword(addUserRequest.Password, salt);

            var user = new User()
            {
                Name = addUserRequest.Name,
                Surname = addUserRequest.Surname,
                Age = addUserRequest.Age,
                Email = addUserRequest.Email,
                Username = addUserRequest.Username,
                Password = hashedPassword, // Guardar el hash en lugar de la contraseña original
                Salt = salt, // Guardar la sal
                Admin = addUserRequest.Admin,
                Owner = addUserRequest.Owner
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user);
        }

        // Método para hashear la contraseña junto con la sal
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }




        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateUser([FromRoute] long id, UpdateUserRequest updateUserRequest)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {

                user.Name = updateUserRequest.Name;
                user.Surname = updateUserRequest.Surname;
                user.Age = updateUserRequest.Age;
                user.Email = updateUserRequest.Email;
                user.Username = updateUserRequest.Username;
                user.Password = updateUserRequest.Password;
                user.Admin = updateUserRequest.Admin;
                user.Owner = updateUserRequest.Owner;

                await dbContext.SaveChangesAsync();
                return Ok(user);

            }

            return NotFound();

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                ////////////////////////////// CAMINO AGENDA ////////////////////////////
                var agendas = _contextAgenda.Agendas.Where(a => a.UserId == id).ToList();
                if (agendas.Any())
                {
                    _contextAgenda.Agendas.RemoveRange(agendas);
                    await _contextAgenda.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO MENSAJES ////////////////////////////
                var messages = _contextMessage.Messages.Where(m => m.UserId == id).ToList();
                if (messages.Any())
                {
                    _contextMessage.Messages.RemoveRange(messages);
                    await _contextMessage.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO PUBLICACIÓN ////////////////////////////
                var publications = _contextPublication.Publications.Where(p => p.UserId == id).ToList();
                if (publications.Any())
                {
                    foreach (var publication in publications)
                    {
                        var messagesPublication = _contextMessage.Messages.Where(m => m.PublicationId == publication.Id).ToList();
                        if (messagesPublication.Any())
                        {
                            _contextMessage.Messages.RemoveRange(messagesPublication);
                            await _contextMessage.SaveChangesAsync();
                        }
                    }
                    _contextPublication.Publications.RemoveRange(publications);
                    await _contextPublication.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO EVENTO ////////////////////////////
                var eventos = _contextEvento.Eventos.Where(e => e.UserId == id).ToList();
                if (eventos.Any())
                {
                    foreach (var evento in eventos)
                    {
                        var agendasEventos = _contextAgenda.Agendas.Where(a => a.EventoId == evento.Id).ToList();
                        if (agendasEventos.Any())
                        {
                            _contextAgenda.Agendas.RemoveRange(agendasEventos);
                            await _contextAgenda.SaveChangesAsync();
                        }

                        var publicationsEventos = _contextPublication.Publications.Where(p => p.EventoId == evento.Id).ToList();
                        if (publicationsEventos.Any())
                        {
                            foreach (var publicationEvento in publicationsEventos)
                            {
                                var messagesPublicationsEventos = _contextMessage.Messages.Where(m => m.PublicationId == publicationEvento.Id).ToList();
                                if (messagesPublicationsEventos.Any())
                                {
                                    _contextMessage.Messages.RemoveRange(messagesPublicationsEventos);
                                    await _contextMessage.SaveChangesAsync();
                                }
                            }
                            _contextPublication.Publications.RemoveRange(publicationsEventos);
                            await _contextPublication.SaveChangesAsync();
                        }
                    }
                    _contextEvento.Eventos.RemoveRange(eventos);
                    await _contextEvento.SaveChangesAsync();
                }

                // Finalmente, eliminar el usuario
                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}