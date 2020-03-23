using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class CompetitionClubRelation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Competition")]
        public int CompetitionId { get; set; }
        public virtual Competition Competition { get; set; }

        
        [ForeignKey("Club")]
        public int ClubId { get; set; }
        public virtual Club Club { get; set; }

        [Required]
        public bool StillInCompetition { get; set; } = true;

    }
}