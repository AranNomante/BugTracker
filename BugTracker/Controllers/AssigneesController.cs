using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Code;

namespace BugTracker.Controllers
{
    public class AssigneesController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        private Helper helper;

        public AssigneesController()
        {
            helper = new Helper(this);
        }
        // GET: Bugs
        /** 
            Flow:
                On page switch searchString is not sent from view to controller which means
            pageNumber will be incremented or decremented after the first load. When the URL is reached for the first time
            the pageNumber ?? 1 will set the pageNumber to 1. Also, if the URL is reached from the search bar then searchString will not be null
            meaning the pageNumber will start from 1.
            
            Currentfilter in ViewData will store searchString, CheckCk() will be run to extend the session if there is any and then ViewBag.msg will 
            store the type of the user. 

            Database will be accessed to get bugs as IQueryable
            Search() and Sort() will be called and then the view will be returned with a PaginatedList as its model
        */
        public async Task<ActionResult> Index(string sortOrder, string currentFilter,
 int? pageNumber, string searchString)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            ViewBag.msg = helper.CheckCk();
            var asgn = _db.Assignee.AsQueryable();
            asgn = helper.Search(asgn, searchString);
            asgn = helper.Sort(asgn, sortOrder);
            //10 items per page
            return View(await PaginatedList<Assignee>.CreateAsync(asgn.AsNoTracking(), pageNumber ?? 1, 10));
        }

        // GET: Assignees/Details/5
        public async Task<ActionResult> Details(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            //To be filled with statistic info
            /*
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignee assignee = await db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return HttpNotFound();
            }*/
            //return View(assignee);
            return View();
        }

        // GET: Assignees/Create
        public ActionResult Create(string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            return View();
        }

        // POST: Assignees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "email,password")] Assignee assignee, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();

            if (ModelState.IsValid)
            {
                _db.Assignee.Add(assignee);
                await _db.SaveChangesAsync();
                Response.Redirect(prevPage);
            }

            return View(assignee);
        }

        // GET: Assignees/Edit/5
        public async Task<ActionResult> Edit(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = String.Format("{0}.com", id);
            Assignee assignee = await _db.Assignee.FindAsync(id);
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
        public async Task<ActionResult> Edit([Bind(Include = "email,password")] Assignee assignee, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (ModelState.IsValid)
            {
                _db.Entry(assignee).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                Response.Redirect(prevPage);
            }
            return View(assignee);
        }

        // GET: Assignees/Delete/5
        public async Task<ActionResult> Delete(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = String.Format("{0}.com", id);
            Assignee assignee = await _db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return HttpNotFound();
            }
            return View(assignee);
        }

        // POST: Assignees/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id, string prevPage)
        {
            id = String.Format("{0}.com", id);
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            Assignee assignee = await _db.Assignee.FindAsync(id);
            _db.Assignee.Remove(assignee);
            int saved = await _db.SaveChangesAsync();
            if (saved > 0)
            {
                Response.Redirect(prevPage);
            }
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
