using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TeamsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Project
        public ActionResult Index()
        {
            var teams = db.Teams;

            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["Message"];
            }

            ViewBag.Teams = teams;
            return View();
        }

        public ActionResult Show(int id)
        {
            try
            {
                Team team = db.Teams.Find(id);
                var projects = from pr in db.Projects
                               where pr.id_team == team.id_team
                               select pr;

                ViewBag.Projects = projects;
                return View(team);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult New()
        {
            Team team = new Team();
            return View(team);
        }
        [HttpPost]
        public ActionResult New(Team team)
        {
            if(ModelState.IsValid)
            {
                db.Teams.Add(team);
                db.SaveChanges();

                TempData["Message"] = "Echipa a fost adaugata";
                return RedirectToAction("Index");
            }
            else
            {
                return View(team);
            }
        }


        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
            return View(team);
        }

        [HttpPut]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (TryUpdateModel(team))
                {
                    team.Name = requestTeam.Name;
                    db.SaveChanges();

                    TempData["Message"] = "Echipa a fost editata";
                    return RedirectToAction("Index");
                }

                return View(team);
            }
            catch
            {
                return View(requestTeam);
            }
        }


        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Team team = db.Teams.Find(id);

                db.Teams.Remove(team);
                db.SaveChanges();

                TempData["Message"] = "Echipa a fost stearsa";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Message"] = "Echipa nu a putut fi stearsa";
                return RedirectToAction("Index");
            }
        }

    }
}