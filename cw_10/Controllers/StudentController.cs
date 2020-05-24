using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cw_10.Models;

namespace cw10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly s18158Context _context;

        public StudentController(s18158Context context)
        {
            _context = context;
        }

        // GET: Student
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var s18158Context = _context.Student.Include(s => s.IdEnrollmentNavigation);
            return View(await s18158Context.ToListAsync());
        }

        // GET: Student/Details/5
        [HttpGet("{id}")]

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.IdEnrollmentNavigation)
                .FirstOrDefaultAsync(m => m.IndexNumber == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        
        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IndexNumber,FirstName,LastName,BirthDate,IdEnrollment")]
            Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdEnrollment"] =
                new SelectList(_context.Enrollment, "IdEnrollment", "IdEnrollment", student.IdEnrollment);
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("IndexNumber,FirstName,LastName,BirthDate,IdEnrollment")]
            Student student)
        {
            if (id != student.IndexNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.IndexNumber))
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

            ViewData["IdEnrollment"] =
                new SelectList(_context.Enrollment, "IdEnrollment", "IdEnrollment", student.IdEnrollment);
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudent(
            [Bind("IndexNumber,FirstName,LastName,BirthDate")]
            Student student,
            [FromQuery] int idStudy,
            [FromQuery] int semester,
            [FromQuery] DateTime startDate
        )
        {
            if (!_context.Studies.Any(s => s.IdStudy == idStudy))
            {
                return NotFound("Study type doesn't exist");
            }


            var enrollment = new Enrollment
            {
                IdStudy = idStudy,
                StartDate = startDate,
                Semester = semester
            };

            await _context.Enrollment.AddAsync(enrollment);

            student.IdEnrollment = enrollment.IdEnrollment;

            await _context.Student.AddAsync(student);
            await _context.SaveChangesAsync();

            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteStudents(int id)
        {
            var studiesExist = _context.Studies.Any(s => s.IdStudy == id);

            if (!studiesExist)
            {
                return NotFound();
            }

            var enrollments = _context.Enrollment.Include(e => e).Where(e => e.IdStudy == id).ToList();

            foreach (var enrollment in enrollments)
            {
                enrollment.Semester++;
                _context.Enrollment.Update(enrollment);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool StudentExists(string id)
        {
            return _context.Student.Any(e => e.IndexNumber == id);
        }
    }
}