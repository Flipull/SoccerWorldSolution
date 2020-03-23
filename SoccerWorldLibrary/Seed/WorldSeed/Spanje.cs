using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoccerWorld.Models;

namespace SoccerWorld.Migrations.WorldSeed
{
    public class Spanje : SeedAbstract
    {
        public Spanje(SoccerWorldDatabaseContext context) : base(context)
        { }

        protected override void CreateClubs(List<Competition> divisions)
        {
            throw new NotImplementedException();
        }

        protected override List<Competition> CreateCompetition(Country country)
        {
            throw new NotImplementedException();
        }

        protected override Country CreateCountry()
        {
            throw new NotImplementedException();
        }

        protected override void CreateCup(Country country)
        {
            throw new NotImplementedException();
        }
    }
}