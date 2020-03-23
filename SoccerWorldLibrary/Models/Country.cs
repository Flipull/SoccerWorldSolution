using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Continent"), Required]
        public int ContinentId { get; set; }
        public virtual Continent Continent { get; set; }

        [Range(1753, 9999)]
        public int? Season { get; set; }

        public virtual ICollection<Competition> Competitions { get; set; }

        [MinLength(1), MaxLength(32), Required]
        public string Name { get; set; }

        public void OnSeasonStart()
        {
            Season = WorldState.GetWorldState().CurrentDateTime.Year;
            WorldState.GetDatabaseContext().SaveChanges();
        }
    }
}