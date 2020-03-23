using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class Competition
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Country"), Required]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        [Required]
        public int PromoteToParentCompetition { get; set; } = 0;
        //[ForeignKey("ParentCompetition")]
        public int? ParentCompetitionId { get; set; }
        [NotMapped]
        public Competition ParentCompetition
        {
            get
            {
                var a = WorldState.GetDatabaseContext().Competitions.Where(o => o.Id == 0);
                
                if (ParentCompetitionId.HasValue)
                    return WorldState.GetDatabaseContext().Competitions
                        .Where(o => o.Id == ParentCompetitionId)
                        .Include(o => o.Country)
                        .Include(o => o.Clubs)
                        .FirstOrDefault();
                else return null;
            }
            set
            {
                ParentCompetitionId = value.Id;
            }
        }

        [Required]
        public int RelegateToChildCompetition { get; set; } = 0;
        //[ForeignKey("ChildCompetition")]
        public int? ChildCompetitionId { get; set; }
        [NotMapped]
        public Competition ChildCompetition
        {
            get
            {
                return WorldState.GetDatabaseContext().Competitions
                        .Where(o => o.Id == ChildCompetitionId)
                        .Include(o => o.Country)
                        .Include(o => o.Clubs)
                        .FirstOrDefault();
            }
            set
            {
                ChildCompetitionId = value.Id;
            }
        }

        public virtual ICollection<Club> Clubs { get; set; }
        
        [MinLength(1), MaxLength(32), Required]
        public virtual string Name { get; set; }

        public virtual void OnSeasonEnd()
        {
            DoPromotion();
            DoRelegation();
        }

        protected virtual void DoPromotion()
        {
            //if hasParent and AmountToPromote > 0
            if (ParentCompetitionId != null && PromoteToParentCompetition > 0)
            {
                //select top x of the league to promote
                var targets =
                    CompetitionLeagueTable.GetStandings(this, (int)Country.Season).Take(PromoteToParentCompetition);

                foreach (var item in targets)
                    item.Club.Competition = ParentCompetition;
                WorldState.GetDatabaseContext().SaveChanges();
            }
        }

        protected virtual void DoRelegation()
        {
            //if hasChild and AmountToRelegate > 0
            if (ChildCompetitionId != null && RelegateToChildCompetition > 0)
            {
                //select bottom x of the league to relegate
                var targets =
                    CompetitionLeagueTable.GetStandingsBackwards(this, (int)Country.Season).Take(RelegateToChildCompetition);
                
                foreach (var item in targets)
                    item.Club.Competition = ChildCompetition;
                WorldState.GetDatabaseContext().SaveChanges();
            }
        }


    }
}