using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
namespace NONINA_AGSC_APP_BACKEND.Controllers
{
    public class EventoesController : Controller
    {
        private readonly NoninabackendContext _context;
        private readonly ILogger<EventoesController> _logger;

        public EventoesController(NoninabackendContext context, ILogger<EventoesController> logger)
        {
            _context = context;
            _logger = logger; // Inyecta el logger en el constructor
        }

        // GET: Eventoes
        public async Task<IActionResult> Index()
        {
            var noninabackendContext = _context.Eventos.Include(e => e.Category).Include(e => e.Municipality).Include(e => e.User);
            return View(await noninabackendContext.ToListAsync());
        }

        // GET: Eventoes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Category)
                .Include(e => e.Municipality)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Eventoes/Create

        /*
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["MunicipalityId"] = new SelectList(_context.Municipalities, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        */

        

        // POST: Eventoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateS([Bind("Id,Title,Description,PlaceLabel,PlaceCoordinates,Date,Time,MunicipalityId,CategoryId,UserId,ImageEvento")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", evento.CategoryId);
            ViewData["MunicipalityId"] = new SelectList(_context.Municipalities, "Id", "Name", evento.MunicipalityId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", evento.UserId);
            return View(evento);
        }

        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,PlaceLabel,PlaceCoordinates,Date,Time,MunicipalityId,CategoryId,UserId,ImageEvento")] Evento evento)
        {
            _logger.LogInformation("Iniciando el proceso de creación...");

            if (ModelState.IsValid)
            {
                _logger.LogInformation("El modelo es válido.");
                _context.Add(evento);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Datos guardados en la base de datos correctamente.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogInformation("El modelo no es válido. Mostrando errores de validación.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", evento.CategoryId);
                ViewData["MunicipalityId"] = new SelectList(_context.Municipalities, "Id", "Name", evento.MunicipalityId);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", evento.UserId);
                return View(evento);
            }
        }

        // GET: Eventoes/Edit/5

        /*
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", evento.CategoryId);
            ViewData["MunicipalityId"] = new SelectList(_context.Municipalities, "Id", "Name", evento.MunicipalityId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", evento.UserId);
            return View(evento);
        }
        */
        // POST: Eventoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Description,PlaceLabel,PlaceCoordinates,Date,Time,MunicipalityId,CategoryId,UserId,ImageEvento")] Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", evento.CategoryId);
            ViewData["MunicipalityId"] = new SelectList(_context.Municipalities, "Id", "Name", evento.MunicipalityId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", evento.UserId);
            return View(evento);
        }


        */
        // GET: Eventoes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.Category)
                .Include(e => e.Municipality)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var evento = await _context.Eventos.FindAsync(id);
                if (evento == null)
                {
                    return NotFound();
                }

                // Buscar las agendas relacionadas con el evento
                var agendas = _context.Agendas.Where(a => a.EventoId == id).ToList();
                if (agendas.Any())
                {
                    // Eliminar las agendas relacionadas
                    _context.Agendas.RemoveRange(agendas);
                    await _context.SaveChangesAsync();
                }

                // Buscar las publicaciones relacionadas con el evento
                var publications = _context.Publications.Where(p => p.EventoId == id).ToList();
                if (publications.Any())
                {
                    foreach (var publication in publications)
                    {
                        var messages = _context.Messages.Where(m => m.PublicationId == publication.Id).ToList();
                        if (messages.Any())
                        {
                            // Eliminar los mensajes relacionados
                            _context.Messages.RemoveRange(messages);
                            await _context.SaveChangesAsync();
                        }
                    }
                    // Eliminar las publicaciones relacionadas
                    _context.Publications.RemoveRange(publications);
                    await _context.SaveChangesAsync();
                }

                // Eliminar el evento
                _context.Eventos.Remove(evento);
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


        private bool EventoExists(long id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
