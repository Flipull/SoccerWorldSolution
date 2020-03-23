using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoccerWorld.Models;
using SoccerWorld.Models.CompetitionEvents;

namespace SoccerWorld.Migrations.SeedDatabase
{
    public class Engeland: SeedAbstract
    {
        public Engeland(SoccerWorldDatabaseContext context) : base(context)
        { }

        protected override Country CreateCountry()
        {
            Continent c = Context.Continents.First(o => o.Name == "Europa");
            Country country =
                Context.Countries.Add(
                        new Country() { Name = "Engeland", Continent = c }
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
                        Name = "Premier League",
                        Country = country,
                        RelegateToChildCompetition = 3
                    }).Entity,
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Sky Bet Championship",
                        Country = country,
                        PromoteToParentCompetition = 2,
                        RelegateToChildCompetition = 3
                    }).Entity,
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Sky Bet League One",
                        Country = country,
                        PromoteToParentCompetition = 2,
                        RelegateToChildCompetition = 4
                    }).Entity,
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Sky Bet League Two",
                        Country = country,
                        PromoteToParentCompetition = 3,
                        RelegateToChildCompetition = 2
                    }).Entity,
                Context.Competitions.Add(
                    new Competition()
                    {
                        Name = "Vanorama National League",
                        Country = country,
                        PromoteToParentCompetition = 1
                    }).Entity
            };
            Context.SaveChanges();
            //Couple all Competitions for promotion-system
            for (int i = 0; i < divisions.Count; i++)
            {
                if (i < divisions.Count-1)
                    divisions[i].ChildCompetitionId = divisions[i + 1].Id;
                if (i > 0)
                    divisions[i].ParentCompetitionId = divisions[i - 1].Id;
            }
            
            //Create Competition Events
            CreateDefaultCompetitionEvents(divisions[0], new DateTime(2019, 6, 6, 12, 0, 0),
                                            new DateTime(2019, 8, 12, 15, 0, 0), new DateTime(2020, 5, 12, 15, 0, 0)
                                            );
            for (int i = 1; i < divisions.Count; i++)
                CreateDefaultCompetitionEvents(divisions[i], new DateTime(2019, 6, 6, 12, 0, 0),
                                                new DateTime(2019, 8, 4, 15, 0, 0), new DateTime(2020, 5, 4, 15, 0, 0)
                                                );

            //Create Playoffs and Events
            CreateDefaultPlayoffStructure(divisions[0], divisions[1], new DateTime(2020, 5, 12, 21, 0, 0),
                                            new DateTime(2020, 5, 17, 15, 0, 0), new DateTime(2020, 5, 24, 15, 0, 0),
                                            0, 4);
            CreateDefaultPlayoffStructure(divisions[1], divisions[2], new DateTime(2020, 5, 12, 21, 0, 0),
                                            new DateTime(2020, 5, 17, 15, 0, 0), new DateTime(2020, 5, 24, 15, 0, 0),
                                            0, 4);
            for (int i = 2; i < divisions.Count - 1; i++)
                CreateDefaultPlayoffStructure(divisions[i], divisions[i + 1], new DateTime(2020, 5, 5, 21, 0, 0),
                                            new DateTime(2020, 5, 9, 15, 0, 0), new DateTime(2020, 5, 12, 15, 0, 0),
                                                0, 4);

            //Create Season end
            for (int i = 0; i <= 2; i++)
                Context.CompetitionEvents.Add(
                    new DefaultSeasonEndEvent()
                    {
                        Name = "Seizoenseinde",
                        Competition = divisions[i],
                        Date = new DateTime(2020, 5, 25, 15, 0, 0)
                    });

            for (int i = 3; i < divisions.Count; i++)
                Context.CompetitionEvents.Add(
                    new DefaultSeasonEndEvent()
                    {
                        Name = "Seizoenseinde",
                        Competition = divisions[i],
                        Date = new DateTime(2020, 5, 13, 15, 0, 0)
                    });

            Context.SaveChanges();
            return divisions;
        }
        protected override void CreateCup(Country country)
        {
            
        }


        
        protected override void CreateClubs(List<Competition> divisions)
        {
            CreateDefaultClub("Liverpool", divisions[0], 15000);
            CreateDefaultClub("Manchester City", divisions[0], 15000);
            CreateDefaultClub("Leicester City", divisions[0], 13000);
            CreateDefaultClub("Chelsea", divisions[0], 15000);
            CreateDefaultClub("Tottenham", divisions[0], 14000);
            CreateDefaultClub("Sheffield United", divisions[0], 12000);
            CreateDefaultClub("Manchester United", divisions[0], 14000);
            CreateDefaultClub("Wolverhampton", divisions[0], 12000);
            CreateDefaultClub("Everton", divisions[0], 12000);
            CreateDefaultClub("Arsenal", divisions[0], 14000);
            CreateDefaultClub("Burnley FC", divisions[0], 10000);
            CreateDefaultClub("Newcastle", divisions[0], 11000);
            CreateDefaultClub("Southampton", divisions[0], 10000);
            CreateDefaultClub("Crystal Palace", divisions[0], 10000);
            CreateDefaultClub("Brighton", divisions[0], 10000);
            CreateDefaultClub("Bournemouth", divisions[0], 10000);
            CreateDefaultClub("Aston Villa", divisions[0], 11000);
            CreateDefaultClub("West Ham", divisions[0], 9000);
            CreateDefaultClub("Watford", divisions[0], 9000);
            CreateDefaultClub("Norwich City", divisions[0], 9000);

            CreateDefaultClub("West Bromwich", divisions[1], 8000);
            CreateDefaultClub("Leeds United", divisions[1], 8000);
            CreateDefaultClub("Fulham", divisions[1], 8000);
            CreateDefaultClub("Nottingham Forest", divisions[1], 8000);
            CreateDefaultClub("Brentford", divisions[1], 8000);
            CreateDefaultClub("Bristol City", divisions[1], 7500);
            CreateDefaultClub("Preston", divisions[1], 7500);
            CreateDefaultClub("Swansea City", divisions[1], 7000);
            CreateDefaultClub("Millwall", divisions[1], 7000);
            CreateDefaultClub("Blackburn", divisions[1], 7500);
            CreateDefaultClub("Sheffield Wednesday", divisions[1], 7500);
            CreateDefaultClub("Cardiff City", divisions[1], 7000);
            CreateDefaultClub("Derby County", divisions[1], 7000);
            CreateDefaultClub("Hull City", divisions[1], 7000);
            CreateDefaultClub("Reading", divisions[1], 7000);
            CreateDefaultClub("Queens Park Rangers", divisions[1], 7000);
            CreateDefaultClub("Birmingham", divisions[1], 7000);
            CreateDefaultClub("Middlesbrough", divisions[1], 7000);
            CreateDefaultClub("Charlton Athletic", divisions[1], 6000);
            CreateDefaultClub("Huddersfield", divisions[1], 6000);
            CreateDefaultClub("Stoke City", divisions[1], 5500);
            CreateDefaultClub("Wigan Athletic", divisions[1], 5500);
            CreateDefaultClub("Barnsley", divisions[1], 5500);
            CreateDefaultClub("Luton Town", divisions[1], 5000);



            CreateDefaultClub("Rotherham", divisions[2], 5000);
            CreateDefaultClub("Wycombe", divisions[2], 5000);
            CreateDefaultClub("Peterborough", divisions[2], 5000);
            CreateDefaultClub("Ipswich Town", divisions[2], 4500);
            CreateDefaultClub("Coventry", divisions[2], 4500);
            CreateDefaultClub("Portsmouth", divisions[2], 4500);
            CreateDefaultClub("Sunderland", divisions[2], 4500);
            CreateDefaultClub("Oxford United", divisions[2], 4000);
            CreateDefaultClub("Doncaster", divisions[2], 4000);
            CreateDefaultClub("Fleetwood", divisions[2], 4000);
            CreateDefaultClub("Burton", divisions[2], 4000);
            CreateDefaultClub("Gillingham", divisions[2], 4000);
            CreateDefaultClub("Bristol Rovers", divisions[2], 4000);
            CreateDefaultClub("Lincoln City", divisions[2], 4000);
            CreateDefaultClub("Blackpool", divisions[2], 3500);
            CreateDefaultClub("Shrewsbury", divisions[2], 3500);
            CreateDefaultClub("Stanley", divisions[2], 3500);
            CreateDefaultClub("Rochdale", divisions[2], 3500);
            CreateDefaultClub("MK Dons", divisions[2], 3500);
            CreateDefaultClub("AFC Wimbeldon", divisions[2], 3000);
            CreateDefaultClub("Tranmere Rovers", divisions[2], 3000);
            CreateDefaultClub("Southend United", divisions[2], 3000);
            CreateDefaultClub("Bolton", divisions[2], 3000);
            CreateDefaultClub("#24 SBL1", divisions[2], 3000);

            CreateDefaultClub("Swindon Town", divisions[3], 3000);
            CreateDefaultClub("Crewe Alexandra", divisions[3], 3000);
            CreateDefaultClub("Plymouth Argyle", divisions[3], 3000);
            CreateDefaultClub("Exeter City", divisions[3], 2900);
            CreateDefaultClub("Northampton", divisions[3], 2900);
            CreateDefaultClub("Colchester", divisions[3], 2800);
            CreateDefaultClub("Cheltenham", divisions[3], 2800);
            CreateDefaultClub("Bradford City", divisions[3], 2700);
            CreateDefaultClub("Forest Green", divisions[3], 2600);
            CreateDefaultClub("Port Vale", divisions[3], 2600);
            CreateDefaultClub("Salford City", divisions[3], 2500);
            CreateDefaultClub("Newport County", divisions[3], 2500);
            CreateDefaultClub("Crawley Town", divisions[3], 2500);
            CreateDefaultClub("Oldham", divisions[3], 2400);
            CreateDefaultClub("Walsall", divisions[3], 2400);
            CreateDefaultClub("Cambridge United", divisions[3], 2400);
            CreateDefaultClub("Grimsby Town", divisions[3], 2300);
            CreateDefaultClub("Leyton Orient", divisions[3], 2300);
            CreateDefaultClub("Scunthorpe", divisions[3], 2250);
            CreateDefaultClub("Carlisle", divisions[3], 2250);
            CreateDefaultClub("Mansfield", divisions[3], 2100);
            CreateDefaultClub("Macclesfield Town", divisions[3], 2100);
            CreateDefaultClub("Morecambe", divisions[3], 2100);
            CreateDefaultClub("Stevenage", divisions[3], 2000);
            
            CreateDefaultClub("Barrow", divisions[4], 2000);
            CreateDefaultClub("Harrogate Town", divisions[4], 2000);
            CreateDefaultClub("Halifax Town", divisions[4], 2000);
            CreateDefaultClub("Yeovil Town", divisions[4], 2000);
            CreateDefaultClub("Boreham Wood", divisions[4], 1950);
            CreateDefaultClub("Notts County", divisions[4], 1900);
            CreateDefaultClub("Bromley", divisions[4], 1900);
            CreateDefaultClub("Dover Athletic", divisions[4], 1900);
            CreateDefaultClub("Solihull Moors", divisions[4], 1900);
            CreateDefaultClub("Woking", divisions[4], 1800);
            CreateDefaultClub("Barnet", divisions[4], 1800);
            CreateDefaultClub("Stockport", divisions[4], 1750);
            CreateDefaultClub("Hartlepool", divisions[4], 1700);
            CreateDefaultClub("Sutton United", divisions[4], 1700);
            CreateDefaultClub("Torquay United", divisions[4], 1650);
            CreateDefaultClub("Eastleigh FC", divisions[4], 1650);
            CreateDefaultClub("Maidenhead United", divisions[4], 1600);
            CreateDefaultClub("Aldershot Town", divisions[4], 1600);
            CreateDefaultClub("Wrexham", divisions[4], 1500);
            CreateDefaultClub("Dagenham & Redbridge", divisions[4], 1500);
            CreateDefaultClub("Chesterfield", divisions[4], 1450);
            CreateDefaultClub("Flyde", divisions[4], 1400);
            CreateDefaultClub("Ebbsfleet United", divisions[4], 1450);
            CreateDefaultClub("Chorley", divisions[4], 1300);
        }



    }
}