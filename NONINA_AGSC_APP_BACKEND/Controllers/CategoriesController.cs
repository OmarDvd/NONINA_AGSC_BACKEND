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
    public class CategoriesController : Controller
    {
        private readonly NoninabackendContext _context;

        public CategoriesController(NoninabackendContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                // Buscar los eventos que sean de esa categoría
                var eventos = _context.Eventos.Where(e => e.CategoryId == id).ToList();
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

                        // Eliminar los eventos
                        _context.Eventos.RemoveRange(eventos);
                        await _context.SaveChangesAsync();
                    }
                }

                // Eliminar la categoría
                _context.Categories.Remove(category);
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









        private bool CategoryExists(long id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
