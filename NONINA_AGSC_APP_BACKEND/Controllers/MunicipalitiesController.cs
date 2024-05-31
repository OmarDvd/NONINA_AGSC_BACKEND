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
    public class MunicipalitiesController : Controller
    {
        private readonly NoninabackendContext _context;

        public MunicipalitiesController(NoninabackendContext context)
        {
            _context = context;
        }

        // GET: Municipalities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Municipalities.ToListAsync());
        }

        // GET: Municipalities/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var municipality = await _context.Municipalities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (municipality == null)
            {
                return NotFound();
            }

            return View(municipality);
        }

        // GET: Municipalities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Municipalities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Municipality municipality)
        {
            if (ModelState.IsValid)
            {
                _context.Add(municipality);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(municipality);
        }

        // GET: Municipalities/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var municipality = await _context.Municipalities.FindAsync(id);
            if (municipality == null)
            {
                return NotFound();
            }
            return View(municipality);
        }

        // POST: Municipalities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] Municipality municipality)
        {
            if (id != municipality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(municipality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MunicipalityExists(municipality.Id))
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
            return View(municipality);
        }

        // GET: Municipalities/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var municipality = await _context.Municipalities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (municipality == null)
            {
                return NotFound();
            }

            return View(municipality);
        }

        // POST: Municipalities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var municipality = await _context.Municipalities.FindAsync(id);
                if (municipality == null)
                {
                    return NotFound();
                }

                // Buscar los eventos que sean de ese municipio
                var eventos = _context.Eventos.Where(e => e.MunicipalityId == id).ToList();
                if (eventos.Any())
                {
                    foreach (var evento in eventos)
                    {
                        // Buscar las agendas relacionadas con los eventos
                        var agendas = _context.Agendas.Where(a => a.EventoId == evento.Id).ToList();
                        if (agendas.Any())
                        {
                            // Eliminar las agendas relacionadas
                            _context.Agendas.RemoveRange(agendas);
                            await _context.SaveChangesAsync();
                        }

                        // Buscar las publicaciones relacionadas con los eventos
                        var publications = _context.Publications.Where(p => p.EventoId == evento.Id).ToList();
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
                            // Eliminar las publicaciones relacionadas con los eventos
                            _context.Publications.RemoveRange(publications);
                            await _context.SaveChangesAsync();
                        }
                    }
                    // Eliminar los eventos
                    _context.Eventos.RemoveRange(eventos);
                    await _context.SaveChangesAsync();
                }

                // Eliminar el municipio
                _context.Municipalities.Remove(municipality);
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

        private bool MunicipalityExists(long id)
        {
            return _context.Municipalities.Any(e => e.Id == id);
        }
    }
}
