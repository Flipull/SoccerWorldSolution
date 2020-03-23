using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoccerWorld.Models
{
    public class FirstReturnMatch : Match
    {
        public override void ProcessMatchEnd()
        {
            HasEnded = true;
            ProcessClubs();
            //removed competition-event caller, that's the second match's task
        }

    }
}