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
        
        [HttpDelete]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Tasks/Show/" + comm.id_tsk);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizator,Admin")]
        public ActionResult New(Comment comm)
        {
            try
            {
                Task task = db.Tasks.Find(comm.id_tsk);
                if (task.Status == "Completed")
                {
                    TempData["message"] = "Taskul este finalizat si alte comentarii nu mai pot fi lasate";
                    return Redirect("/Tasks/Show/" + comm.id_tsk);
                }
                
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
            Comment comm = db.Comments.Find(id);

            ViewBag.Comment = comm;
            return View(comm);
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