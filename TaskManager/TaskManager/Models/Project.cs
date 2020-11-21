using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class Project
    {
        [Key]
        public int id_pr { get; set; }
        public int id_team { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date_St { get; set; }
        public DateTime Date_End { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public virtual Team Team { get; set; }

    }
}