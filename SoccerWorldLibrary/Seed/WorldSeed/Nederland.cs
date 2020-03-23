using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoccerWorld.Models;
using SoccerWorld.Models.CompetitionEvents;

namespace SoccerWorld.Migrations.SeedDatabase
{
    public class Nederland : SeedAbstract
    {
        public Nederland(SoccerWorldDatabaseContext context) : base(context)
        { }

        protected override Country CreateCountry()
        {
            Continent c = Context.Continents.First(o => o.Name == "Europa");
            Country country =
                Context.Countries.Add(
                        new Country() { Name = "Nederland", Continent = c }
                ).Entity;
            return country;
        }

        protected override List<Competition> CreateCompetition(Country country)
        {
            //All League Competitions
            List<Competition> divisions = new List<Competition>()
            {
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Eredivisie",
                        Country = country,
                        RelegateToChildCompetition = 1
                    }).Entity,
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Jupiler League",
                        Country = country,
                        PromoteToParentCompetition = 1
                    }).Entity
            };
            Context.SaveChanges();
            //Couple all Competitions for promotion-system
            for (int i = 0; i < divisions.Count; i++)
            {
                if (i < divisions.Count - 1)
                    divisions[i].ChildCompetitionId = divisions[i + 1].Id;
                if (i > 0)
                    divisions[i].ParentCompetitionId = divisions[i - 1].Id;
            }

            //Create Competition Events
            CreateDefaultCompetitionEvents(divisions[0], new DateTime(2019, 6, 1, 12, 0, 0),
                                            new DateTime(2019, 7, 24, 14, 30, 0), new DateTime(2020, 5, 1, 14, 30, 0)
                                            );
            CreateDefaultCompetitionEvents(divisions[1], new DateTime(2019, 6, 1, 12, 0, 0),
                                            new DateTime(2019, 7, 25, 14, 30, 0), new DateTime(2020, 4, 24, 14, 30, 0)
                                            );
            
            //Create Playoffs and Events
            CreateDefaultPlayoffStructure(divisions[0], divisions[1], new DateTime(2020, 5, 1, 18, 0, 0),
                                            new DateTime(2020, 5, 12, 13, 0, 0), new DateTime(2020, 5, 19, 13, 0, 0),
                                            2, 6,
                                            2);

            //Create Season end
            for (int i = 0; i <= 1; i++)
                Context.CompetitionEvents.Add(
                    new DefaultSeasonEndEvent()
                    {
                        Name = "Seizoenseinde",
                        Competition = divisions[i],
                        Date = new DateTime(2020, 5, 19, 18, 0, 0)
                    });

            Context.SaveChanges();
            return divisions;
        }

        protected override void CreateCup(Country country)
        {
        }

        protected override void CreateClubs(List<Competition> divisions)
        {
            CreateDefaultClub("Ajax", divisions[0], 12000);
            CreateDefaultClub("PSV", divisions[0], 10000);
            CreateDefaultClub("Feyenoord", divisions[0], 10000);
            CreateDefaultClub("AZ", divisions[0], 8000);
            CreateDefaultClub("Utrecht", divisions[0], 7000);
            CreateDefaultClub("Willem II", divisions[0], 7000);
            CreateDefaultClub("Heerenveen", divisions[0], 7000);
            CreateDefaultClub("Vitesse", divisions[0], 7000);
            CreateDefaultClub("Groningen", divisions[0], 7000);
            CreateDefaultClub("Sparta", divisions[0], 6000);
            CreateDefaultClub("Zwolle", divisions[0], 6000);
            CreateDefaultClub("Heracles", divisions[0], 6000);
            CreateDefaultClub("Den Haag", divisions[0], 5000);
            CreateDefaultClub("RKC Waalwijk", divisions[0], 5000);
            CreateDefaultClub("Twente", divisions[0], 5000);
            CreateDefaultClub("FC Emmen", divisions[0], 5000);
            CreateDefaultClub("Fortuna Sittard", divisions[0], 4000);
            CreateDefaultClub("VVV", divisions[0], 4000);

            CreateDefaultClub("Cambuur", divisions[1], 3500);
            CreateDefaultClub("De Graafschap", divisions[1], 3500);
            CreateDefaultClub("FC Volendam", divisions[1], 3500);
            CreateDefaultClub("Go Ahead Eagles", divisions[1], 3500);
            CreateDefaultClub("NAC", divisions[1], 3500);
            CreateDefaultClub("Telstar", divisions[1], 3500);
            CreateDefaultClub("Excelsior", divisions[1], 3500);
            CreateDefaultClub("NEC", divisions[1], 3500);
            CreateDefaultClub("Almere", divisions[1], 3000);
            CreateDefaultClub("Den Bosch", divisions[1], 3000);
            CreateDefaultClub("FC Eindhoven", divisions[1], 3000);
            CreateDefaultClub("MVV", divisions[1], 3000);
            CreateDefaultClub("Roda", divisions[1], 3000);
            CreateDefaultClub("FC Oss", divisions[1], 2500);
            CreateDefaultClub("Helmond Sport", divisions[1], 2500);
            CreateDefaultClub("Dordrecht", divisions[1], 2500);
            CreateDefaultClub("DHSC", divisions[1], 1000);
            CreateDefaultClub("GVV", divisions[1], 1000);
            CreateDefaultClub("Hollandia", divisions[1], 1000);
            CreateDefaultClub("Eemdijk", divisions[1], 1000);
        }
    }
}