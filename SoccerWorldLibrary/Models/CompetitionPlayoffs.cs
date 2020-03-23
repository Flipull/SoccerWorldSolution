using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class CompetitionPlayoffs: Competition
    {
        protected override void DoPromotion()
        {
            //return list of the winners
            var winners =
                WorldState.GetDatabaseContext().CompetitionClubRelations.Where(
                            o => o.CompetitionId == Id && o.StillInCompetition)
                                        .ToList();

            foreach (var clubrelation in winners)
            {
                clubrelation.Club.CompetitionId = (int)ParentCompetitionId;
            }
        }
        protected override void DoRelegation()
        {
            //return list of all other teams
            var losers =
                WorldState.GetDatabaseContext().CompetitionClubRelations.Where(
                            o => o.CompetitionId == Id && o.StillInCompetition == false)
                                        .ToList();

            foreach (var clubrelation in losers)
            {
                clubrelation.Club.CompetitionId = (int)ChildCompetitionId;
            }
        }

    }
}