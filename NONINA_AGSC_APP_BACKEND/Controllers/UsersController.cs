using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NONINA_AGSC_APP_BACKEND.Models;

namespace NONINA_AGSC_APP_BACKEND.Controllers
{
    public class UsersController : Controller
    {
        private readonly NoninabackendContext _context;

        public UsersController(NoninabackendContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create

        /*
        public IActionResult Create()
        {
            return View();
        }

        */

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Age,Email,Username,Password,Admin,Owner")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        */

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Surname,Age,Email,Username,Password,Admin,Owner")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                ////////////////////////////// CAMINO AGENDA ////////////////////////////
                var agendas = _context.Agendas.Where(a => a.UserId == id).ToList();
                if (agendas.Any())
                {
                    _context.Agendas.RemoveRange(agendas);
                    await _context.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO MENSAJES ////////////////////////////
                var messages = _context.Messages.Where(m => m.UserId == id).ToList();
                if (messages.Any())
                {
                    _context.Messages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO PUBLICACIÓN ////////////////////////////
                var publications = _context.Publications.Where(p => p.UserId == id).ToList();
                if (publications.Any())
                {
                    foreach (var publication in publications)
                    {
                        var messagesPublication = _context.Messages.Where(m => m.PublicationId == publication.Id).ToList();
                        if (messagesPublication.Any())
                        {
                            _context.Messages.RemoveRange(messagesPublication);
                            await _context.SaveChangesAsync();
                        }
                    }
                    _context.Publications.RemoveRange(publications);
                    await _context.SaveChangesAsync();
                }

                ////////////////////////////// CAMINO EVENTO ////////////////////////////
                var eventos = _context.Eventos.Where(e => e.UserId == id).ToList();
                if (eventos.Any())
                {
                    foreach (var evento in eventos)
                    {
                        var agendasEventos = _context.Agendas.Where(a => a.EventoId == evento.Id).ToList();
                        if (agendasEventos.Any())
                        {
                            _context.Agendas.RemoveRange(agendasEventos);
                            await _context.SaveChangesAsync();
                        }

                        var publicationsEventos = _context.Publications.Where(p => p.EventoId == evento.Id).ToList();
                        if (publicationsEventos.Any())
                        {
                            foreach (var publicationEvento in publicationsEventos)
                            {
                                var messagesPublicationsEventos = _context.Messages.Where(m => m.PublicationId == publicationEvento.Id).ToList();
                                if (messagesPublicationsEventos.Any())
                                {
                                    _context.Messages.RemoveRange(messagesPublicationsEventos);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            _context.Publications.RemoveRange(publicationsEventos);
                            await _context.SaveChangesAsync();
                        }
                    }
                    _context.Eventos.RemoveRange(eventos);
                    await _context.SaveChangesAsync();
                }

                // Finalmente, eliminar el usuario
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
