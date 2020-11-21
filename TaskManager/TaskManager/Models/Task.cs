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
        public String Title { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
        public DateTime Date_St { get; set; }
        public DateTime Date_End { get; set; }
        public int id_us { get; set; }

        public virtual Project Project { get; set; }

        internal static Task FromResult(int v)
        {
            throw new NotImplementedException();
        }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}