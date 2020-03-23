using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }
        
        public int Season { get; set; }

        [ForeignKey("CompetitionEvent"), Required]
        public int CompetitionEventId { get; set; }
        public virtual CompetitionEvent CompetitionEvent { get; set; }

        public int? RoundNumber { get; set; }
        
        [ForeignKey("HomeClub"), Required]
        public int HomeClubId { get; set; }
        public virtual Club HomeClub { get; set; }

        [ForeignKey("AwayClub"), Required]
        public int AwayClubId { get; set; }
        public virtual Club AwayClub { get; set; }

        //[Index("DateIndex", IsUnique=false), Required]
        public DateTime Date { get; set; }
        
        [Range(0, 100)]
        public int? HomeScore { get; set; } = 0;

        [Range(0, 100)]
        public int? AwayScore { get; set; } = 0;

        [Required]
        public int MinutesPlayed { get; set; } = 0;

        [Required]
        public bool IsCupMatch { get; set; } = false;

        [Required]
        public bool HasEnded { get; set; } = false;



        public virtual bool HomeClubWinning()
        {
            return (HomeScore > AwayScore);
        }
        public virtual bool ClubsBothTied()
        {
            return (HomeScore == AwayScore);
        }
        public virtual bool AwayClubWinning()
        {
            return (HomeScore < AwayScore);
        }
        public bool IsHomeClub(Club other)
        {
            return (HomeClub == other);
        }
        public bool IsAwayClub(Club other)
        {
            return (AwayClub == other);
        }

        public virtual Club MatchWinner()
        {
            if (HomeScore > AwayScore)
                return HomeClub;
            else
                if (HomeScore < AwayScore)
                return AwayClub;
            else
                return null;
        }
        public virtual Club MatchLoser()
        {
            if (HomeScore > AwayScore)
                return AwayClub;
            else
                if (HomeScore < AwayScore)
                return HomeClub;
            else
                return null;
        }


        public void ExecuteGameTick(Random dice)
        {
            //ExecuteGameTick every minute of the match (1/90 match-length)
            //simplest "game-engine", 1 integer (Reputation) and 2 dice
            
            if (dice.NextDouble() < Math.Min(3, HomeClub.Reputation/(double)AwayClub.Reputation) / 90d )
            {
                HomeScore++;
            }
            if (dice.NextDouble() < Math.Min(3, AwayClub.Reputation/(double)HomeClub.Reputation) / 90d  )
            {
                AwayScore++;
            }

            if (MinutesPlayed == 90)
            {
                if (IsCupMatch && ClubsBothTied())
                    MinutesPlayed++;
                else
                    if (dice.NextDouble() < 0.3)
                        ProcessMatchEnd();
            }
            else if (IsCupMatch && MinutesPlayed == 120)
            {
                if (dice.NextDouble() < 0.6)
                {
                    if (ClubsBothTied())
                    {
                        ProcessPenaltyShootout(dice);
                        ProcessMatchEnd();
                    }
                    else
                        ProcessMatchEnd();
                }
            }
            else
            {
                MinutesPlayed++;
            }
        }
        private void ProcessPenaltyShootout(Random dice) 
        {
            double home_reputation_difference = HomeClub.Reputation / (double)AwayClub.Reputation;
            double away_reputation_difference = AwayClub.Reputation / (double)HomeClub.Reputation;
            double sum_differences = home_reputation_difference + away_reputation_difference;

            //random relative distribution decides the winner
            if (dice.NextDouble() * sum_differences < home_reputation_difference)
                HomeScore++;
            else
                AwayScore++;
        }

        public virtual void ProcessMatchEnd()
        {
            HasEnded = true;
            ProcessClubs();
            //call some CompetitionEvent.OnMatchComplete()
            CompetitionEvent.OnMatchComplete(this);
        }

        public void ProcessClubs()
        {
            //call the clubs to update themselfs
            int home_reputation = HomeClub.Reputation;
            int away_reputation = AwayClub.Reputation;
            HomeClub.OnMatchComplete(this, away_reputation, (int)HomeScore, (int)AwayScore);
            AwayClub.OnMatchComplete(this, home_reputation, (int)AwayScore, (int)HomeScore);
        }
    }
}