using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SoccerWorld.Models.CompetitionEvents
{
    public abstract class DrawMatchesEvent : CompetitionEvent
    {
        [Required]
        public DateTime FirstRound { get; set; }
        [Required]
        public DateTime LastRound { get; set; }

        [Required]
        public int PlayDateDeviation { get; set; } = 0;

        [Required]
        public bool PlayTwicePerOpponent { get; set; } = true;

        public abstract ICollection<Club> ChooseClubs();
        public abstract void GenerateMatches(ICollection<Club> participants);

        
        public override void Execute()
        {
            GenerateMatches(ChooseClubs());

            base.Execute();
        }

        public override void OnSeasonEnd()
        {
            //Season is over; plan this event for next season
            FirstRound = new DateTime(FirstRound.Year + 1, FirstRound.Month, FirstRound.Day,
                                        FirstRound.Hour, FirstRound.Minute, FirstRound.Second);
            LastRound = new DateTime(LastRound.Year + 1, LastRound.Month, LastRound.Day,
                                        LastRound.Hour, LastRound.Minute, LastRound.Second);
            base.OnSeasonEnd();
        }


        static protected List<Club> SortRandom(ICollection<Club> list)
        {
            return SortRandom(list.ToList());
        }
        static protected List<Club> SortRandom(List<Club> list)
        {
            list.OrderBy(o => o.Id ^ o.Reputation % list.Count);
            return list;
        }
        static protected int Modulo(int value, int range)
        {
            int r = value % range;
            return (r < 0 ? r + range : r);
        }
        static protected DateTime[] LerpDates(int amount, DateTime start, DateTime end)
        {
            DateTime[] result = new DateTime[amount];
            TimeSpan timespan = end - start;
            if (amount == 1) {
                result[0] = start.AddMinutes(0);
                return result;
            }

            for (int i = 0; i < amount; i++)
            {
                result[i] = start.AddMinutes((int)timespan.TotalMinutes * i / (amount - 1));
            }
            return result;
        }

    }
}