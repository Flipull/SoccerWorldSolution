using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DefaultSeasonEndEvent : CompetitionEvent
    {
        public override void Execute()
        {
            /* update all competition-events for next season
             * 
             */
            Competition.OnSeasonEnd();
            
            //get all events for this competition
            var events = WorldState.GetDatabaseContext().CompetitionEvents.Where(o => o.CompetitionId == CompetitionId)
                                                                          .ToList();
            //foreach event 
            foreach (CompetitionEvent comp_event in events) 
            {
                //  tell the events to update for next season
                comp_event.OnSeasonEnd();
            }
            WorldState.GetDatabaseContext().SaveChanges();

            if (NextCompetitionEvent != null)
                NextCompetitionEvent.Execute();

            base.Execute();
        }
    }
}