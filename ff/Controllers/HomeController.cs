using ff.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace fin.Controllers
{

    
    public class HomeController : Controller
    {
        private evEntities1 db = new evEntities1();
        public ActionResult Index()
        {
            if (Session["email"] != null)
            {
                return RedirectToAction("Index", "user");
                
            }
            else
                return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(mail mail)
        {
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 465,
                EnableSsl = true,
         //       DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential()
                {
                    UserName = "larkemyacine21@gmail.com",
                    Password = "Realmadrid1"
                }

            };
            MailAddress fromEmail = new MailAddress("larkemyacine21@gmail.com", "yacine larkem");
            MailAddress ToEmail = new MailAddress(mail.emailDestinataire, "someone");
            MailMessage message = new MailMessage()
            {
                From = fromEmail,
                Subject = mail.sujet,
                Body = mail.contenue,
                IsBodyHtml = false

            };
            message.To.Add(ToEmail);
            try
            {
                client.Send(message);
                ViewBag.Mail = "Le message a ete bien envoye";
                return View();
            }
            catch(Exception e)
            {
                ViewBag.Mail = "something wrong" + e.Message + "Error";
                return View();
            }
         

          
        }

        public ActionResult Send()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(mail mail)
        {
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 465,
                EnableSsl = true,
          //      DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "larkemyacine21@gmail.com",
                    Password = "Realmadrid1"
                }

            };
            MailAddress fromEmail = new MailAddress("larkemyacine21@gmail.com", "client");
            MailAddress ToEmail = new MailAddress("larkemyacine21@gmail.com", "yacine larkem");
            MailMessage message = new MailMessage()
            {
                From = fromEmail,
                Subject = mail.sujet,
                Body = mail.contenue,


            };
            message.To.Add(ToEmail);
            try
            {
                client.Send(message);
                MessageBox.Show("sent succesfully", "Done");
            }
            catch (Exception e)
            {
                MessageBox.Show("something wrong" + e.Message, "Error");
            }


            return RedirectToAction("Index", "Home");
        }

        public ActionResult signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult signup(personne p)
        {
           

            if (db.personnes.Any(x => x.email == p.email))
            {
                ViewBag.Notification = "le compte est existant";
                return View();
            }
            else
            {


                Session["mail"] = p.email.ToString();
                db.personnes.Add(p);
                db.SaveChanges();
              
            


            }
           personne pe = db.personnes.Find(Session["mail"].ToString());
         
          
            db.users.Add(new user()
            {
                personneEmail = Session["mail"].ToString()
            }); 
                db.SaveChanges();
            return RedirectToAction("Index", "Home");
        
        }

        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(personne p)
        {
           
            var checkLogin = db.personnes.Where(x => x.email.Equals(p.email) && x.motDePasse.Equals(p.motDePasse)).FirstOrDefault();
   

                if (checkLogin != null)
            {

                Session["e"] = p.email.ToString();
               
            }
           
            else if (checkLogin == null)
            {
               
                ViewBag.Message = "email ou mot de passe invalide !";
                return View();
               
            }
            if (db.administrateurs.Any(x => x.personneEmail == p.email))
            {
                Session["email"] = "admin";
            }
            else
            {
                Session["email"] = p.email.ToString();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Profil(string id)
        {
            id = Session["e"].ToString();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            personne personne = db.personnes.Find(id);
            if (personne == null)
            {
                return HttpNotFound();

            }
            return View(personne);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            personne personne = db.personnes.Find(id);
            if (personne == null)
            {
                return HttpNotFound();
            }

            return View(personne);
        }

        // POST: personnes/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(personne personne)
        {
            if (db.personnes.Any(x => x.email == personne.email))
            {
                db.Entry(personne).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(personne);
        }

        public ActionResult logOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}