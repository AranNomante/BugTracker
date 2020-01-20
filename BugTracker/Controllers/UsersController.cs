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
    public class UsersController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        private Helper helper;
        public UsersController()
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
            var usr = _db.User.AsQueryable();
            usr = helper.Search(usr, searchString);
            usr = helper.Sort(usr, sortOrder);
            //10 items per page
            return View(await PaginatedList<User>.CreateAsync(usr.AsNoTracking(), pageNumber ?? 1, 10));
        }


        public async Task<ActionResult> Stats(int? pageNumber)
        {
            ViewBag.msg = helper.CheckCk();
            var data = _db.Bug.AsQueryable();
            string usr = helper.Decrypt(helper.DeCode(Request.Cookies["user"].Value), Request.Cookies["clearance"].Value);
            ViewBag.total = data.Where(b => b.submitter.Equals(usr)).Count();
            ViewBag.open = data.Where(b => b.state.Equals("open") && b.submitter.Equals(usr)).Count();
            ViewBag.closed = data.Where(b => b.state.Equals("closed") && b.submitter.Equals(usr)).Count();
            data = data.Where(b => b.submitter.Equals(usr));
            data = data.OrderByDescending(x => x.submit_time);
            //10 items per page
            return View(await PaginatedList<Bug>.CreateAsync(data.AsNoTracking(), pageNumber ?? 1, 10));
        }

        // GET: Users/Create
        public ActionResult Create(string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "email,password")] User user, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (ModelState.IsValid)
            {
                _db.User.Add(user);
                await _db.SaveChangesAsync();
                Response.Redirect(prevPage);
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = String.Format("{0}.com", id);
            User user = await _db.User.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "email,password")] User user, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (ModelState.IsValid)
            {
                _db.Entry(user).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                Response.Redirect(prevPage);
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = String.Format("{0}.com", id);
            User user = await _db.User.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id, string prevPage)
        {
            id = String.Format("{0}.com", id);
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            User user = await _db.User.FindAsync(id);
            _db.User.Remove(user);
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
