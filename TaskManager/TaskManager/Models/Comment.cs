using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class Comment
    {
        [Key]
        public int id_com { get; set; }

        public string UserId { get; set; }
        public int id_tsk { get; set; }
        [Required(ErrorMessage = "Orice comentariu trebuie sa aiba continut")]
        public string Content { get; set; }

        public virtual Task Task { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}