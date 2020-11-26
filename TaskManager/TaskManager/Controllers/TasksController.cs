using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
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

            return View();
        }

        // GET: One Task

        public ActionResult Show(int id)
        {
            Task task = db.Tasks.Find(id);
            ViewBag.Task = task;
            return View();
        }

        // GET pt New

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