using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ff.Models;

namespace ff.Controllers
{
    public class administrateursController : Controller
    {
        private evEntities1 db = new evEntities1();

        // GET: administrateurs
        public ActionResult Index()
        {
            var administrateurs = db.administrateurs.Include(a => a.administrateur_evenement).Include(a => a.personne);
            return View(administrateurs.ToList());
        }

        // GET: administrateurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            administrateur administrateur = db.administrateurs.Find(id);
            if (administrateur == null)
            {
                return HttpNotFound();
            }
            return View(administrateur);
        }

        // GET: administrateurs/Create
        public ActionResult Create()
        {
            ViewBag.idAdministrateur = new SelectList(db.administrateur_evenement, "administrateurId", "administrateurId");
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom");
            return View();
        }

        // POST: administrateurs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idAdministrateur,personneEmail")] administrateur administrateur)
        {
            var check = db.personnes.Find( administrateur.personneEmail);
            if (check == null)
            {
                ViewBag.administrateur = "vous devez creer l'utilisateur avant de l'ajouter comme administrateur";
                    return View();
            }



                else
                {
             //  administrateur admin = new administrateur(administrateur.personneEmail);
                    db.administrateurs.Add(administrateur);
                try
                {
                    db.SaveChanges();
                }
              catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
                }

                ViewBag.idAdministrateur = new SelectList(db.administrateur_evenement, "administrateurId", "administrateurId", administrateur.idAdministrateur);
                ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", administrateur.personneEmail);
                return View(administrateur);
        
        }

        // GET: administrateurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            administrateur administrateur = db.administrateurs.Find(id);
            if (administrateur == null)
            {
                return HttpNotFound();
            }
            ViewBag.idAdministrateur = new SelectList(db.administrateur_evenement, "administrateurId", "administrateurId", administrateur.idAdministrateur);
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", administrateur.personneEmail);
            return View(administrateur);
        }

        // POST: administrateurs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idAdministrateur,personneEmail")] administrateur administrateur)
        {
            if (ModelState.IsValid)
            {
                db.Entry(administrateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idAdministrateur = new SelectList(db.administrateur_evenement, "administrateurId", "administrateurId", administrateur.idAdministrateur);
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", administrateur.personneEmail);
            return View(administrateur);
        }

        // GET: administrateurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            administrateur administrateur = db.administrateurs.Find(id);
            if (administrateur == null)
            {
                return HttpNotFound();
            }
            return View(administrateur);
        }

        // POST: administrateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            administrateur administrateur = db.administrateurs.Find(id);
            db.administrateurs.Remove(administrateur);
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
