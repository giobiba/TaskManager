using Microsoft.AspNet.Identity;
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
            ViewBag.Task = task;
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
    }
}