using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
//TODO image değiştirirken veya eklerken bind içerisinde postta gelebiliyor mu bir bak
namespace BugTracker.Controllers
{
    public class BugsController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();

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
        public async Task<ActionResult> Index(string sortOrder, string sortColumn, string currentFilter,
 int? pageNumber, string searchString, string filter)
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
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            var bug = _db.Bug.Include(b => b.Assignee1).Include(b => b.User);
            bug = Search(bug, searchString, filter);
            bug = Sort(bug, sortOrder, sortColumn);
            //10 items per page
            return View(await PaginatedList<Bug>.CreateAsync(bug.AsNoTracking(), pageNumber ?? 1, 10));
        }
        public async Task<ActionResult> About(int? pageNumber)
        {
            IQueryable<BugStats> data =
                _db.Bug.GroupBy(x => DbFunctions.TruncateTime(x.submit_time)).Select(x => new BugStats()
                {
                    FileDate = x.Key,
                    FileCount = x.Count()
                });
            /* from bug in _db.Bug
             group bug by bug.submit_time into dateGroup
             select new BugStats()
             {
                 FileDate = dateGroup.Key,
                 FileCount = dateGroup.Count()
             };*/
            ViewBag.total = _db.Bug.Count();
            ViewBag.open = (from bug in _db.Bug
                            where bug.state.Equals("open")
                            select bug).Count();
            ViewBag.closed = (from bug in _db.Bug
                              where bug.state.Equals("closed")
                              select bug).Count();
            ViewBag.users = _db.User.Count();
            ViewBag.assignees = _db.Assignee.Count();
            data = data.OrderBy(x => x.FileDate);
            //10 items per page
            return View(await PaginatedList<BugStats>.CreateAsync(data.AsNoTracking(), pageNumber ?? 1, 10));
        }
        // GET: Bugs/Details/5
        public async Task<ActionResult> Details(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return HttpNotFound();
            }
            return View(bug);
        }

        // GET: Bugs/Create
        public ActionResult Create(string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            return View();
        }

        // POST: Bugs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "title,severity,version,description")] Bug bug, string prevPage, HttpPostedFileBase postedFile)
        {
            //TODO include will change 
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            Image img = FetchImg(postedFile,bug.title);
            bug.assignee = null;
            bug.submit_time = DateTime.Now;
            bug.fix_time = null;
            bug.state = "open";
            bug.submitter = HomeController.Decrypt(HomeController.DeCode(Request.Cookies["user"].Value), Request.Cookies["clearance"].Value);
            bug.Assignee1 = null;
            bug.fix_description = null;
            string mail = bug.submitter;
            bug.User = (from User in _db.User where User.email.Equals(mail) select User).DefaultIfEmpty(null).First();
            if (ModelState.IsValid)
            {
                _db.Bug.Add(bug);
                if (img != null)
                {
                    _db.Image.Add(img);
                }
                await _db.SaveChangesAsync();

                Response.Redirect(prevPage);
            }

            ViewBag.submitter = new SelectList(_db.User, "email", "password", bug.submitter);
            return View(bug);
        }

        // GET: Bugs/Edit/5
        public async Task<ActionResult> Edit(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> emails = new List<SelectListItem>();
            foreach (var item in await _db.Assignee.ToListAsync())
            {
                emails.Add(new SelectListItem { Text = item.email });
            }
            //display img from imgdataurl
            string imreBase64Data = Convert.ToBase64String(bug.Image.Data);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            ViewBag.imgurl = imgDataURL;
            ViewBag.assignee = emails;
            ViewBag.submitter = bug.submitter;
            return View(bug);
        }

        // POST: Bugs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "title,submitter,assignee,severity,state,submit_time,version,fix_time")] Bug bug, string prevPage)
        {
            //TODO include will change 
            //get filebase if null no change on img db and if there's change proceed with FetchImg method...
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            if (ModelState.IsValid)
            {
                if (bug.state.Equals("closed"))
                {
                    bug.fix_time = DateTime.Now;
                }
                else if (bug.state.Equals("open"))
                {
                    bug.fix_time = null;
                }
                else
                {
                    ViewBag.assignee = new SelectList(_db.Assignee, "email", "password", bug.assignee);
                    ViewBag.submitter = new SelectList(_db.User, "email", "password", bug.submitter);
                    return View(bug);
                }
                _db.Entry(bug).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                Response.Redirect(prevPage);
            }
            ViewBag.assignee = new SelectList(_db.Assignee, "email", "password", bug.assignee);
            ViewBag.submitter = new SelectList(_db.User, "email", "password", bug.submitter);
            return View(bug);
        }

        // GET: Bugs/Delete/5
        public async Task<ActionResult> Delete(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return HttpNotFound();
            }
            return View(bug);
        }

        // POST: Bugs/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            CheckCk();
            ViewBag.msg = GetTypeUsr();
            Bug bug = _db.Bug.Find(id);
            _db.Bug.Remove(bug);
            int saved = await _db.SaveChangesAsync();
            if (saved > 0)
            {
                Response.Redirect(prevPage);
            }
            return View("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
        /**
             GetTypeUsr() will check the cookies to return the user type
              */
        public string GetTypeUsr()
        {
            if (Request.Cookies["user"] == null)
            {
                return "no";
            }
            else
            {

                if (Request.Cookies["clearance"].Value.Equals("user"))
                {
                    return "user";
                }
                else if (Request.Cookies["clearance"].Value.Equals("assignee"))
                {
                    return "assignee";
                }
                else
                {
                    return "admin";
                }
            }
        }
        /**
            CheckCk()
            if the user cookie exists ExtendCk() will be called then true will be returned
            else false will be returned
             */
        private bool CheckCk()
        {
            if (Request.Cookies["user"] != null)
            {
                ExtendCk();
                return true;
            }
            else
            {
                return false;
            }
        }
        /**
            ExtendCk() will set their expiration timers and update the cookies
         */
        private void ExtendCk()
        {
            //always called when cookie:name exists
            HttpCookie cu = Request.Cookies["user"];
            HttpCookie cc = Request.Cookies["clearance"];
            cu.Expires = DateTime.Now.AddMinutes(1);
            cc.Expires = DateTime.Now.AddMinutes(1);
            Response.Cookies.Set(cu);
            Response.Cookies.Set(cc);
        }
        /**
         Sort() matches sortColumn with cases and then if there's a match sortOrder is used to determine the OrderBy/OrderByDescending statement,
            opposite value of the order is stored in ViewData because it behaves like a real world switch.
             */
        private IQueryable<Bug> Sort(IQueryable<Bug> bug, string sortOrder, string sortColumn)
        {
            switch (sortColumn)
            {

                case "title":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.title);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.title);
                    }
                    ViewData["sortColumn"] = "title";
                    break;
                case "state":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.state);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.state);
                    }
                    ViewData["sortColumn"] = "state";
                    break;

                case "severity":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.severity);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.severity);
                    }
                    ViewData["sortColumn"] = "severity";
                    break;
                case "submit":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.submit_time);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.submit_time);
                    }
                    ViewData["sortColumn"] = "submit";
                    break;
                case "version":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.version);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.version);
                    }
                    ViewData["sortColumn"] = "version";
                    break;

                case "fix":
                    if (sortOrder.Equals("asc"))
                    {
                        ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.fix_time);
                    }
                    else
                    {
                        ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.fix_time);
                    }
                    ViewData["sortColumn"] = "fix";
                    break;
                default:
                    ViewData["order"] = "desc";
                    ViewData["sortColumn"] = "title";
                    bug = bug.OrderBy(b => b.title);
                    break;
            }
            return bug;
        }
        /**
         Search() will first store its string parameteres in their respective ViewData locations.
            Checks if both filter and searchString have non-empty values then matches filter with the 
            switch cases to update bug accordingly then returns the updated bug
             */

        private IQueryable<Bug> Search(IQueryable<Bug> bug, string searchString, string filter)
        {
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(filter))
            {
                switch (filter)
                {
                    case "all":
                        bug = bug.Where(b => b.title.Contains(searchString) || b.severity.Contains(searchString) ||
                            b.state.Contains(searchString) || b.version.Contains(searchString));
                        break;
                    case "title":
                        bug = bug.Where(b => b.title.Contains(searchString));
                        break;
                    case "severity":
                        bug = bug.Where(b => b.severity.Contains(searchString));
                        break;
                    case "state":
                        bug = bug.Where(b => b.state.Contains(searchString));
                        break;
                    case "version":
                        bug = bug.Where(b => b.version.Contains(searchString));
                        break;
                }
            }
            ViewData["searchString"] = searchString;
            ViewData["filter"] = filter;
            return bug;
        }

        private Image FetchImg(HttpPostedFileBase postedFile, string btitle)
        {
            Image img = null;
            Debug.WriteLine("postedfile is null:" + (postedFile == null));
            if (postedFile != null)
            {
                bool extension = Path.GetExtension(postedFile.FileName).ToLower() == ".png";
                bool content = postedFile.ContentType.ToLower() == "image/png";
                Debug.WriteLine("Extension:" + (Path.GetExtension(postedFile.FileName).ToLower()) + "\tContent:" + (postedFile.ContentType.ToLower()));
                if (extension)
                {
                    Debug.WriteLine("IN EXTENSION");
                    if (content)
                    {
                        Debug.WriteLine("IN CONTENT");

                        Debug.WriteLine("ContentLength:" + postedFile.ContentLength);
                        if (postedFile.ContentLength <= 256000)
                        {
                            Debug.WriteLine("IN LENGTH");
                            byte[] bytes;
                            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                            {
                                bytes = br.ReadBytes(postedFile.ContentLength);
                            }
                            Debug.WriteLine("Bytes.len:" + bytes.Length);
                            img = new Image()
                            {
                                title = btitle,
                                Name = postedFile.FileName,
                                ContentType = postedFile.ContentType,
                                Data = bytes
                            };
                            Debug.WriteLine("\n-----------\nImg:\ntitle:" + img.title + "\nName" + img.Name + "\nContentType:" + img.ContentType +
                                "\n-----------");
                        }
                        else
                        {
                            //file size too big
                        }
                    }
                    else
                    {
                        //content not png
                    }
                }
                else
                {
                    //display message if file extension is not correct
                }
            }
            return img;
        }
    }
}
