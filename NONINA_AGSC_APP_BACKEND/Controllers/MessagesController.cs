using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.Extensions.Logging;

namespace NONINA_AGSC_APP_BACKEND.Controllers
{
    public class MessagesController : Controller
    {
        private readonly NoninabackendContext _context;
        private readonly ILogger<EventoesController> _logger;

        public MessagesController(NoninabackendContext context, ILogger<EventoesController> logger)
        {
            _context = context;
            _logger = logger; // Inyecta el logger en el constructor
        }


        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var noninabackendContext = _context.Messages.Include(m => m.Publication).Include(m => m.User);
            return View(await noninabackendContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Publication)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create

        /*
        public IActionResult Create()
        {
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }
        */
        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PublicationId,Description")] Message message)
        {

            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            string errorMessage = "El modelo no es válido. Mostrando errores de validación. " +
                                  $"Parámetros del formulario: Id={message.Id}, UserId={message.UserId}, " +
                                  $"PublicationId={message.PublicationId}, Description={message.Description}";
            _logger.LogInformation(errorMessage);
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description", message.PublicationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", message.UserId);
            return View(message);
        }
        */
        
        // GET: Messages/Edit/5
        /*
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description", message.PublicationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", message.UserId);
            return View(message);
        }
        */
        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,PublicationId,Description")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Description", message.PublicationId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", message.UserId);
            return View(message);
        }
        */
        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Publication)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var message = await _context.Messages.FindAsync(id);
                if (message == null)
                {
                    return NotFound();
                }

                _context.Messages.Remove(message);
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


        private bool MessageExists(long id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
