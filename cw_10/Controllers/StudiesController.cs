using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cw_10.Models;

namespace cw_10.Controllers
{
    public class StudiesController : Controller
    {
        private readonly s18158Context _context;

        public StudiesController(s18158Context context)
        {
            _context = context;
        }

        // GET: Studies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Studies.ToListAsync());
        }

        // GET: Studies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studies = await _context.Studies.FirstOrDefaultAsync(m => m.IdStudy == id);

            if (studies == null)
            {
                return NotFound();
            }

            return View(studies);
        }

        // GET: Studies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Studies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStudy,Name")] Studies studies)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studies);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studies);
        }

        // GET: Studies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studies = await _context.Studies.FindAsync(id);
            if (studies == null)
            {
                return NotFound();
            }
            return View(studies);
        }

        // POST: Studies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStudy,Name")] Studies studies)
        {
            if (id != studies.IdStudy)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudiesExists(studies.IdStudy))
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
            return View(studies);
        }

        // GET: Studies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studies = await _context.Studies
                .FirstOrDefaultAsync(m => m.IdStudy == id);
            if (studies == null)
            {
                return NotFound();
            }

            return View(studies);
        }

        // POST: Studies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studies = await _context.Studies.FindAsync(id);
            _context.Studies.Remove(studies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool StudiesExists(int id)
        {
            return _context.Studies.Any(e => e.IdStudy == id);
        }
    }
}
