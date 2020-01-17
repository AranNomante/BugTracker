using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private BugTrackerEntities _db = new BugTrackerEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.msg = CheckCk();
            return View();
        }


        public ActionResult Login()
        {
            string userType = CheckCk();
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
                    Value = Encode(Encrypt(flag.Email, flag.Title)),
                    Expires = DateTime.Now.AddMinutes(1),
                    HttpOnly = true
                };
                HttpCookie clr = new HttpCookie("clearance")
                {
                    Value = flag.Title,
                    Expires = DateTime.Now.AddMinutes(1),
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

        /**
               CheckCk()
               if the user cookie exists ExtendCk() will be called then usertype will be returned
                */
        private string CheckCk()
        {
            ViewBag.clrassignee = "no clearance";
            if (Request.Cookies["user"] != null)
            {
                ExtendCk();
                if (Request.Cookies["clearance"].Value.Equals("user"))
                {
                    return "user";
                }
                else if (Request.Cookies["clearance"].Value.Equals("assignee"))
                {
                    ViewBag.clrassignee = HomeController.Decrypt(HomeController.DeCode(Request.Cookies["user"].Value), Request.Cookies["clearance"].Value);
                    return "assignee";
                }
                else
                {
                    return "admin";
                }
            }
            else
            {
                return "no";
            }
        }
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
        //2 rail rail-fence cipher
        public static string Encode(string input)
        {
            char[] inputc = input.ToCharArray();
            string pasA = "";
            string pasB = "";
            bool x = true;
            foreach (char ch in inputc)
            {
                if (x == true)
                {
                    pasA += ch;
                    x = false;
                }
                else
                {
                    x = true;
                    pasB += ch;
                }
            }
            return pasA + pasB;
        }
        public static string DeCode(string input)
        {
            int lnl = (int)Math.Round((decimal)input.Length / 2);
            //Write Strings
            char[] ln1, ln2;
            ln1 = input.Substring(0, lnl).ToCharArray();
            ln2 = input.Substring(lnl).ToCharArray();
            //Writen strings, now seperate into one
            bool top = true;
            string output = "";
            int x, topx, botx;
            x = 1; topx = 0; botx = 0;
            while (x != input.Length + 1)
            {
                if (top == true)
                {
                    if (topx > ln1.Length - 1)
                    {
                        return output;
                    }
                    output += ln1[topx]; topx += 1; top = false;

                }
                else
                {
                    output += ln2[botx]; botx += 1; top = true;
                }
                x++;
            }
            return output;
        }
        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="plainText">String to be encrypted</param>
        /// <param name="password">Password</param>
        public static string Encrypt(string plainText, string password)
        {
            if (plainText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="encryptedText">String to be decrypted</param>
        /// <param name="password">Password used during encryption</param>
        /// <exception cref="FormatException"></exception>
        public static string Decrypt(string encryptedText, string password)
        {
            if (encryptedText == null)
            {
                return null;
            }

            if (password == null)
            {
                password = String.Empty;
            }

            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
