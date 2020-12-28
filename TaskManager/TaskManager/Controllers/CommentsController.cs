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
    public class CommentsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            var userId = User.Identity.GetUserId();
            Team team = db.Teams.Find(db.Projects.Find(db.Tasks.Find(comm.id_tsk).id_pr).id_team);
            if (comm.UserId == userId || User.IsInRole("Admin") || (User.IsInRole("Organizator") && team.UserId == User.Identity.GetUserId()))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }
            else
            {
                TempData["message"] = "Comentariul nu a fost lasat de tine deci nu poti sa l stergi";
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }
                
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult New(Comment comm)
        {
            var userId = User.Identity.GetUserId(); // userul curent
            
            try
            {
                comm.UserId = userId;
                comm.User = db.Users.Find(userId);
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }
            catch
            {
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }

        }

        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            Comment comm = db.Comments.Find(id);
            Team team = db.Teams.Find(db.Projects.Find(db.Tasks.Find(comm.id_tsk).id_pr).id_team);
            if (comm.UserId == userId || User.IsInRole("Admin") || (User.IsInRole("Organizator") && team.UserId == User.Identity.GetUserId()))
            {
                ViewBag.Comment = comm;
                return View(comm);
            }
            else
            {
                TempData["message"] = "Comentariul nu a fost lasat de tine deci nu poti sa l editezi";
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }
                
        }

        [HttpPut]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Edit(int id, Comment RequestComment)
        {
            try
            {
                Comment comm = db.Comments.Find(id);
                if (TryUpdateModel(comm))
                {
                    comm.Content = RequestComment.Content;
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost modificat!";
                    return Redirect("/Tasks/Show/" + comm.id_tsk);
                }
                
                return View(comm);

            }
            catch
            {
                Comment comm = db.Comments.Find(id);
                TempData["message"] = "Comentariul nu a fost modificat!";
                return View(comm);
            }

        }
    }
}