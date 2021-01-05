using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskManager.Models
{
    public class Project
    {
        [Key]
        public int id_pr { get; set; }
        
        public int id_team { get; set; }

        [Display(Name = "Title"), Required(ErrorMessage = "Proiectul trebuie sa aiba un titlu")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Data_St"), Required(ErrorMessage = "Data de inceput este obligatorie")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date_St { get; set; }
        [Display(Name = "Data_End"), Required(ErrorMessage = "Data de final este obligatorie")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date_End { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
        public virtual Team Team { get; set; }

        public IEnumerable<SelectListItem> Teams { get; set; }
    }
}