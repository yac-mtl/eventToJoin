
//using fin.Models;
using ff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fin.Controllers
{
    public class userController : Controller
    {

        private evEntities1 db = new evEntities1();
        // GET: user
        public ActionResult Index()
        {
            string session = Session["e"].ToString();
            if(session != null)
            {
                ViewBag.totalEvent = db.evenements.Where(u => u.personneEmail == session).Count();
                int identifiant = verifierUser();
                Session["identifiant"] = identifiant;
                var evenements = from e in db.evenements
                                 join pe in db.participant_evenement on e.idEvenement equals pe.evenementId
                                 where pe.idUtilisateur == identifiant && e.dateFin >= DateTime.Now
                                 select e;
                ViewBag.TotalParticipation = evenements.ToList().Count();
                return View();
            }
            return View();

        }

        public ActionResult calculerEvenement()
        {
           string session = Session["e"].ToString();

            ViewBag.totalEvent = db.evenements.Where(u => u.personneEmail == session).Count();
          

            return View();
        }






        public int verifierUser()
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
    }
}