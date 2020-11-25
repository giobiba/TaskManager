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

        public int id_user { get; set; }
        public int id_tsk { get; set; }
        public string Content { get; set; }

        public virtual Task Task { get; set; }
    }
}