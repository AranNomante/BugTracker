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
using BugTracker.Code;
namespace BugTracker.Controllers
{
    public class ManualsController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        private Helper helper;
        public ManualsController()
        {
            helper = new Helper(this);
        }
        // GET: Manuals
        public async Task<ActionResult> Index()
        {
            string ustp = helper.CheckCk();
            ViewBag.msg = ustp;
            return View(await _db.Manuals.ToListAsync());
        }

        // GET: Manuals/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manuals manuals = await _db.Manuals.FindAsync(id);
            if (manuals == null)
            {
                return HttpNotFound();
            }
            return View(manuals);
        }

        // GET: Manuals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manuals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,text")] Manuals manuals)
        {
            if (ModelState.IsValid)
            {
                _db.Manuals.Add(manuals);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(manuals);
        }

        // GET: Manuals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manuals manuals = await _db.Manuals.FindAsync(id);
            if (manuals == null)
            {
                return HttpNotFound();
            }
            return View(manuals);
        }

        // POST: Manuals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,text")] Manuals manuals)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(manuals).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(manuals);
        }

        // GET: Manuals/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manuals manuals = await _db.Manuals.FindAsync(id);
            if (manuals == null)
            {
                return HttpNotFound();
            }
            return View(manuals);
        }

        // POST: Manuals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Manuals manuals = await _db.Manuals.FindAsync(id);
            _db.Manuals.Remove(manuals);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
