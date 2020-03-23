using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class Club
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Country"), Required]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        [ForeignKey("Competition"), Required]
        public int CompetitionId { get; set; }

        private Competition _competition;
        public virtual Competition Competition
        {
            get
            {
                return _competition;
            }
            set
            {
                if (value == null)
                {
                    //when EF6 removes the item from DB it 'unsets' it's properties?
                    Country = null;
                    _competition = null;
                }
                else
                {
                    Country = value.Country;
                    _competition = value;
                }
            }
        }

        

        [MinLength(1), MaxLength(32), Required]
        public string Name { get; set; }

        [Required]
        public int Reputation { get; set; }

        


        public void OnMatchComplete(Match match, int opponent_reputation, int my_score, int their_score)
        {
            //update my reputation, compared to score and opponent reputation
            //todo implement multipliers to competitions 
            //if I won
            if (my_score > their_score)
                Reputation += RatioEloPoints(opponent_reputation, Reputation);

            //if tied
            if (my_score == their_score)
                if (Reputation < opponent_reputation)
                    Reputation += (int)Math.Sqrt(RatioEloPoints(opponent_reputation, Reputation));
                else if (Reputation > opponent_reputation)
                    Reputation -= (int)Math.Sqrt(RatioEloPoints(Reputation, opponent_reputation));
            
            //if I lost
            if (my_score < their_score)
                Reputation -= RatioEloPoints(Reputation, opponent_reputation);
            
        }

        int MINIMUM_AMOUNT_OF_CLUBREPUTATION = 0;
        private int RatioEloPoints(int loser_rep, int winner_rep)
        {
            double points = Math.Pow(2, Math.Pow(Math.Min(3, loser_rep / (double)winner_rep), 2) );
            points = Math.Min(Math.Floor(points), Math.Min(loser_rep, winner_rep) - MINIMUM_AMOUNT_OF_CLUBREPUTATION);
            return (int)points;
        }

    }
}