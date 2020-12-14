using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;
using TaskManager;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualBasic.ApplicationServices;

namespace TaskManager.Controllers
{
    [Authorize(Roles = "User,Admin,Organizator")]
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
                ApplicationUser org = db.Users.Find(team.UserId);

                ViewBag.Projects = projects;
                ViewBag.Organizator = org;
                ViewBag.Users = from usr in db.Users
                                join usrteam in db.UserTeams
                                    on usr.Id equals usrteam.UserId
                                where usrteam.id_team == id
                                select usr;
                if (TempData.ContainsKey("message"))
                {
                    ViewBag.Message = TempData["Message"];
                }
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

            team.UserId = User.Identity.GetUserId();

            return View(team);
        }
        [HttpPost]
        public ActionResult New(Team team)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var userCurr = UserManager.FindById(User.Identity.GetUserId());
            var userOldRole = userCurr.Roles.FirstOrDefault().RoleId;
            var userRoleName = (from role in db.Roles where role.Id == userOldRole select role.Name).First();

            UserManager.RemoveFromRole(userCurr.Id, userRoleName); 
            UserManager.AddToRole(userCurr.Id, "Organizator");

            // trebuie un mod in care sa se dea automat refresh la rol

            team.UserId = User.Identity.GetUserId();
            if(ModelState.IsValid)
            {
                UserTeams ut = new UserTeams();
                ut.id_team = team.id_team;
                ut.UserId = team.UserId;

                db.Teams.Add(team);
                db.UserTeams.Add(ut);
                db.SaveChanges();

                TempData["Message"] = "Echipa a fost adaugata";
                return RedirectToAction("Index");
            }
            else
            {
                return View(team);
            }
        }

        public ActionResult NewUser(int id)
        {
 
            ViewBag.Id = id;
            return View();

        }

        [HttpPost]
        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult NewUser(int id, string Email) // id reprezinta idul echipei
        {
            try { 
                if(User.IsInRole("Organizator"))
                {
                    var currUserId = User.Identity.GetUserId();
                    var team = db.Teams.Find(id);

                    if(currUserId != team.UserId)
                    {
                        TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                        return RedirectToAction("Index");
                    }
                }
                UserTeams ut = new UserTeams();
                ut.id_team = id;
                var user_adaugat = from usr in db.Users
                        where usr.Email == Email
                        select usr;
                if(user_adaugat.Count() != 1)
                {
                    TempData["Message"] = "Nu exista utilizator cu acest email";
                    return RedirectToAction("Show/" + id.ToString());
                }
                ut.UserId = user_adaugat.First().Id;

                db.UserTeams.Add(ut);
                db.SaveChanges();
                TempData["Message"] = "Membrul a fost adaugata";
                return RedirectToAction("Show/" + id.ToString());
                }
            catch
            {
                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Edit(int id)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (User.IsInRole("Organizator"))
                {
                    var currUserId = User.Identity.GetUserId();

                    if (currUserId != team.UserId)
                    {
                        TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                        return RedirectToAction("Index");
                    }
                }
                if (User.IsInRole("Organizator"))
                {
                    var currUserId = User.Identity.GetUserId();

                    if (currUserId != team.UserId)
                    {
                        TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                        return RedirectToAction("Index");
                    }
                }
                return View(team);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (User.IsInRole("Organizator"))
                {
                    var currUserId = User.Identity.GetUserId();

                    if (currUserId != team.UserId)
                    {
                        TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                        return RedirectToAction("Index");
                    }
                }
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
        [Authorize(Roles = "Admin,Organizator")]
        public ActionResult Delete(int id)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (User.IsInRole("Organizator"))
                {
                    var currUserId = User.Identity.GetUserId();

                    if (currUserId != team.UserId)
                    {
                        TempData["Message"] = "Nu sunteti organizator pentru acest proiect";
                        return RedirectToAction("Index");
                    }
                }

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
        /*[NonAction]
        private IEnumerable<SelectListItem> GetAllUsers()
        {
            var selectList = new List<SelectListItem>();

            var users = from cat in db.Users select cat;

            foreach(var user in users)
            {
                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.Email.ToString()
                });
            }

            return selectList;
        }*/
    }
}