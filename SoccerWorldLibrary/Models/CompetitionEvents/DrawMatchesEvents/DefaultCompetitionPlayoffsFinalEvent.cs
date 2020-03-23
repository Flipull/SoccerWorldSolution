using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models.CompetitionEvents
{
    public class DefaultCompetitionPlayoffsFinalEvent : DefaultCompetitionPlayoffsNextEvent
    {
        public override void OnSeasonEnd()
        {
            //  delete RT vars in CompetitionPlayoffsClubs
            var relation_state = WorldState.GetDatabaseContext().CompetitionClubRelations
                                                            .Where(o => o.CompetitionId == CompetitionId);

            foreach (var old_state in relation_state)
                WorldState.GetDatabaseContext().CompetitionClubRelations.Remove(old_state);

            base.OnSeasonEnd();
        }

    }
}