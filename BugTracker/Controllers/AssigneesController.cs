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
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            ViewBag.msg = ustp;
            var asgn = _db.Assignee.AsQueryable();
            asgn = helper.Search(asgn, searchString);
            asgn = helper.Sort(asgn, sortOrder);
            //10 items per page
            return View(await PaginatedList<Assignee>.CreateAsync(asgn.AsNoTracking(), pageNumber ?? 1, 10));
        }

        public async Task<ActionResult> Stats(int? pageNumber)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("assignee"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.msg = ustp;
            var data = _db.Bug.AsQueryable();
            string asg = helper.Decrypt(helper.DeCode(Request.Cookies["user"].Value), Request.Cookies["clearance"].Value);
            ViewBag.total = data.Where(b => b.assignee.Equals(asg)).Count();
            ViewBag.open = data.Where(b => b.state.Equals("open") && b.assignee.Equals(asg)).Count();
            ViewBag.closed = data.Where(b => b.state.Equals("closed") && b.assignee.Equals(asg)).Count();
            data = data.Where(b => b.assignee.Equals(asg));
            data = data.OrderByDescending(x => x.submit_time);
            //10 items per page
            return View(await PaginatedList<Bug>.CreateAsync(data.AsNoTracking(), pageNumber ?? 1, 10));
        }
        // GET: Assignees/Create
        public ActionResult Create(string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            return View();
        }

        // POST: Assignees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "email,password")] Assignee assignee, string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;

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
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ustp.Equals("assignee"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            if (id == null)
            {
                return RedirectToAction("BadReq", "Error");
            }
            id = String.Format("{0}.com", id);
            Assignee assignee = await _db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(assignee);
        }

        // POST: Assignees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "email,password")] Assignee assignee, string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ustp.Equals("assignee"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
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
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            if (id == null)
            {
                return RedirectToAction("BadReq", "Error");
            }
            id = String.Format("{0}.com", id);
            Assignee assignee = await _db.Assignee.FindAsync(id);
            if (assignee == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(assignee);
        }

        // POST: Assignees/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id, string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            id = String.Format("{0}.com", id);
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
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
