using SoccerWorld.Migrations.SeedDatabase;
using SoccerWorld.Models;
using System;

namespace SoccerWorld
{

    public sealed class Seed
    {
        ////////////////
        static private readonly DateTime _Default_SimulationStart = new DateTime(2019, 5, 1, 0, 0, 0);
        static private readonly DateTime _Default_StartLeague = new DateTime(1753, 6, 9, 10, 0, 0);
        static private readonly DateTime _Default_FirstMatch = new DateTime(1753, 6, 27, 14, 30, 0);
        static private readonly DateTime _Default_LastMatch = new DateTime(1754, 5, 5, 21, 0, 0);
        static private readonly DateTime _Default_Playoffs_FirstMatch = new DateTime(1754, 5, 9, 14, 30, 0);
        static private readonly DateTime _Default_Playoffs_LastMatch = new DateTime(1754, 5, 15, 21, 0, 0);
        
        public static void RecreateDatabase(SoccerWorldDatabaseContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        public static void SeedDatabase(SoccerWorldDatabaseContext context)
        {
            context.WorldState.Add(
                new WorldState()
                {
                    AsyncProcessesCount = 0,
                    CurrentDateTime = _Default_SimulationStart
                });

            var eu = context.Continents.Add(new Continent() { Name = "Europa" });
            context.SaveChanges();
            
            new Engeland(context).Seed();
            new Nederland(context).Seed();

            /*            

                        var nld = context.Countries.Add(new Country() { Name = "Nederland", Continent = eu });
                        var div1 = context.Competitions.Add(
                            new Competition() 
                            { 
                                Name = "Eredivisie", 
                                Country = nld,
                                RelegateToChildCompetition = 1
                            });
                        var div2 = context.Competitions.Add(
                            new Competition() 
                            { 
                                Name = "Jupiler League", 
                                Country = nld,
                                PromoteToParentCompetition = 1
                            });
                        context.SaveChanges();

                        div1.ChildCompetition = div2;
                        div2.ParentCompetition = div1;
                        //
                        var nacomp =
                            new CompetitionPlayoffs()
                            {
                                ParentCompetition = div1,
                                ChildCompetition = div2,
                                PromoteToParentCompetition = 1,
                                Country = nld,
                                Name = "Nacompetitie"
                            };
                        var nacomp_events = new DefaultCompetitionPlayoffsStartEvent()
                        {
                            Name = "Halve finale",
                            Competition = nacomp,
                            AmountFromParentCompetition = 1,
                            AmountFromChildCompetition = 3,
                            FirstRound = _Default_Playoffs_FirstMatch,
                            LastRound = _Default_Playoffs_LastMatch.AddDays(-3),
                            //temp, because bug
                            PlayTwicePerOpponent = false,
                            NextCompetitionEvent =
                                new DefaultCompetitionPlayoffsFinalEvent()
                                {
                                    Name = "Finale",
                                    Competition = nacomp,
                                    FirstRound = _Default_Playoffs_LastMatch,
                                    LastRound = _Default_Playoffs_LastMatch,
                                    PlayTwicePerOpponent = false,
                                    NextCompetitionEvent =
                                        new DefaultSeasonEndEvent()
                                        {
                                            Name = "Seizoenseinde",
                                            Competition = div1,
                                            NextCompetitionEvent =
                                                new DefaultSeasonEndEvent()
                                                {
                                                    Name = "Seizoenseinde",
                                                    Competition = div2,
                                                    NextCompetitionEvent =
                                                        new DefaultSeasonEndEvent()
                                                        {
                                                            Name = "Seizoenseinde",
                                                            Competition = nacomp
                                                        }
                                                }
                                        }
                                }
                        };

                        context.CompetitionEvents.Add(
                            new DefaultSeasonStartEvent()
                            {
                                Name = "Seizoenstart",
                                Competition = div1,
                                Date = _Default_StartLeague,
                                NextCompetitionEvent =
                                    new DrawDefaultCompetitionMatchesEvent()
                                    {
                                        Name = "Groepsfase",
                                        Competition = div1,
                                        FirstRound = _Default_FirstMatch,
                                        LastRound = _Default_LastMatch,
                                        PlayDateDeviation = 2,
                                        NextCompetitionEvent = nacomp_events
                                    }
                            });

                        context.CompetitionEvents.Add(
                            new DefaultSeasonStartEvent()
                            {
                                Name = "Seizoenstart",
                                Competition = div2,
                                Date = _Default_StartLeague,
                                NextCompetitionEvent =
                                    new DrawDefaultCompetitionMatchesEvent()
                                    {
                                        Name = "Groepsfase",
                                        Competition = div2,
                                        FirstRound = _Default_FirstMatch.AddDays(-1),
                                        LastRound = _Default_LastMatch.AddDays(-1),
                                        PlayDateDeviation = 4,
                                    }
                            });
                        context.Clubs.Add(new Club() { Name = "Ajax", Competition = div1, Reputation = 12000 });
                        context.Clubs.Add(new Club() { Name = "PSV", Competition = div1, Reputation = 10000 });
                        context.Clubs.Add(new Club() { Name = "Feyenoord", Competition = div1, Reputation = 10000 });
                        context.Clubs.Add(new Club() { Name = "AZ", Competition = div1, Reputation = 8000 });
                        context.Clubs.Add(new Club() { Name = "Utrecht", Competition = div1, Reputation = 7000 });
                        context.Clubs.Add(new Club() { Name = "Willem II", Competition = div1, Reputation = 7000 });
                        context.Clubs.Add(new Club() { Name = "Heerenveen", Competition = div1, Reputation = 7000 });
                        context.Clubs.Add(new Club() { Name = "Vitesse", Competition = div1, Reputation = 7000 });
                        context.Clubs.Add(new Club() { Name = "Groningen", Competition = div1, Reputation = 7000 });
                        context.Clubs.Add(new Club() { Name = "Sparta", Competition = div1, Reputation = 6000 });
                        context.Clubs.Add(new Club() { Name = "Zwolle", Competition = div1, Reputation = 6000 });
                        context.Clubs.Add(new Club() { Name = "Heracles", Competition = div1, Reputation = 6000 });
                        context.Clubs.Add(new Club() { Name = "Den Haag", Competition = div1, Reputation = 5000 });
                        context.Clubs.Add(new Club() { Name = "RKC Waalwijk", Competition = div1, Reputation = 5000 });
                        context.Clubs.Add(new Club() { Name = "Twente", Competition = div1, Reputation = 5000 });
                        context.Clubs.Add(new Club() { Name = "FC Emmen", Competition = div1, Reputation = 5000 });
                        context.Clubs.Add(new Club() { Name = "Fortuna Sittard", Competition = div1, Reputation = 4000 });
                        context.Clubs.Add(new Club() { Name = "VVV", Competition = div1, Reputation = 4000 });

                        context.Clubs.Add(new Club() { Name = "Cambuur", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "De Graafschap", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "FC Volendam", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "Go Ahead Eagles", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "NAC", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "Telstar", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "Excelsior", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "NEC", Competition = div2, Reputation = 3500 });
                        context.Clubs.Add(new Club() { Name = "Almere", Competition = div2, Reputation = 3000 });
                        context.Clubs.Add(new Club() { Name = "Den Bosch", Competition = div2, Reputation = 3000 });
                        context.Clubs.Add(new Club() { Name = "FC Eindhoven", Competition = div2, Reputation = 3000 });
                        context.Clubs.Add(new Club() { Name = "MVV", Competition = div2, Reputation = 3000 });
                        context.Clubs.Add(new Club() { Name = "Roda", Competition = div2, Reputation = 3000 });
                        context.Clubs.Add(new Club() { Name = "FC Oss", Competition = div2, Reputation = 2500 });
                        context.Clubs.Add(new Club() { Name = "Helmond Sport", Competition = div2, Reputation = 2500 });
                        context.Clubs.Add(new Club() { Name = "Dordrecht", Competition = div2, Reputation = 2500 });
                        context.Clubs.Add(new Club() { Name = "DHSC", Competition = div2, Reputation = 1000 });
                        context.Clubs.Add(new Club() { Name = "GVV", Competition = div2, Reputation = 1000 });
                        context.Clubs.Add(new Club() { Name = "Hollandia", Competition = div2, Reputation = 1000 });
                        context.Clubs.Add(new Club() { Name = "Eemdijk", Competition = div2, Reputation = 1000 });
                        */
        }
    }
}
