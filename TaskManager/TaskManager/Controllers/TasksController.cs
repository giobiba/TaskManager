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

            // de adaugat utilizatorii care au asignat task-ul

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        // GET: One Task

        public ActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
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
            else
                ViewBag.User = user.UserName;
            ViewBag.Task = task;
            if(TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            ViewBag.IsCompleted = false;
            if (task.Status == "Completed")
            {
                ViewBag.IsCompleted = true;
            }
            return View();
        }

        //get specific 
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult IndexSpecific(int id) // id proiect
        {

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
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
        public ActionResult New(int id)
        {
            var project = db.Projects.Find(id);
            Task task = new Task();
            task.id_pr = id;
            task.Project = project;
            return View(task);
        }

        // Post New Task

        [HttpPost]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult New(Task task)
        {

            var teamId = (from pr in db.Projects
                          where pr.id_pr == task.id_pr
                          select pr.id_team).First(); // selectam id-ul echipei la care vrem sa atasam proiectul
            var team = db.Teams.Find(teamId);
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                try
                {
                    task.Status = "Not started";
                    if (TryUpdateModel(task) && task.Date_St < task.Date_End)
                    {
                        db.Tasks.Add(task);
                        db.SaveChanges();
                        TempData["message"] = "Taskul a fost adaugat!";
                        return Redirect("/Tasks/IndexSpecific/" + task.id_pr);
                    }
                    if (task.Date_St > task.Date_End)
                        TempData["message"] = "Data de inceput trebuie sa fie inaintea datei de final";
                    var projects = from prj in db.Projects
                                   select prj;
                    ViewBag.Projects = projects;
                    TempData["message"] = "Taskul nu a putut fi adaugat";
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
            else
            {
                TempData["message"] = "Nu sunteti organizatorul bun";
                return Redirect("/Tasks/IndexSpecific/" + task.id_pr);
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
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId(); // userul curent

            Task task = db.Tasks.Find(id);
            var teamId = (from pr in db.Projects
                          where pr.id_pr == task.id_pr
                          select pr.id_team).First(); // selectam id-ul echipei de la care avem taskul
            var team = db.Teams.Find(teamId);
            
            var ut_exists = (from u in db.UserTeams
                     where u.id_team == teamId &&
                     u.UserId == userId
                     select u).Count() > 0; // este true daca userul cure

            if (ut_exists || User.IsInRole("Admin"))
            {
                ViewBag.Project = task.id_pr;
                var projects = from prj in db.Projects
                                select prj;
                ViewBag.Projects = projects;

                if (TempData.ContainsKey("message"))
                    ViewBag.Message = TempData["message"];


                string[] statusuri = { "Not started", "In progress", "Completed" };
                ViewBag.Status = statusuri;

                ViewBag.IsUser = false;
                if (team.UserId != userId)
                {
                    ViewBag.IsUser = true;
                }

                return View(task);
            }
            else
            {
                TempData["message"] = "Nu sunteti organizatorul corespunzator acestui task";
                return Redirect("/Tasks/IndexSpecific/" + task.id_pr);
            }
                
        }

        // Put Edited Task
        [HttpPut]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit (int id, Task requestTask)
        {
            try
            {
                Task task = db.Tasks.Find(id);
                var currOrg = (from prj in db.Projects // selectam organizatorul curent
                              join tm in db.Teams
                              on prj.id_team equals tm.id_team
                              select tm.UserId).First();

                if (currOrg != User.Identity.GetUserId()) // daca nu e organizatorul din echipa in care apartine taskul
                {
                    task.Status = requestTask.Status;
                    db.SaveChanges();
                    TempData["message"] = "Taskul a fost modificat!";
                    return RedirectToAction("Index");
                }
                if ( TryUpdateModel(task) && requestTask.Date_St < requestTask.Date_End) // cazul in care este organizator/ admin
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

                string[] statusuri = { "Not started", "In progress", "Completed" };
                ViewBag.Status = statusuri;

                return View(requestTask);
                
            }
            catch
            {
                var projects = from prj in db.Projects
                               select prj;
                ViewBag.Projects = projects;

                string[] statusuri = { "Not started", "In progress", "Completed" };
                ViewBag.Status = statusuri;

                return View(requestTask);
            }
        }

        //Delete Task
        [HttpDelete]
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            var teamId = (from pr in db.Projects
                          where pr.id_pr == task.id_pr
                          select pr.id_team).First(); // selectam id-ul echipei de la care avem taskul
            var team = db.Teams.Find(teamId);
            if (team.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
                TempData["message"] = "Taskul a fost sters!";
                return Redirect("/Tasks/IndexSpecific/" + task.id_pr);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti taskul";
                return Redirect("/Tasks/IndexSpecific/" + task.id_pr);
            }
            
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