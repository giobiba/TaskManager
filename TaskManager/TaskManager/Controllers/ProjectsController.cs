using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "User,Admin,Organizator")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Project
        public ActionResult Index()
        {
            ViewBag.IsOrganizator = false;
            if ( User.IsInRole("Admin"))
            {
                var projects = db.Projects.Include("Team");
                ViewBag.Projects = projects;
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var teams = from tems in db.Teams
                            join usrteam in db.UserTeams
                                on tems.id_team equals usrteam.id_team
                            where usrteam.UserId == userId
                            select tems;
                var projects = from prj in db.Projects
                               join tems in teams
                                    on prj.id_team equals tems.id_team
                               select prj;
                ViewBag.Projects = projects;
            }
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            
            return View();
        }

        public ActionResult Show(int id)
        {
            try
            {
                Project project = db.Projects.Find(id);

                if (project == null)
                {
                    TempData["message"] = "Proiectul nu exista";
                    return RedirectToAction("Index");
                }

                project.Team = db.Teams.Find(project.id_team);

                ViewBag.IsOrganizator = false;
                if (project.Team.UserId == User.Identity.GetUserId()) {
                    ViewBag.IsOrganizator = true;
                }

                return View(project);
            }
            catch
            {
                TempData["message"] = "Proiectul nu exista";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Edit(int id)
        {
            try
            {
                Project project = db.Projects.Find(id);

                if (project == null)
                {
                    TempData["message"] = "Proiectul nu exista";
                    return RedirectToAction("Index");
                }

                project.Teams = GetAllTeams();

                project.Team = db.Teams.Find(project.id_team);

                if (TempData.ContainsKey("message"))
                    ViewBag.Message = TempData["message"];

                return View(project);
            }
            catch
            {
                TempData["message"] = "Proiectul nu exista";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Edit(int id, Project requestProject)
        {
            try
            {
                if (ModelState.IsValid && requestProject.Date_St < requestProject.Date_End)
                {
                    Project project = db.Projects.Find(id);

                    if (project == null)
                    {
                        TempData["message"] = "Proiectul nu exista";
                        return RedirectToAction("Index");
                    }

                    if (TryUpdateModel(project))
                    {
                        project = requestProject;
                        db.SaveChanges();

                        TempData["message"] = "Proiectul a fost modificat cu succes";
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    requestProject.Teams = GetAllTeams();
                    requestProject.Team = db.Teams.Find(requestProject.id_team);

                    if (requestProject.Date_St > requestProject.Date_End)
                        TempData["message"] = "Data de inceput trebuie sa fie inaintea datei de final";

                    return View(requestProject);
                }
            }
            catch
            {
                requestProject.Teams = GetAllTeams();
                requestProject.Team = db.Teams.Find(requestProject.id_team);
                return View(requestProject);
            }
        }

        public ActionResult New()
        {
            Project project = new Project();
            project.Teams = GetAllTeams();

            if (TempData.ContainsKey("message"))
                ViewBag.Message = TempData["message"];

            return View(project);
        }
        [HttpPost]
        public ActionResult New(Project project)
        {
            if (ModelState.IsValid && project.Date_St < project.Date_End)
            {
                db.Projects.Add(project);
                db.SaveChanges();

                TempData["message"] = "Proiectul a fost adaugat cu succes";
                return RedirectToAction("Index");
            }
            else
            {
                if (project.Date_St > project.Date_End)
                    TempData["message"] = "Data de inceput trebuie sa fie inaintea datei de final";

                project.Teams = GetAllTeams();
                return View(project);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Delete(int id)
        {
            try
            {
                Project project = db.Projects.Find(id);
                db.Projects.Remove(project);
                db.SaveChanges();

                TempData["message"] = "Proiectul s-a sters cu succes";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["message"] = "Proiectul nu a fost sters";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        private IEnumerable<SelectListItem> GetAllTeams()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var teams = from cat in db.Teams
                             select cat;

            // iteram prin categorii
            foreach (var team in teams)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = team.id_team.ToString(),
                    Text  = team.Name.ToString()
                });
            }

            return selectList;
        }
    }
}