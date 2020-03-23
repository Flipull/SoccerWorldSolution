using SoccerWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoccerWorldSignalR.Models
{
    public class MatchViewModel
    {
        public int Id;
        public string HomeClub;
        public string AwayClub;
        public DateTime Date;
        public int? HomeScore;
        public int? AwayScore;
        public int? MinutesPlayed;
        public bool isFinished;

        public MatchViewModel(Match source)
        {
            Id = source.Id;
            HomeClub = source.HomeClub.Name;
            AwayClub = source.AwayClub.Name;
            Date = source.Date;
            HomeScore = source.HomeScore;
            AwayScore = source.AwayScore;
            MinutesPlayed = source.MinutesPlayed;
            isFinished = source.HasEnded;
        }

    }
}
