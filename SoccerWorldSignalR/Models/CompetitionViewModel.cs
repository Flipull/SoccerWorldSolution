using SoccerWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SoccerWorldSignalR.Models
{
    public class CompetitionViewModel
    {
        public int Id;
        public string Name;

        public CompetitionViewModel(Competition source)
        {
            Id = source.Id;
            Name = source.Name;
        }

    }
}
