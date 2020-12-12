using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class Task
    {
        [Key]
        public int id_tsk { get; set; }
        public int id_pr { get; set; }
        [Required(ErrorMessage = "Campul Titlu este obligatoriu")]
        public String Title { get; set; }
        [Required(ErrorMessage = "Campul Descriere este obligatoriu")]
        public String Description { get; set; }
        [Required(ErrorMessage = "Campul status este obligatoriu")]
        public String Status { get; set; }
        public DateTime Date_St { get; set; }
        public DateTime Date_End { get; set; }
        public string UserId { get; set; }

        public virtual Project Project { get; set; }

        // user adauga task
        
        public virtual ApplicationUser User { get; set; }

        internal static Task FromResult(int v)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}