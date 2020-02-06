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


        // GET: Manuals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") )
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
                return RedirectToAction("NotFound","Error");
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
            if (!ustp.Equals("admin"))
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
