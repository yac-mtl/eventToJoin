using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using ff.Models;

namespace ff.Controllers
{
    public class mailsController : Controller
    {
        private evEntities1 db = new evEntities1();

        // GET: mails
        public ActionResult Index()
        {
            var mails = db.mails.Include(m => m.personne);
            return View(mails.ToList());
        }

        // GET: mails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mail mail = db.mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // GET: mails/Create
        public ActionResult Create()
        {
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom");
            return View();
        }

        // POST: mails/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEmail,emailDestinataire,sujet,contenue,dateEnvoie,etat,personneEmail")] mail mail)
        {
            if (ModelState.IsValid)
            {
                SmtpClient client = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "larkemyacine21@gmail.com",
                        Password = "Realmadrid1"
                    }

                };
                MailAddress fromEmail = new MailAddress("larkemyacine21@gmail.com", "yacine larkem");
                MailAddress ToEmail = new MailAddress(mail.personneEmail, "someone");
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

            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", mail.personneEmail);
            return View(mail);
        }

        // GET: mails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mail mail = db.mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", mail.personneEmail);
            return View(mail);
        }

        // POST: mails/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEmail,emailDestinataire,sujet,contenue,dateEnvoie,etat,personneEmail")] mail mail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", mail.personneEmail);
            return View(mail);
        }

        // GET: mails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mail mail = db.mails.Find(id);
            if (mail == null)
            {
                return HttpNotFound();
            }
            return View(mail);
        }

        // POST: mails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            mail mail = db.mails.Find(id);
            db.mails.Remove(mail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
