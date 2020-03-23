using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class CompetitionEventHistory
    {
        [Key]
        public int Id { get; set; }

        //[Index("DateIndex", IsUnique = false), Required]
        public DateTime Date { get; set; } = WorldState.GetWorldState().CurrentDateTime;

        [ForeignKey("CompetitionEvent"), Required]
        public int CompetitionEventId { get; set; }
        public virtual CompetitionEvent CompetitionEvent { get; set; }

        [MaxLength(32), Required]
        public string Type { get; set; }
    }
}