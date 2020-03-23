using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DefaultSeasonStartEvent : CompetitionEvent
    {
        public override void Execute()
        {
            //Update Country (WARNING, FUNCTION CAN BE CALLED MULTIPLE TIMES (for each SeasonStartEvent once)
            Competition.Country.OnSeasonStart();
            
            //execute next event
            NextCompetitionEvent.Execute();

            base.Execute();
        }

    }
}