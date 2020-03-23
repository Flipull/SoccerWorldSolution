using SoccerWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoccerWorldSignalR.Models
{
    public class HomeViewModel
    {
        public DateTime WorldDate;
        public int? Season;
        //public int CurrentCompetitionRound;
        public IEnumerable<SelectListItem> Continents;
        public IEnumerable<SelectListItem> ContinentCountries;
        public IEnumerable<SelectListItem> CountryCompetitions;
        public IEnumerable<SelectListItem> CountrySeasons;
        public List<CompetitionLeagueTable> CompetitionStandings;
        public List<Match> CompetitionRoundMatches;
    }
}
