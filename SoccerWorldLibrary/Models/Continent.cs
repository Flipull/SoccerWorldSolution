using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class Continent
    {
        [Key]
        public int Id { get; set; }

        [MinLength(1), MaxLength(32), Required]
        public string Name { get; set; }
        
        public virtual ICollection<Country> Countries { get; set; }
    }
}