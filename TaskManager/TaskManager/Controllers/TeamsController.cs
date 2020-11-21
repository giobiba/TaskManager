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
                ViewBag.Team = team;
                return View();
            }
            catch (Exception e)
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
            db.Teams.Add(team);
            db.SaveChanges();
            return RedirectToAction("Index");
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
                }

                return RedirectToAction("Index");
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

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}