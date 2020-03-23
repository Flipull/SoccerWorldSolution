using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class SecondReturnMatch : Match
    {
        [Required]
        public int FirstMatchId { get; set; }
        [NotMapped]
        public Match FirstMatch 
        { 
            get 
            {
                return WorldState.GetDatabaseContext().Matches.Where(o => o.Id == (int)FirstMatchId).First();
            }
            set { FirstMatchId = value.Id; } 
        }

        public override Club MatchWinner()
        {
            if (GetHomeClubReturnScore() > GetAwayClubReturnScore())
                return HomeClub;
            else
                if (GetHomeClubReturnScore() < GetAwayClubReturnScore())
                return AwayClub;
            else
                return null;
        }
        public override Club MatchLoser()
        {
            if (GetHomeClubReturnScore() > GetAwayClubReturnScore())
                return AwayClub;
            else
                if (GetHomeClubReturnScore() < GetAwayClubReturnScore())
                return HomeClub;
            else
                return null;
        }

        public override bool HomeClubWinning()
        {
            return (GetHomeClubReturnScore() > GetAwayClubReturnScore() );
        }
        public override bool ClubsBothTied()
        {
            return (GetHomeClubReturnScore() == GetAwayClubReturnScore() );
        }
        public override bool AwayClubWinning()
        {
            return (GetHomeClubReturnScore() < GetAwayClubReturnScore() );
        }

        public int GetHomeClubReturnScore()
        {
            int homescore = (int)HomeScore + (int)FirstMatch.AwayScore*2;
            return homescore;
        }
        public int GetAwayClubReturnScore()
        {
            int awayscore = (int)AwayScore*2 + (int)FirstMatch.HomeScore;
            return awayscore;
        }

    }
}