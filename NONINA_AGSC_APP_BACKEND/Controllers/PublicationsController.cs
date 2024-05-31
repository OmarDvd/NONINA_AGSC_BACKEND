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
    public class PublicationsController : Controller
    {
        private readonly NoninabackendContext _context;

        public PublicationsController(NoninabackendContext context)
        {
            _context = context;
        }

        // GET: Publications
        public async Task<IActionResult> Index()
        {
            var noninabackendContext = _context.Publications.Include(p => p.Evento).Include(p => p.User);
            return View(await noninabackendContext.ToListAsync());
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Evento)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // GET: Publications/Create


        /*
        public IActionResult Create()
        {
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }
        */
        // POST: Publications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,EventoId,Description")] Publication publication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Iitle", publication.EventoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", publication.UserId);
            return View(publication);
        }

        */

        // GET: Publications/Edit/5

        /*
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Title", publication.EventoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", publication.UserId);
            return View(publication);
        }

        */
        // POST: Publications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,EventoId,Description")] Publication publication)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
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
            ViewData["EventoId"] = new SelectList(_context.Eventos, "Id", "Title", publication.EventoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", publication.UserId);
            return View(publication);
        }

        */

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Evento)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var publication = await _context.Publications.FindAsync(id);
                if (publication == null)
                {
                    return NotFound();
                }

                // Buscar los mensajes relacionados con la publicación
                var messages = _context.Messages.Where(m => m.PublicationId == id).ToList();
                if (messages.Any())
                {
                    // Eliminar los mensajes relacionados
                    _context.Messages.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                }

                // Eliminar la publicación
                _context.Publications.Remove(publication);
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

        private bool PublicationExists(long id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
    }
}
