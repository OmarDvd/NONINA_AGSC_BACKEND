using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly MessageAPIDbContext dbContext;

        public MessagesController(MessageAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }





        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetMessage([FromRoute] long id)
        {
            var message = await dbContext.Messages
                .Include(m => m.Publication)
                .Include(m => m.User)


                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var messageConNombres = new
            {
                message.Id,
                message.UserId,
                message.PublicationId,
                message.User.Username,
                message.Description,
                PublicationDescription = message.Publication.Description,
                PublicationUser = message.Publication.User.Username,
            };

            return Ok(messageConNombres);
        }


        [HttpPost]
        public async Task<IActionResult> AddMessage(AddMessageRequest addMessageRequest)
        {


            var message = new Message()
            {

                Id = addMessageRequest.Id,
                UserId = addMessageRequest.UserId,
                PublicationId = addMessageRequest.PublicationId,
                Description = addMessageRequest.Description

            };

            await dbContext.Messages.AddAsync(message);
            await dbContext.SaveChangesAsync();

            return Ok(message);
        }




        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] long id)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var message = await dbContext.Messages.FindAsync(id);
                if (message == null)
                {
                    return NotFound();
                }

                dbContext.Messages.Remove(message);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }

    }
