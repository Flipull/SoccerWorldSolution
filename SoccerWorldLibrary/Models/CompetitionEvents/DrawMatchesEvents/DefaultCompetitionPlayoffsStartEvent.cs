using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DefaultCompetitionPlayoffsStartEvent : DrawMatchesEvent
    {
        [Required]
        public int AmountFromParentCompetition { get; set; }
        [Required]
        public int AmountFromChildCompetition { get; set; }

        public override ICollection<Club> ChooseClubs()
        {
            //throw exception when Parent or Child Competition is null
            if (Competition.ParentCompetitionId == null || Competition.ChildCompetitionId == null)
                throw new ArgumentNullException();

            
            //if (Competition.ParentCompetition.RelegateToChildCompetition != Competition.ChildCompetition.PromoteToParentCompetition)
                //not supported for now, but maybe it can be useful to enable
                //throw new InvalidOperationException();
            
            //      pick clubs from both leagues
            ICollection<CompetitionLeagueTable> parent_div_league =
                CompetitionLeagueTable.GetStandingsBackwards(Competition.ParentCompetition, (int)Competition.Country.Season)
                                        .Skip(Competition.ParentCompetition.RelegateToChildCompetition)
                                        .Take(AmountFromParentCompetition)
                                        .ToList();
            ICollection<CompetitionLeagueTable> child_div_league =
                CompetitionLeagueTable.GetStandings(Competition.ChildCompetition, (int)Competition.Country.Season)
                                        .Skip(Competition.ChildCompetition.PromoteToParentCompetition)
                                        .Take(AmountFromChildCompetition)
                                        .ToList();

            List<Club> all_clubs = new List<Club>();
            all_clubs.AddRange(parent_div_league.Select<CompetitionLeagueTable, Club>(o => o.Club) );
            all_clubs.AddRange(child_div_league.Select<CompetitionLeagueTable, Club>(o => o.Club) );

            //choice-algorithm  (easy-way): floor club-count to a two-power number, to make a perfect knock-out round
            //INTRODUCES A PROMOTION BUG WHERE (all_clubs no power of 2? unbalanced chosen clubs from competition because of random chosen_amount)
            int i = 0;
            int clubcount = all_clubs.Count();
            while (clubcount > 1)
            {
                i++;
                clubcount >>= 1;
            }
            int chosen_amount = clubcount << i;
            
            all_clubs = SortRandom(all_clubs);
            var chosen_clubs = all_clubs.Take(chosen_amount);
            
            //      log clubs in CompetitionClubsRelation
            foreach (Club club in chosen_clubs)
            {
                var playing_in_competition_relation = new CompetitionClubRelation()
                {
                    Club = club,
                    Competition = Competition
                };
                WorldState.GetDatabaseContext().CompetitionClubRelations.Add(playing_in_competition_relation);
            }
            WorldState.GetDatabaseContext().SaveChanges();

            //      return clubs
            return chosen_clubs.ToList();
        }

        public override void GenerateMatches(ICollection<Club> participants)
        {
            //determine amount of matchrounds
            int match_rounds = 1;
            if (PlayTwicePerOpponent)
                match_rounds = 2;

            //determine matchdates
            DateTime[] dates = LerpDates(match_rounds, FirstRound, LastRound);

            //pseudo-random shuffling
            var club_listrandom = SortRandom(participants);
            Stack<Club> club_stack = new Stack<Club>(club_listrandom);
            //generate random 1-vs-1 games
            while (club_stack.Count > 1)
            {
                Club club1 = club_stack.Pop();
                Club club2 = club_stack.Pop();
                if (PlayTwicePerOpponent)
                {
                    Match match1 =
                        WorldState.GetDatabaseContext().Matches.Add(
                                new FirstReturnMatch()
                                {
                                    Season = Competition.Country.Season.Value,
                                    CompetitionEvent = this,
                                    Date = dates[0],
                                    IsCupMatch = false,
                                    HomeClub = club2,
                                    AwayClub = club1
                                }
                            ).Entity;
                    WorldState.GetDatabaseContext().SaveChanges();

                    WorldState.GetDatabaseContext().Matches.Add(
                                new SecondReturnMatch()
                                {
                                    Season = (int)Competition.Country.Season,
                                    CompetitionEvent = this,
                                    FirstMatchId = match1.Id,
                                    Date = dates[1],
                                    IsCupMatch = true,
                                    HomeClub = club1,
                                    AwayClub = club2
                                }
                            );
                } else
                {
                    WorldState.GetDatabaseContext().Matches.Add(
                        new Match()
                        {
                            Season = Competition.Country.Season.Value,
                            CompetitionEvent = this,
                            Date = dates[0],
                            IsCupMatch = true,
                            HomeClub = club2,
                            AwayClub = club1
                        });
                }

            }
            
            WorldState.GetDatabaseContext().SaveChanges();
        }

        public override void OnMatchComplete(Match match)
        {
            Club lost_club = match.MatchLoser();
            var relation = WorldState.GetDatabaseContext().CompetitionClubRelations.Where(
                        o => o.CompetitionId == CompetitionId &&
                                o.ClubId == lost_club.Id
                    ).First();
            relation.StillInCompetition = false;
            WorldState.GetDatabaseContext().SaveChanges();

            if (AreAllMatchesComplete())
            {
                //  matches are all done, this is the last time this function runs, so call the next event
                OnFinishedEvent();
                //call next event, if exists
                if (NextCompetitionEvent != null)
                    NextCompetitionEvent.Execute();
            }
        }

        public bool AreAllMatchesComplete()
        {
            var upcoming_match =
                WorldState.GetDatabaseContext().Matches
                                                .Where(o => o.CompetitionEventId == Id &&
                                                        o.HasEnded == false)
                                                .FirstOrDefault();
            return (upcoming_match == null);
        }


    }
}