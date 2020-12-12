using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManager.Models
{
    public class Team
    {
        [Key]
        public int id_team { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}