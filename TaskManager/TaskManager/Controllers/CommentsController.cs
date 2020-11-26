using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class CommentsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Tasks/Show/" + comm.id_tsk);
        }

        [HttpPost]
        public ActionResult New(Comment comm)
        {
            try
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }
            catch (Exception e)
            {
                return Redirect("/Tasks/Show/" + comm.id_tsk);
            }

        }
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            ViewBag.Comment = comm;
            return View(comm);
        }

        [HttpPut]
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
            catch (Exception e)
            {
                Comment comm = db.Comments.Find(id);
                TempData["message"] = "Comentariul nu a fost modificat!";
                return View(comm);
            }

        }
    }
}