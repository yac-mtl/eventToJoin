using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows.Forms;
using ff.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Page = System.Web.UI.Page;
//using Page = System.Windows.Forms.VisualStyles.VisualStyleElement.Page;


//using fin.modelEv;
//using fin.Models;

namespace fin.Controllers
{
    public class evenementsController : Controller
    {
       //private evenementEntities1 db1 = new evenementEntities1();
       
        private evEntities1 db = new evEntities1();

        // GET: evenements
        //afficher la liste des evenement d'un utilisateur donnee
        public ActionResult Index()
        {
            var evenements = db.evenements.Include(e => e.personne) ;
            return View(evenements.ToList());
        }
        //afficher tous les evenements
        public ActionResult liste()
        {
            var evenements = db.evenements.Include(e => e.personne);
            return View(evenements.OrderBy(c => c.dateDebut).ThenBy(c => c.personneEmail).ToList());
        }
        
             public ActionResult listeventArchives()
        {
            var evenements = db.evenements.Include(e => e.personne);
            return View(evenements.Where(c=>c.dateFin<DateTime.Now).OrderBy(c => c.dateDebut).ThenBy(c => c.personneEmail).ToList());
        }


        //afficher tous les evenement avec une autre forme pour la page user

        public ActionResult listeUser()
        {
            var evenements = db.evenements.Include(e => e.personne);
            return View(evenements.Where(c=> c.dateFin > DateTime.Now).OrderBy(c => c.dateDebut).ThenBy(c => c.personneEmail).ToList());
        }
        

             public ActionResult listeUserArchives()
        {
            var evenements = db.evenements.Include(e => e.personne);
            return View(evenements.Where(c => c.dateFin < DateTime.Now).OrderBy(c => c.dateDebut).ThenBy(c => c.personneEmail).ToList());
        }

        //liste des participations a venir 
        public ActionResult listParticipation()
        {
            int identifiant = verifierUser();
            Session["identifiant"] = identifiant;
            var evenements = from e in db.evenements
                             join pe in db.participant_evenement on e.idEvenement equals pe.evenementId
                             where pe.idUtilisateur == identifiant && e.dateFin >= DateTime.Now
                             select e;
       
            return View(evenements.ToList());
        }

        
        //liste archivee des participations
                  public ActionResult listArchives()
        {
            int identifiant = verifierUser();
            Session["identifiant"] = identifiant;
            var evenements = from e in db.evenements
                             join pe in db.participant_evenement on e.idEvenement equals pe.evenementId
                             where pe.idUtilisateur == identifiant && e.dateFin < DateTime.Now
                             select e;

            return View(evenements.ToList());
        }

        // GET: evenements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evenement evenement = db.evenements.Find(id);
            if (evenement == null)
            {
                return HttpNotFound();
            }
            return View(evenement);
        }

        // GET: evenements/Create
        public ActionResult Create()
        {
            List<SelectListItem> listType = new List<SelectListItem>
            {new  SelectListItem {Text = "Match",Value ="Match",Selected=true},
            new SelectListItem {Text= "Tournoi",Value="Tournoi"}
            };


            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom");
            return View();
        }

        // POST: evenements/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEvenement,dateDebut,dateFin,adresse,ville,province,pays,codePostal,sport,prix,nombreParticipant,heureDebut,heureFin,type,genre,dateCreation,dateSuppression,personneEmail")] evenement evenement)
        {
            if (ModelState.IsValid)
            {
                organisateur o = new organisateur(evenement.personneEmail);
                db.evenements.Add(evenement);
                db.organisateurs.Add(o);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", evenement.personneEmail);
            return View(evenement);
        }

        // GET: evenements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evenement evenement = db.evenements.Find(id);
            if (evenement == null)
            {
                return HttpNotFound();
            }
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", evenement.personneEmail);
            return View(evenement);
        }

        // POST: evenements/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEvenement,dateDebut,dateFin,adresse,ville,province,pays,codePostal,sport,prix,nombreParticipant,heureDebut,heureFin,type,genre,dateCreation,dateSuppression,personneEmail")] evenement evenement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evenement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", evenement.personneEmail);
            return View(evenement);
        }

        // GET: evenements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evenement evenement = db.evenements.Find(id);
            if (evenement == null)
            {
                return HttpNotFound();
            }
            return View(evenement);
        }

        // POST: evenements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            evenement evenement = db.evenements.Find(id);
     
            db.evenements.Remove(evenement);
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

        public ActionResult Rejoindre(int? id)
        {
            int identifiant = verifierUser();
            user u = db.users.Find(identifiant);
            int idParticipant = trouverParticipant(identifiant);
           // personne personne = db.personnes.Find(Session["email"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            evenement evenement = db.evenements.Find(id);
            if (evenement == null)
            {
                return HttpNotFound();
            }
            
           /* else if(personne.genre != evenement.genre || evenement.genre != "mixte")
            {
                ViewBag.AlertGenre = "vous pouvez pas rejoindre l'evenement, l'evenement est pour un autre sexe" + id;

                return View();
            }*/
       
            else if (evenement.nombreRejoint == evenement.nombreParticipant)
            {
               
                MessageBox.Show("l'evenement est complet");
                return RedirectToAction("Index", "Home");
            }
            else if (siDejaRejoint(identifiant, id))
            {
               ViewBag.Alert= "vous avez deja rejoint l'evenement avec l'id "+id;
               
                return View();
             

            }

            else
            {
                
             
                    participant p = new participant(DateTime.Now, identifiant);
                    db.participants.Add(p);
                    db.SaveChanges();
                    participant_evenement parti = new participant_evenement(p.idParticipant, id, u.idUtilisateur);
                    db.participant_evenement.Add(parti);
                    db.SaveChanges();
                    mofdifierEvenement(id);
                ViewBag.Alert = "vous venez de rejoindre l'evenement avec l'id " + id;
                return View();
      
            }
           
        }

        public ActionResult Payer(int? id)
        {


            return RedirectToAction("Index", "Home");
        }












            // ViewBag.personneEmail = new SelectList(db.personnes, "email", "nom", evenement.personneEmail);










        public   int verifierUser()
        {
            List<user> list = db.users.ToList();
            foreach (user user in list)
            {
                if (user.personneEmail.Equals(Session["e"].ToString()))
                {
                    int idParticipant = user.idUtilisateur;
                    return idParticipant;
                }
            }
            return 0;
        }

        public int trouverParticipant(int i)
        {
            List<participant> list = db.participants.ToList();
            foreach (participant p in list)
            {
                if (p.utilisateurIdUtilisateur == i)
                {
                    return p.idParticipant;
                }
            }
            return 0;

        }

        public participant findParticipant(int id)
        {
            List<participant> list = db.participants.ToList();
            foreach (participant p in list)
            {
                if (p.utilisateurIdUtilisateur == id)
                {
                    return p;
                }
            }
            return null;
        }

        public bool siDejaRejoint(int idParticipant, int? idEvenement)
        {
            // var check = db.participant_evenement.Where(c => c.IdParticipant.Equals(idParticipant) && c.evenementId.Equals(idEvenement)).FirstOrDefault();
            List<participant_evenement> listparticipant_Evenements = db.participant_evenement.ToList();

            foreach (participant_evenement pe in listparticipant_Evenements)
            {
                if (pe.idUtilisateur == idParticipant && pe.evenementId == idEvenement)
                {
                    return true;
                }
             }
            return false;
        }

       public void mofdifierEvenement(int? id)
        {

            // var std = db.evenements.SqlQuery("update evenement set nombreRejoint = +1 where idEvenement = 1011; ");
            evenement obj = db.evenements.Single(evenement => evenement.idEvenement == id);
              obj.nombreRejoint += 1;
           
              db.SaveChanges();

        }

        public ActionResult Quitter(int? id)
        {
            int identifiant = verifierUser();
            user u = db.users.Find(identifiant);
            int idParticipant = trouverParticipant(identifiant);
            evenement obj = db.evenements.Single(evenement => evenement.idEvenement == id);
            obj.nombreRejoint -= 1;
            participant_evenement pe = db.participant_evenement.Where(x => x.evenementId == id && x.idUtilisateur==u.idUtilisateur).Single<participant_evenement>();
            db.participant_evenement.Remove(pe);
           
            db.SaveChanges();
            ViewBag.Quitter = "vous venez de quitter l'evenement avec l'id " + id;

            return View();

        }

        public ActionResult Noter(int? id)
        {
           

         
              
        
                int identifiant = verifierUser();
                evenement obj = db.evenements.Single(evenement => evenement.idEvenement == id);
                obj.prix += 1;
         

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            

        }

        public void Messagebox(string xMessage)
        {
            Response.Write("<script>alert('" + xMessage + "')</script>");
        }

        private void MsgBox(string sMessage)
        {
            string msg = "<script language=\"javascript\">";
            msg += "alert('" + sMessage + "');";
            msg += "</script>";
            Response.Write(msg);
        }


    }
    }

