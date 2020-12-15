﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "User,Organizator,Admin")]

    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Tasks
        public ActionResult Index()
        {
            var tasks = db.Tasks;
            ViewBag.Tasks = tasks;

            // de adaugat utilizatorii care au asignat task-ul

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.IsOrganizator = false;
            return View();
        }

        // GET: One Task

        public ActionResult Show(int id)
        {
            
            Task task = db.Tasks.Find(id);
            ApplicationUser user = db.Users.Find(task.UserId);

            if(task == null)
            {
                TempData["message"] = "Taskul respectiv nu exista";
                return Redirect("/Projects/Index");
            } 
            if(user == null)
            {
                ViewBag.User = "Acest task nu are utilizator";
            }
            ViewBag.Task = task;
            ViewBag.User = user.UserName;
            return View();
        }


        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult IndexSpecific(int id) // id proiect
        {
            try
            {
                var tasks = from tsk in db.Tasks
                            where tsk.id_pr == id
                            select tsk;
                ViewBag.Tasks = tasks;

                var teamId = (from pr in db.Projects
                              where pr.id_pr == id
                              select pr.id_team).First(); // selectam id-ul echipei care are proiectul
                var team = db.Teams.Find(teamId);

                if (team.UserId == User.Identity.GetUserId())
                {
                    ViewBag.IsOrganizator = true;
                }
                else
                    ViewBag.IsOrganizator = false;
                return View("Index");
            }
            catch
            {
                return Redirect("/Projects/Show/" + id);
            }
        }

        // GET pt New
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult New()
        {
            var projects = from prj in db.Projects
                           select prj;
            Task task = new Task();
            ViewBag.Projects = projects;
            return View(task);
        }

        // Post New Task

        [HttpPost]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult New(Task task)
        {
            try
            {
                if(TryUpdateModel(task) && task.Date_St < task.Date_End)
                {

                    db.Tasks.Add(task);
                    db.SaveChanges();
                    TempData["message"] = "Taskul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                if (task.Date_St > task.Date_End)
                    TempData["message"] = "Data de inceput trebuie sa fie inaintea datei de final";
                var projects = from prj in db.Projects
                               select prj;
                ViewBag.Projects = projects;
                return View(task);
            }
            catch
            {
                var projects = from prj in db.Projects
                               select prj;
                ViewBag.Projects = projects;
                return View(task);
            }
        }

        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult AddUserToTask(int id)
        {
            // aflam echipa curenta
            var idTeam = (from pr in db.Projects
                          join task in db.Tasks on pr.id_pr equals task.id_pr
                          select pr.id_team).First();
            //verific daca e organizatorul bun sau admin
            if (User.IsInRole("Organizator"))
            {
                Team team = db.Teams.Find(idTeam);
                Task task = db.Tasks.Find(id);

                if (task == null || team == null)
                {
                    TempData["message"] = "Taskul respectiv nu exista";
                    return Redirect("/Projects/Index");
                }
                if (team.UserId != User.Identity.GetUserId())
                {
                    TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                    return RedirectToAction("IndexSpecific/" + task.id_pr);
                }
            }

            ViewBag.IdTask = id;
            ViewBag.Users = GetAllUsers(idTeam);
            return View();
        }

        [HttpPut]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult AddUserToTask(int id, string user)
        {

            Task task = db.Tasks.Find(id);

            //verificam daca id-ul taskului este valid
            if(task == null)
            {
                TempData["Message"] = "Assignarea utilizatorului nu a putut fi facuta. Task-ul nu exista";
            }

            task.UserId = user;
            db.SaveChanges();
            return RedirectToAction("Show/" + id);
        }
        // Get Edit Task
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Edit(int id)
        {
            Task task = db.Tasks.Find(id);
            ViewBag.Project = task.id_pr;
            var projects = from prj in db.Projects
                           select prj;
            ViewBag.Projects = projects;

            if (TempData.ContainsKey("message"))
                ViewBag.Message = TempData["message"];

            return View(task);
        }

        // Put Edited Task
        [HttpPut]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Edit (int id, Task requestTask)
        {
            try
            {
                Task task = db.Tasks.Find(id);
                if ( TryUpdateModel(task) && requestTask.Date_St < requestTask.Date_End)
                {

                    task = requestTask;
                    db.SaveChanges();
                    TempData["message"] = "Taskul a fost modificat!";
                    return RedirectToAction("Index");
                }

                if (requestTask.Date_St > requestTask.Date_End)
                    TempData["message"] = "Data de inceput trebuie sa fie inaintea datei de final";

                var projects = from prj in db.Projects
                               select prj;
                ViewBag.Projects = projects;

                return View(requestTask);
                
            }
            catch
            {
                var projects = from prj in db.Projects
                               select prj;
                ViewBag.Projects = projects;
                return View(requestTask);
            }
        }

        //Delete Task
        [HttpDelete]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            TempData["message"] = "Taskul a fost sters!";
            return RedirectToAction("Index");
        }

        [NonAction]
        private IEnumerable<SelectListItem> GetAllUsers(int idTeam)
        {
            var selectList = new List<SelectListItem>();
            var Users = from ut in db.UserTeams
                        join us in db.Users on ut.UserId equals us.Id
                        where ut.id_team == idTeam
                        select us;
            foreach (var user in Users)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.UserName.ToString()
                });
            }

            return selectList;
        }
    }
}