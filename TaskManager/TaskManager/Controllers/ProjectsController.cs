using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class ProjectsController : Controller
    {

        // GET: Project
        public ActionResult Index()
        {
            var projects = db.Projects.Include("Team");

            ViewBag.Projects = projects;
            return View();
        }

        public ActionResult Show(int id)
        {
            try
            {
                Project project = db.Projects.Find(id);

                Team team = db.Teams.Find(project.id_team);

                ViewBag.Project = project;
                ViewBag.Team = team;
                return View();
            }
            catch (Exception e)
            {
                return View();
            }
        }


        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            var teams = from tms in db.Teams select tms;
            var currTeam = db.Teams.Find(project.id_team);
            ViewBag.Teams = teams;

            return View(project);
        }

        [HttpPut]
        public ActionResult Edit(int id, Project requestProject)
        {
            try
            {
                Project project = db.Projects.Find(id);

                if (TryUpdateModel(project))
                {
                    project = requestProject;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(requestProject);
            }
        }


        public ActionResult New()
        {
            var teams = from tms in db.Teams select tms;

            Project project = new Project();
            ViewBag.Teams = teams;
            return View(project);
        }
        [HttpPost]
        public ActionResult New(Project project)
        {
            db.Projects.Add(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Project project = db.Projects.Find(id);
                db.Projects.Remove(project);
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