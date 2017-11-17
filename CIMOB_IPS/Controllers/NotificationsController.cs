using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CIMOB_IPS.Models;

namespace CIMOB_IPS.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly CIMOB_IPS_DBContext _context;

        public NotificationsController(CIMOB_IPS_DBContext context)
        {
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var cIMOB_IPS_DBContext = _context.Notification.Include(n => n.IdAccountNavigation);
            return View(await cIMOB_IPS_DBContext.ToListAsync());
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .Include(n => n.IdAccountNavigation)
                .SingleOrDefaultAsync(m => m.IdNotification == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["IdAccount"] = new SelectList(_context.Account, "IdAccount", "Email");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNotification,IdAccount,Description")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAccount"] = new SelectList(_context.Account, "IdAccount", "Email", notification.IdAccount);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification.SingleOrDefaultAsync(m => m.IdNotification == id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["IdAccount"] = new SelectList(_context.Account, "IdAccount", "Email", notification.IdAccount);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdNotification,IdAccount,Description")] Notification notification)
        {
            if (id != notification.IdNotification)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.IdNotification))
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
            ViewData["IdAccount"] = new SelectList(_context.Account, "IdAccount", "Email", notification.IdAccount);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .Include(n => n.IdAccountNavigation)
                .SingleOrDefaultAsync(m => m.IdNotification == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var notification = await _context.Notification.SingleOrDefaultAsync(m => m.IdNotification == id);
            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(long id)
        {
            return _context.Notification.Any(e => e.IdNotification == id);
        }
    }
}
