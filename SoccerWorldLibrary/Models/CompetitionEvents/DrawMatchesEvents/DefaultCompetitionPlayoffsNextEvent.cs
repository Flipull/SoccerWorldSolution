using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DefaultCompetitionPlayoffsNextEvent : DefaultCompetitionPlayoffsStartEvent
    {
        public override ICollection<Club> ChooseClubs()
        {
            //select all clubs in the Relations-table where StillInCompetition
            return
                WorldState.GetDatabaseContext().CompetitionClubRelations
                                                  .Where(o => o.CompetitionId == CompetitionId &&
                                                                o.StillInCompetition == true)
                                                  .Select(o => o.Club)
                                                  .ToList();
        }
        
    }
}