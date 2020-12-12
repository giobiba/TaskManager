using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TaskManager.Models
{
    public class UserTeams
    {
        [ForeignKey("Teams")]
        public int id_team { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}