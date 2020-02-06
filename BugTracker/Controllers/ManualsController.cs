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

  
        // GET: Manuals/Create
        public ActionResult Create()
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            var id = TempData["manid"];
            if (id != null)
            {
                ViewBag.manid = id;
            }
            ViewBag.msg = ustp;
            return View();
        }

        // POST: Manuals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "id,text")] Manuals manuals)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            if (ModelState.IsValid)
            {
                _db.Manuals.Add(manuals);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            }

            return View(manuals);
        }

        // GET: Manuals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            if (id == null)
            {
                return RedirectToAction("BadReq", "Error");
            }
            Manuals manuals = await _db.Manuals.FindAsync(id);
            if (manuals == null)
            {
                TempData["manid"] = id;
                return RedirectToAction("Create");
            }
            return View(manuals);
        }

        // POST: Manuals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "id,text")] Manuals manuals)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            if (ModelState.IsValid)
            {
                _db.Entry(manuals).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index","Home");
            }
            return View(manuals);
        }

        // GET: Manuals/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            if (id == null)
            {
                return RedirectToAction("BadReq", "Error");
            }
            Manuals manuals = await _db.Manuals.FindAsync(id);
            if (manuals == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(manuals);
        }

        // POST: Manuals/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ViewBag.adm == "m")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            Manuals manuals = await _db.Manuals.FindAsync(id);
            _db.Manuals.Remove(manuals);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index","Home");
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
