using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BugTracker.Models;
namespace BugTracker.Code
{
    public class Helper
    {
        private Controller controller;

        /**
    CheckCk()
    if the user cookie exists ExtendCk() will be called then usertype will be returned
     */
        public Helper(Controller ctrl)
        {
            controller = ctrl;
        }
        public string CheckCk()
        {
            controller.ViewBag.clrassignee = "no clearance";
            if (controller.Request.Cookies["user"] != null)
            {
                ExtendCk();
                if (controller.Request.Cookies["clearance"].Value.Equals("user"))
                {
                    controller.ViewBag.clrusr = Decrypt(DeCode(controller.Request.Cookies["user"].Value), controller.Request.Cookies["clearance"].Value);
                    return "user";
                }
                else if (controller.Request.Cookies["clearance"].Value.Equals("assignee"))
                {
                    controller.ViewBag.clrassignee = Decrypt(DeCode(controller.Request.Cookies["user"].Value), controller.Request.Cookies["clearance"].Value);
                    return "assignee";
                }
                else
                {
                    string admintype = Decrypt(DeCode(controller.Request.Cookies["user"].Value), controller.Request.Cookies["clearance"].Value);
                    if (admintype.Split('@')[0].Equals("master"))
                    {
                        controller.ViewBag.adm = "m";
                    }
                    else
                    {
                        controller.ViewBag.adm = admintype;
                    }
                    return "admin";
                }
            }
            else
            {
                return "no";
            }
        }
        /**
            ExtendCk() will set their expiration timers and update the cookies
         */
        private void ExtendCk()
        {
            //always called when cookie:name exists
            HttpCookie cu = controller.Request.Cookies["user"];
            HttpCookie cc = controller.Request.Cookies["clearance"];
            cu.Expires = DateTime.Now.AddMinutes(10);
            cc.Expires = DateTime.Now.AddMinutes(10);
            controller.Response.Cookies.Set(cu);
            controller.Response.Cookies.Set(cc);
        }
        public IQueryable<Assignee> Sort(IQueryable<Assignee> asg, string sortOrder)
        {
            switch (sortOrder)
            {
                case "asc":

                    controller.ViewData["order"] = "desc";
                    asg = asg.OrderBy(b => b.email);
                    break;
                case "desc":
                    controller.ViewData["order"] = "asc";
                    asg = asg.OrderByDescending(b => b.email);
                    break;
                default:
                    controller.ViewData["order"] = "desc";
                    asg = asg.OrderBy(b => b.email);
                    break;
            }
            return asg;
        }
        public IQueryable<Assignee> Search(IQueryable<Assignee> asg, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                asg = asg.Where(a => a.email.Contains(searchString));
            }
            controller.ViewData["searchString"] = searchString;
            return asg;
        }
        public IQueryable<User> Sort(IQueryable<User> usr, string sortOrder)
        {
            switch (sortOrder)
            {
                case "asc":

                    controller.ViewData["order"] = "desc";
                    usr = usr.OrderBy(b => b.email);
                    break;
                case "desc":
                    controller.ViewData["order"] = "asc";
                    usr = usr.OrderByDescending(b => b.email);
                    break;
                default:
                    controller.ViewData["order"] = "desc";
                    usr = usr.OrderBy(b => b.email);
                    break;
            }
            return usr;
        }
        public IQueryable<User> Search(IQueryable<User> usr, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                usr = usr.Where(a => a.email.Contains(searchString));
            }
            controller.ViewData["searchString"] = searchString;
            return usr;
        }
        public IQueryable<Admin> Sort(IQueryable<Admin> adm, string sortOrder)
        {
            switch (sortOrder)
            {
                case "asc":

                    controller.ViewData["order"] = "desc";
                    adm = adm.OrderBy(b => b.email);
                    break;
                case "desc":
                    controller.ViewData["order"] = "asc";
                    adm = adm.OrderByDescending(b => b.email);
                    break;
                default:
                    controller.ViewData["order"] = "desc";
                    adm = adm.OrderBy(b => b.email);
                    break;
            }
            return adm;
        }
        public IQueryable<Admin> Search(IQueryable<Admin> adm, string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                adm = adm.Where(a => a.email.Contains(searchString));
            }
            controller.ViewData["searchString"] = searchString;
            return adm;
        }
        public IQueryable<Bug> Sort(IQueryable<Bug> bug, string sortOrder, string sortColumn)
        {
            switch (sortColumn)
            {

                case "title":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.title);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.title);
                    }
                    controller.ViewData["sortColumn"] = "title";
                    break;
                case "state":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.state);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.state);
                    }
                    controller.ViewData["sortColumn"] = "state";
                    break;

                case "severity":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.severity);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.severity);
                    }
                    controller.ViewData["sortColumn"] = "severity";
                    break;
                case "submit":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.submit_time);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.submit_time);
                    }
                    controller.ViewData["sortColumn"] = "submit";
                    break;
                case "version":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.version);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.version);
                    }
                    controller.ViewData["sortColumn"] = "version";
                    break;

                case "fix":
                    if (sortOrder.Equals("asc"))
                    {
                        controller.ViewData["order"] = "desc";
                        bug = bug.OrderBy(b => b.fix_time);
                    }
                    else
                    {
                        controller.ViewData["order"] = "asc";
                        bug = bug.OrderByDescending(b => b.fix_time);
                    }
                    controller.ViewData["sortColumn"] = "fix";
                    break;
                default:
                    controller.ViewData["order"] = "desc";
                    controller.ViewData["sortColumn"] = "title";
                    bug = bug.OrderBy(b => b.title);
                    break;
            }
            return bug;
        }
        public IQueryable<Bug> Search(IQueryable<Bug> bug, string searchString, string filter)
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
            controller.ViewData["searchString"] = searchString;
            controller.ViewData["filter"] = filter;
            return bug;
        }

        //2 rail rail-fence cipher
        public string Encode(string input)
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
        public string DeCode(string input)
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
        public string Encrypt(string plainText, string password)
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
        public string Decrypt(string encryptedText, string password)
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

        public byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
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

        public byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
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


