using SoccerWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoccerWorldSignalR.Models
{
    public class CompetitionLeagueTableViewModel
    {
        public string ClubName;
        public int Won;
        public int Tied;
        public int Lost;
        public int GoalsFor;
        public int GoalsAgainst;

        public int Points;

        public CompetitionLeagueTableViewModel(CompetitionLeagueTable source)
        {
            ClubName = source.Club.Name;
            Won = source.Won;
            Tied = source.Tied;
            Lost = source.Lost;
            GoalsFor = source.GoalsFor;
            GoalsAgainst = source.GoalsAgainst;
            Points = source.Points;
        }

    }
}
