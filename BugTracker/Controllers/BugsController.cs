using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Code;
//TODO image değiştirirken veya eklerken bind içerisinde postta gelebiliyor mu bir bak
namespace BugTracker.Controllers
{
    public class BugsController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        private Helper helper;

        public BugsController()
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
            ViewBag.msg = helper.CheckCk();
            var bug = _db.Bug.Include(b => b.Assignee1).Include(b => b.User);
            bug = helper.Search(bug, searchString, filter);
            bug = helper.Sort(bug, sortOrder, sortColumn);
            //10 items per page
            return View(await PaginatedList<Bug>.CreateAsync(bug.AsNoTracking(), pageNumber ?? 1, 10));
        }
        public async Task<ActionResult> About(int? pageNumber)
        {
            ViewBag.msg = helper.CheckCk();
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
        public async Task<ActionResult> Details(int? id, string prevPage)
        {
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = helper.CheckCk();
            if (id == null)
            {
                return RedirectToAction("BadReq", "Error");
            }
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Image[] imgs = (from im in _db.Image where im.id_bug == bug.id select im).DefaultIfEmpty(null).ToArray();
            String[] imstrs = null;
            int[] imgid = null;
            if (imgs != null)
            {
                imgid = new int[imgs.Length];
                imstrs = new string[imgs.Length];
                int i = 0;
                foreach (Image img in imgs)
                {
                    if (img != null)
                    {
                        imgid[i] = img.id;
                        imstrs[i] = FetchImgStr(img);
                        i++;
                    }

                }
            }
            ViewBag.imgid = imgid;
            ViewBag.imgurl = imstrs;
            return View(bug);
        }

        // GET: Bugs/Create
        public ActionResult Create(string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("user"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            return View();
        }

        // POST: Bugs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "title,severity,version,description")] Bug bug, string prevPage, HttpPostedFileBase[] postedFile)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("user"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            Image[] img = FetchImg(postedFile, bug.id);
            bug.assignee = null;
            bug.submit_time = DateTime.Now;
            bug.fix_time = null;
            bug.state = "open";
            bug.submitter = helper.Decrypt(helper.DeCode(Request.Cookies["user"].Value), Request.Cookies["clearance"].Value);
            bug.Assignee1 = null;
            bug.fix_description = null;
            string mail = bug.submitter;
            bug.User = (from User in _db.User where User.email.Equals(mail) select User).DefaultIfEmpty(null).First();
            if (ModelState.IsValid)
            {
                _db.Bug.Add(bug);
                if (img != null)
                {
                    foreach (Image im in img)
                    {
                        if (im != null)
                        {

                            _db.Image.Add(im);
                        }
                    }
                }
                await _db.SaveChangesAsync();

                Response.Redirect(prevPage);
            }

            return View(bug);
        }

        // GET: Bugs/Edit/5
        public async Task<ActionResult> Edit(int? id, string prevPage)
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
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            List<SelectListItem> emails = new List<SelectListItem>();
            foreach (var item in await _db.Assignee.ToListAsync())
            {
                emails.Add(new SelectListItem { Text = item.email });
            }
            //display img from imgdataurl
            Image[] imgs = (from im in _db.Image where im.id_bug == bug.id select im).DefaultIfEmpty(null).ToArray();
            String[] imstrs = null;
            int[] imgid = null;
            if (imgs != null)
            {
                imgid = new int[imgs.Length]; 
                imstrs = new string[imgs.Length];
                int i = 0;
                foreach (Image img in imgs)
                {
                    if (img != null)
                    {
                        imgid[i] = img.id;
                        imstrs[i] = FetchImgStr(img);
                        i++;
                    }
                }
            }
            ViewBag.imgid = imgid;
            ViewBag.imgurl = imstrs;
            ViewBag.assignee = emails;
            ViewBag.submitter = bug.submitter;
            return View(bug);
        }

        // POST: Bugs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "id,title,submitter,assignee,severity,state,submit_time,version,fix_time,description,fix_description")] Bug bug, string prevPage, HttpPostedFileBase[] postedFile)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ustp.Equals("assignee"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            Image[] img = FetchImg(postedFile, bug.id);
            if (ModelState.IsValid)
            {
                if (bug.fix_description != null)
                {
                    bug.fix_time = DateTime.Now;
                    bug.state = "closed";
                }
                else if (bug.fix_description == null)
                {
                    bug.state = "open";
                    bug.fix_time = null;
                }
                else
                {
                    ViewBag.assignee = new SelectList(_db.Assignee, "email", "password", bug.assignee);
                    ViewBag.submitter = new SelectList(_db.User, "email", "password", bug.submitter);
                    return View(bug);
                }
                if (img != null)
                {
                    for (int i = 0; i < img.Length; i++)
                    {
                        if (img[i] != null)
                        {
                            String nm = img[i].Name;
                            Image q = _db.Image.FirstOrDefault(im => im.id_bug == bug.id && im.Name.Equals(nm));                          
                            if (q != null)
                            {
                                q.Data = img[i].Data;
                                q.ContentType = img[i].ContentType;
                                _db.Entry(q).State = EntityState.Modified;
                            }
                            else
                            {
                                _db.Image.Add(img[i]);
                            }
                        }
                    }
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
        public async Task<ActionResult> Delete(int? id, string prevPage)
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
            Bug bug = await _db.Bug.FindAsync(id);
            if (bug == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            return View(bug);
        }

        // POST: Bugs/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int? id, string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            Bug bug = _db.Bug.Find(id);
            _db.Bug.Remove(bug);
            int saved = await _db.SaveChangesAsync();
            if (saved > 0)
            {
                Response.Redirect(prevPage);
            }
            return View("Index");
        }
        // GET: Bugs/Delete/5
        public async Task<ActionResult> DeleteImg(int? id, string prevPage)
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
            Image img = await _db.Image.FindAsync(id);
            if (img == null)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Bug b= (from bug in _db.Bug where bug.id == img.id_bug select bug).FirstOrDefault();
            if (b != null)
            {
                ViewBag.imgasg = b.assignee;
            }
            ViewBag.imgurl = FetchImgStr(img);
            return View(img);
        }

        // POST: Bugs/Delete/5
        [HttpPost, ActionName("DeleteImg")]
        public async Task<ActionResult> DeleteImgConfirmed(int? id, string prevPage)
        {
            string ustp = helper.CheckCk();
            if (!ustp.Equals("admin") && !ustp.Equals("assignee"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.urlPrev = prevPage;
            ViewBag.msg = ustp;
            Image img = _db.Image.Find(id);
            _db.Image.Remove(img);
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



        private Image[] FetchImg(HttpPostedFileBase[] postedFiles, int bid)
        {
            Image[] img = null;
            Debug.WriteLine("postedfile is null:" + (postedFiles == null));
            if (postedFiles != null)
            {
                img = new Image[postedFiles.Length];
                int i = 0;
                foreach (HttpPostedFileBase postedFile in postedFiles)
                {
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
                                if (postedFile.ContentLength <= 1024 * 1024 * 8)//up to 8 mb 
                                {
                                    Debug.WriteLine("IN LENGTH");
                                    byte[] bytes;
                                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                                    {
                                        bytes = br.ReadBytes(postedFile.ContentLength);
                                    }
                                    Debug.WriteLine("Bytes.len:" + bytes.Length);
                                    Image imgs = new Image()
                                    {
                                        id_bug = bid,
                                        Name = postedFile.FileName,
                                        ContentType = postedFile.ContentType,
                                        Data = bytes
                                    };
                                    img[i] = imgs;
                                    i++;
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
                }
            }
            return img;
        }
        private string FetchImgStr(Image img)
        {
            string imreBase64Data = Convert.ToBase64String(img.Data);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            return imgDataURL;
        }
    }
}
