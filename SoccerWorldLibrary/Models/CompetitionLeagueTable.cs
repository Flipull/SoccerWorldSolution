using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SoccerWorld.Models
{
    public class StandingOrderComparer : IComparer<CompetitionLeagueTable>
    {
        private bool backwards_sort;
        public StandingOrderComparer(bool backwards = false)
        {
            backwards_sort = backwards;
        }

        public int Compare(CompetitionLeagueTable x, CompetitionLeagueTable y)
        {
            int result = CompareGeneral(x, y);
            if (backwards_sort)
                return result * -1;
            else
                return result;
        }


        private int CompareGeneral(CompetitionLeagueTable x, CompetitionLeagueTable y)
        {
            if (x.Points > y.Points)
                return -1;
            if (x.Points < y.Points)
                return 1;
            if (x.GoalsFor - x.GoalsAgainst > y.GoalsFor - y.GoalsAgainst)
                return -1;
            if (x.GoalsFor - x.GoalsAgainst < y.GoalsFor - y.GoalsAgainst)
                return 1;
            if (x.GoalsFor > y.GoalsFor)
                return -1;
            if (x.GoalsFor < y.GoalsFor)
                return 1;
            return 0;
        }
    }

    public class CompetitionLeagueTable
    {

        [Key]
        public int Id { get; set; }

        //[Index("EventSeasonAndClub", Order = 0, IsUnique = true)]
        public int Season { get; set; }

        //[Index("Competition")]
        [ForeignKey("Competition"), Required]
        public int CompetitionId { get; set; }
        public virtual Competition Competition { get; set; }

        //[Index("EventSeasonAndClub", Order = 1, IsUnique = true)]
        [ForeignKey("CompetitionEvent"), Required]
        public int CompetitionEventId { get; set; }
        public virtual CompetitionEvent CompetitionEvent { get; set; }

        //[Index("EventSeasonAndClub", Order = 2, IsUnique = true)]
        [ForeignKey("Club"), Required]
        public int ClubId { get; set; }
        public virtual Club Club { get; set; }

        [Required]
        public int Won { get; set; } = 0;
        [Required]
        public int Tied { get; set; } = 0;
        [Required]
        public int Lost { get; set; } = 0;

        [Required]
        public int GoalsFor { get; set; } = 0;
        [Required]
        public int GoalsAgainst { get; set; } = 0;

        [Required]
        public int Points { get; set; } = 0;


        
        static public IEnumerable<CompetitionLeagueTable> GetStandingsBackwards(CompetitionEvent comp_event, int season)
        {
            var list = SoccerWorldDatabaseContext.GetService().CompetitionLeagueTable
                                                    .Where(o => o.Season == season && 
                                                            o.CompetitionEventId == comp_event.Id)
                                                    .Include(o => o.Club)
                                                    .ToList();
            list.Sort(new StandingOrderComparer(backwards: true));
            return list;
        }
        static public IEnumerable<CompetitionLeagueTable> GetStandings(CompetitionEvent comp_event, int season)
        {
            var list = SoccerWorldDatabaseContext.GetService().CompetitionLeagueTable
                                                    .Where(o => o.Season == season &&
                                                            o.CompetitionEventId == comp_event.Id)
                                                    .Include(o => o.Club)
                                                    .ToList();
            list.Sort(new StandingOrderComparer(backwards: false));
            return list;
        }

        static public IEnumerable<CompetitionLeagueTable> GetStandingsBackwards(Competition comp, int season)
        {
            var first_tableitem = SoccerWorldDatabaseContext.GetService().CompetitionLeagueTable
                                                                    .Where(o => o.Season == season && 
                                                                            o.CompetitionId == comp.Id)
                                                                    .Include(o => o.CompetitionEvent)
                                                                    .First();
            CompetitionEvent first_event = first_tableitem.CompetitionEvent;
            return
                GetStandingsBackwards(first_event, season);
        }
        static public IEnumerable<CompetitionLeagueTable> GetStandings(Competition comp, int season)
        {
            var first_tableitem = SoccerWorldDatabaseContext.GetService().CompetitionLeagueTable
                                                                    .Where(o => o.Season == season && 
                                                                            o.CompetitionId == comp.Id)
                                                                    .Include(o => o.CompetitionEvent)
                                                                    .FirstOrDefault();
            if (first_tableitem == null)
                return new List<CompetitionLeagueTable>();

            CompetitionEvent first_event = first_tableitem.CompetitionEvent;
            return GetStandings(first_event, season);
        }
        static public IEnumerable<CompetitionLeagueTable> GetStandings(int competition_id)
        {
            var comp = SoccerWorldDatabaseContext.GetService().Competitions.Where(c => c.Id == competition_id).First();
            return GetStandings(comp, (int)comp.Country.Season);
        }

    }
}