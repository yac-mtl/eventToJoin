using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ff.Models;

namespace ff.Controllers
{
    public class personnesController : Controller
    {
        private evEntities1 db = new evEntities1();

        // GET: personnes
        public ActionResult Index()
        {
            var personnes = db.personnes.Include(p => p.administrateur_personne);
            return View(personnes.ToList());
        }

        // GET: personnes/Details/5
        public ActionResult Details(string id)
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

        // GET: personnes/Create
        public ActionResult Create()
        {
            ViewBag.email = new SelectList(db.administrateur_personne, "personneEmail", "personneEmail");
            return View();
        }

        // POST: personnes/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( personne personne)
        {
            if (db.personnes.Any(x => x.email == personne.email))
            {
                ViewBag.Notification = "le compte est existant";
                return View();
            }
            else
            {
                Session["mail"] = personne.email.ToString();
                db.personnes.Add(personne);
                db.SaveChanges();
                


                db.users.Add(new user()
                {
                    personneEmail = Session["mail"].ToString()
                });
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
          

            ViewBag.email = new SelectList(db.administrateur_personne, "personneEmail", "personneEmail", personne.email);
            return View(personne);
        }
       

      

        // GET: personnes/Edit/5
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
            ViewBag.email = new SelectList(db.administrateur_personne, "personneEmail", "personneEmail", personne.email);
            return View(personne);
        }

        // POST: personnes/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nom,prenom,email,motDePasse,dateDeNaissance,genre,occupation,DateDeCreation")] personne personne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personne).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.email = new SelectList(db.administrateur_personne, "personneEmail", "personneEmail", personne.email);
            return View(personne);
        }

        // GET: personnes/Delete/5
        public ActionResult Delete(string id)
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

        // POST: personnes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            personne personne = db.personnes.Find(id);
            db.personnes.Remove(personne);
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
