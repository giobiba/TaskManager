using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class Team
    {
        [Key]
        public int id_team { get; set; }
        public int id_org { get; set; } // foreign key catre useri

        public string Name { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}