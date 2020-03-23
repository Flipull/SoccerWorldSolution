using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DrawDefaultCompetitionMatchesEvent : DrawMatchesEvent
    {

        [Range(1, 16), Required]
        public int PouleAmount { get; set; } = 1;

        public override void Execute()
        {
            CreateCompetitionTable(ChooseClubs());
            base.Execute();
        }

        private void CreateCompetitionTable(ICollection<Club> participants)
        {
            foreach (Club team in participants)
            {

                CompetitionLeagueTable league_table = new CompetitionLeagueTable()
                {
                    Club = team,
                    Season = (int)Competition.Country.Season,
                    CompetitionEvent = this,
                    Competition = Competition
                };
                WorldState.GetDatabaseContext().CompetitionLeagueTable.Add(league_table);
            }
            WorldState.GetDatabaseContext().SaveChanges();
        }

        public override ICollection<Club> ChooseClubs()
        {
            return Competition.Clubs;
        }
        
        
        public override void GenerateMatches(ICollection<Club> participants)
        {
            int teamscount = participants.Count;
            
            if (teamscount % 2 != 0)
                throw new ArgumentOutOfRangeException();
            
            //decide amount of rounds
            int round_amount = 2 * (teamscount - 1);
            
            //decide dates, with some sparse distribution, based on day-name, and time from first and last round
            DateTime[] dates = LerpDates(round_amount, FirstRound, LastRound);

            //draw all 2*(n-1) play-rounds, round-robin, with date-deviations
            /*
             * arrange matches with team #1 and team#n-1,team #2 and team#n-2, ...., team#n/2-1 and team#n/2+1 
             * arrange matches with team #1+1 and team#n-2,team #2+1 and team#n-3, ...., team#n/2-2 and team#n/2 
             * arrange matches with team #1+2 and team#n-3,team #2+2 and team#n-4, ...., team#n/2-3 and team#n/2-1
             * n-1 amount of times
             * 
             * each round add team#n and team#n/2 too
             */
           var teamslist = SortRandom(participants);

            for (int round_loop = 0; round_loop < teamscount-1; round_loop++)
            {
                Match new_match;
                for (int match_loop = 0; match_loop < teamscount/2-1; match_loop++)
                {
                    int club1 = Modulo(0 + match_loop - round_loop, teamscount-1);
                    int club2 = Modulo(teamscount - 2 - match_loop - round_loop, teamscount-1);
                    if (round_loop%2 == 0)
                    {
                        new_match = CreateMatch(round_loop, dates[round_loop], teamslist[club1], teamslist[club2]);
                        WorldState.GetDatabaseContext().Matches.Add(new_match);
                        new_match = CreateMatch(teamscount - 1 + round_loop, dates[teamscount - 1 + round_loop], teamslist[club2], teamslist[club1]);
                        WorldState.GetDatabaseContext().Matches.Add(new_match);
                    }
                    else
                    {
                        new_match = CreateMatch(round_loop, dates[round_loop], teamslist[club2], teamslist[club1]);
                        WorldState.GetDatabaseContext().Matches.Add(new_match);
                        new_match = CreateMatch(teamscount - 1 + round_loop, dates[teamscount - 1 + round_loop], teamslist[club1], teamslist[club2]);
                        WorldState.GetDatabaseContext().Matches.Add(new_match);
                    }
                }
                int lastclub = teamscount - 1;
                int centerclub = Modulo(teamscount/2 - 1 - round_loop, teamscount - 1);
                if (round_loop % 2 == 0)
                {
                    new_match = CreateMatch(round_loop, dates[round_loop], teamslist[lastclub], teamslist[centerclub]);
                    WorldState.GetDatabaseContext().Matches.Add(new_match);
                    new_match = CreateMatch(teamscount - 1 + round_loop, dates[teamscount - 1 + round_loop], teamslist[centerclub], teamslist[lastclub]);
                    WorldState.GetDatabaseContext().Matches.Add(new_match);
                }
                else
                {
                    new_match = CreateMatch(round_loop, dates[round_loop], teamslist[centerclub], teamslist[lastclub]);
                    WorldState.GetDatabaseContext().Matches.Add(new_match);
                    new_match = CreateMatch(teamscount - 1 + round_loop, dates[teamscount - 1 + round_loop], teamslist[lastclub], teamslist[centerclub]);
                    WorldState.GetDatabaseContext().Matches.Add(new_match);
                }

            }
            WorldState.GetDatabaseContext().SaveChanges();
            

        }
        private Match CreateMatch(int? roundnr, DateTime date, Club homeclub, Club awayclub)
        {
            return new Match()
            {
                CompetitionEvent = this,
                RoundNumber = roundnr,
                Date = date,
                HomeClub = homeclub,
                AwayClub = awayclub,
                Season = (int)Competition.Country.Season
            };
        }
        
        public override void OnMatchComplete(Match match)
        {
            //write score of both clubs to the CompetitionLeagueTable SHOULD BE A CALLABLE FUNCTION IN CLUB?
            CompetitionLeagueTable homeclub_leagueitem = 
                    WorldState.GetDatabaseContext().CompetitionLeagueTable
                                                .Where(o => o.Season == match.Season &&
                                                        o.CompetitionEventId == Id &&
                                                        o.ClubId == match.HomeClubId)
                                                .First();
            homeclub_leagueitem.GoalsFor += (int)match.HomeScore;
            homeclub_leagueitem.GoalsAgainst += (int)match.AwayScore;

            if (match.HomeClubWinning())
            {
                homeclub_leagueitem.Won += 1;
                homeclub_leagueitem.Points += 3;
            }
            else if (match.ClubsBothTied())
            {
                homeclub_leagueitem.Tied += 1;
                homeclub_leagueitem.Points += 1;
            }
            else
                homeclub_leagueitem.Lost += 1;
            
            var awayclub_leagueitem = WorldState.GetDatabaseContext().CompetitionLeagueTable
                                                .First(o => o.Season == match.Season &&
                                                        o.CompetitionEventId == Id &&
                                                        o.ClubId == match.AwayClubId);
            awayclub_leagueitem.GoalsFor += (int)match.AwayScore;
            awayclub_leagueitem.GoalsAgainst += (int)match.HomeScore;

            if (match.AwayClubWinning())
            {
                awayclub_leagueitem.Won += 1;
                awayclub_leagueitem.Points += 3;
            }
            else if (match.ClubsBothTied())
            {
                awayclub_leagueitem.Tied += 1;
                awayclub_leagueitem.Points += 1;
            }
            else
                awayclub_leagueitem.Lost += 1;

            WorldState.GetDatabaseContext().SaveChanges();

            //if all matches complete
            var upcoming_match =
                WorldState.GetDatabaseContext().Matches
                                                .Where(o => o.CompetitionEventId == Id &&
                                                        o.HasEnded == false)
                                                .FirstOrDefault();

            if (upcoming_match == null)
            {
                //  matches are all done, this is the last time this function runs, so call the next event
                OnFinishedEvent();

                //call next event, if exists
                if (NextCompetitionEvent != null)
                    NextCompetitionEvent.Execute();
            }
        }


    }
}