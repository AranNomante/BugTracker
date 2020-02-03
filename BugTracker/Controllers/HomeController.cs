using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Code;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        private Helper helper;
        public HomeController()
        {
            helper = new Helper(this);
        }
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.msg = helper.CheckCk();
            return View();
        }

        public ActionResult Login()
        {
            string userType = helper.CheckCk();
            ViewBag.msg = userType;
            if (userType.Equals("no"))
            {
                RedirectToAction("Index");
            }
            return View();
        }
        public void Logout()
        {
            var u = new HttpCookie("user");
            var c = new HttpCookie("clearance");
            u.Expires = DateTime.Now.AddDays(-1);
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(u);
            Response.Cookies.Add(c);
            Response.Redirect("/Home");
        }
        [HttpPost]
        public void Verify(Credentials crd)
        {

            var flag = GetUser(crd);
            if (flag == null)
            {
                flag = GetAssignee(crd);
                if (flag == null)
                {
                    flag = GetAssignee(crd);
                    if (flag == null)
                    {
                        flag = GetAdmin(crd);
                    }
                }
            }

            if (flag != null)
            {
                HttpCookie usr = new HttpCookie("user")
                {
                    Value = helper.Encode(helper.Encrypt(flag.Email, flag.Title)),
                    Expires = DateTime.Now.AddMinutes(10),
                    HttpOnly = true
                };
                HttpCookie clr = new HttpCookie("clearance")
                {
                    Value = flag.Title,
                    Expires = DateTime.Now.AddMinutes(10),
                    HttpOnly = true
                };
                Response.Cookies.Add(usr);
                Response.Cookies.Add(clr);
                Response.Redirect("/Home");
            }
            else
            {
                Response.Redirect("/Home/Login");
            }
        }

        private GenUser GetUser(Credentials crd)
        {
            User user = (from User in _db.User where User.email.Equals(crd.Email) && User.password.Equals(crd.Password) select User).DefaultIfEmpty(null).First();
            if (user == null)
            {
                return null;
            }
            GenUser usr = new GenUser();
            usr.Email = user.email;
            usr.Title = "user";
            return usr;
        }
        private GenUser GetAssignee(Credentials crd)
        {
            Assignee asg = (from Assignee in _db.Assignee where Assignee.email.Equals(crd.Email) && Assignee.password.Equals(crd.Password) select Assignee).DefaultIfEmpty(null).First();
            if (asg == null)
            {
                return null;
            }
            GenUser usr = new GenUser();
            usr.Email = asg.email;
            usr.Title = "assignee";
            return usr;
        }
        private GenUser GetAdmin(Credentials crd)
        {
            Admin adm = (from Admin in _db.Admin where Admin.email.Equals(crd.Email) && Admin.password.Equals(crd.Password) select Admin).DefaultIfEmpty(null).First();
            if (adm == null)
            {
                return null;
            }
            GenUser usr = new GenUser();
            usr.Email = adm.email;
            usr.Title = "admin";
            return usr;
        }
    }
}