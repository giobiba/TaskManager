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
            ViewBag.Projects = projects;
            return View();
        }

        // Post New Task

        [HttpPost]
        public ActionResult New(Task task)
        {
            try
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // Get Edit Task

        public ActionResult Edit(int id)
        {
            Task task = db.Tasks.Find(id);
            ViewBag.Task = task;
            ViewBag.Project = task.id_pr;
            var projects = from prj in db.Projects
                           select prj;
            ViewBag.Projects = projects;
            return View();
        }

        // Put Edited Task
        [HttpPut]
        public ActionResult Edit (int id, Task requestTask)
        {
            try
            {
                Task task = db.Tasks.Find(id);
                if ( TryUpdateModel(task))
                {
                    task.Title = requestTask.Title;
                    task.Description = requestTask.Description;
                    task.Status = requestTask.Status;
                    task.Date_St = requestTask.Date_St;
                    task.Date_End = requestTask.Date_End;
                    task.id_pr = requestTask.id_pr;
                    task.id_us = requestTask.id_us;
                    db.SaveChanges();

                }
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return View();
            }
        }

        //Delete Task
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}