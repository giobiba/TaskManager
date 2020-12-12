using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManager.Models
{
    public class UserTeams
    {
        [Key]
        public int id_ut { get; set; }
        public int id_team { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual Team Team { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}