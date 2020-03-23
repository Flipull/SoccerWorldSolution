using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoccerWorld.Models
{
    public abstract class CompetitionEvent
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Competition"), Required]
        public int CompetitionId { get; set; }
        public virtual Competition Competition { get; set; }

        [ForeignKey("NextCompetitionEvent")]
        public int? NextCompetitionEventId { get; set; } = null;
        public virtual CompetitionEvent NextCompetitionEvent { get; set; }

        public DateTime? Date { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        public virtual void Execute()
        {
            OnExecuteEvent();
        }

        public virtual void OnMatchComplete(Match match) 
        {
            return;
        }
        
        public virtual void OnSeasonEnd()
        {
            //Season is over; plan this event for next season
            if (Date.HasValue)
            {
                DateTime d = (DateTime)Date;
                Date = new DateTime( d.Year + 1, d.Month, d.Day, 
                                    d.Hour, d.Minute, d.Second);
            }
            WorldState.GetDatabaseContext().SaveChanges();
        }
        
        private void OnExecuteEvent()
        {
            WorldState.GetDatabaseContext().CompetitionEventHistory.Add(
                new CompetitionEventHistory()
                {
                    CompetitionEvent = this,
                    Type = "Started",
                    //Date = WorldState.GetWorldState().CurrentDateTime
                });
            WorldState.GetDatabaseContext().SaveChanges();
        }

        protected void OnFinishedEvent()
        {
            WorldState.GetDatabaseContext().CompetitionEventHistory.Add(
                new CompetitionEventHistory()
                {
                    CompetitionEvent = this,
                    Type = "Finished",
                    //Date = WorldState.GetWorldState().CurrentDateTime
                });
            WorldState.GetDatabaseContext().SaveChanges();
        }

    }
}