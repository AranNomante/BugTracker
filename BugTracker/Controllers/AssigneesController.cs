using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class AssigneesController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        // GET: Assignees
        public async Task<ActionResult> Index()
        {
            return View(await db.Assignee.ToListAsync());
        }

        // GET: Assignees/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignee assignee = await db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return HttpNotFound();
            }
            return View(assignee);
        }

        // GET: Assignees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Assignees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "email,password")] Assignee assignee)
        {
            if (ModelState.IsValid)
            {
                db.Assignee.Add(assignee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(assignee);
        }

        // GET: Assignees/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignee assignee = await db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return HttpNotFound();
            }
            return View(assignee);
        }

        // POST: Assignees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "email,password")] Assignee assignee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(assignee);
        }

        // GET: Assignees/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignee assignee = await db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return HttpNotFound();
            }
            return View(assignee);
        }

        // POST: Assignees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Assignee assignee = await db.Assignee.FindAsync(id);
            db.Assignee.Remove(assignee);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
