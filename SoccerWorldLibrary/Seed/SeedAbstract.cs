using SoccerWorld.Models;
using SoccerWorld.Models.CompetitionEvents;
using System;
using System.Collections.Generic;

namespace SoccerWorld.Migrations
{
    public abstract class SeedAbstract
    {
        protected SoccerWorldDatabaseContext Context;
        public SeedAbstract(SoccerWorldDatabaseContext db_context)
        {
            Context = db_context;
        }

        public void Seed()
        {
            var country = CreateCountry();
            var divisions = CreateCompetition(country);
            CreateCup(country);
            CreateClubs(divisions);
            
            Context.SaveChanges();
        }
        abstract protected Country CreateCountry();
        abstract protected List<Competition> CreateCompetition(Country country);
        abstract protected void CreateCup(Country country);
        abstract protected void CreateClubs(List<Competition> divisions);
        
        
        public void CreateDefaultPlayoffStructure(Competition parent, Competition child, DateTime start_day,
                                                  DateTime first_day, DateTime last_day,
                                                  int relegators_count, int promoters_count,
                                                  int goingup_count = 1)
        {
            Competition playoffs =
                Context.Competitions.Add(
                        new CompetitionPlayoffs()
                        {
                            Name = "Playoffs",
                            Country = parent.Country,
                            PromoteToParentCompetition = goingup_count,
                            ParentCompetition = parent,
                            ChildCompetition = child
                        }
                    )
                .Entity;

            Context.CompetitionEvents.Add(
                    new DefaultCompetitionPlayoffsStartEvent()
                    {
                        Name = "Halve finale",
                        Competition = playoffs,
                        Date = start_day,
                        AmountFromParentCompetition = relegators_count,
                        AmountFromChildCompetition = promoters_count,
                        FirstRound = first_day,
                        LastRound = last_day.AddDays(-3),
                        PlayTwicePerOpponent = true,
                        NextCompetitionEvent =
                            new DefaultCompetitionPlayoffsFinalEvent()
                            {
                                Name = "Finale",
                                Competition = playoffs,
                                FirstRound = last_day,
                                LastRound = last_day,
                                PlayTwicePerOpponent = false,
                                NextCompetitionEvent =
                                    new DefaultSeasonEndEvent()
                                    {
                                        Competition = playoffs,
                                        Name = "Seizoenseinde"
                                    }
                            }
                    });

        }
        
        public void CreateDefaultCompetitionEvents(Competition competition, DateTime start_day,
                                                DateTime first_day, DateTime last_day)
        {
            Context.CompetitionEvents.Add(
                new DefaultSeasonStartEvent()
                {
                    Name = "Seizoenstart",
                    Competition = competition,
                    Date = start_day,
                    NextCompetitionEvent =
                        new DrawDefaultCompetitionMatchesEvent()
                        {
                            Name = "Groepsfase",
                            Competition = competition,
                            FirstRound = first_day,
                            LastRound = last_day,
                            PlayDateDeviation = 2
                        }
                });
        }

        public void CreateDefaultClub(string name, Competition competition, int reputation)
        {
            Context.Clubs.Add(
                new Club()
                {
                    Name = name,
                    Competition = competition,
                    Country = competition.Country,
                    Reputation = reputation
                });
        }

        
    }
        
}