using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SoccerWorld;
using SoccerWorld.Models;
using SoccerWorldSignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoccerWorldSignalR
{
    public class SignalRHub : Microsoft.AspNetCore.SignalR.Hub, IRealtimeCallback//, IHub
    {
        public static IServiceProvider _internal_serviceprovider { get; set; }
        /*
        Microsoft.AspNet.SignalR.Hubs.HubCallerContext IHub.Context { get; set; }
        IHubCallerConnectionContext<dynamic> IHub.Clients { get; set; }
        Microsoft.AspNet.SignalR.IGroupManager IHub.Groups { get; set; }
        /*
        public new IHubCallerConnectionContext<dynamic> Clients { get; set; }
        public new Microsoft.AspNet.SignalR.IGroupManager Groups { get; set; }
        public new Microsoft.AspNet.SignalR.Hubs.HubCallerContext Context { get; set; }
        */


        public SignalRHub() : base()
        { }
        public SignalRHub GetService()
        {
            return (SignalRHub)_internal_serviceprovider.GetService(typeof(SignalRHub));
        }
        public Task OnConnected()
        {
            return Task.CompletedTask;
        }

        public Task OnReconnected()
        {
            return Task.CompletedTask;
        }

        public Task OnDisconnected(bool stopCalled)
        {
            return Task.CompletedTask;
        }

        ///////////////////////////////////
        public async Task ContinueWorld()
        {
            var a = new Action(() => WorldState.ProcessWorld(this));
            Task.Run(a);
        }

        //////////////////////////////
        public async void CompetitionsDataRequest(int? id)
        {
            if (id == null)
            {
                await Clients.Caller.SendAsync("OnCompetitionsDataRequest", "{}");
                return;
            }

            using (var context = SoccerWorldDatabaseContext.GetService())
            {
                var comps = context.Competitions
                    .Where(c => c.CountryId == id)
                    .ToList();
                await Clients.Caller.SendAsync("OnCompetitionsDataRequest",
                    JsonConvert.SerializeObject(SerializeCompetitions(comps)));
            }
        }
        public async void CompetitionDataRequest(int id, int? season, int? round)
        {
            using (var context = SoccerWorldDatabaseContext.GetService())
            {
                var comp = context.Competitions
                        .Where(c => c.Id == id)
                        .Include(c => c.Country).First();

                if (comp.Country.Season == null)
                {
                    await Clients.Caller.SendAsync("OnCompetitionDataRequest",
                        JsonConvert.SerializeObject(
                            new
                            {
                                Competition = new CompetitionViewModel(comp),
                                CompetitionSeasons = new Array[0],
                                LeagueTable = new Array[0],
                                Matches = new Array[0]
                            }
                        ));
                    return;//nullable-int syndrome solution #1
                }
                if (season == null)
                    season = comp.Country.Season;

                var country_seasons = 
                    context.CompetitionLeagueTable.Where(o => o.Competition == comp)
                                                .Select(o => o.Season)
                                                .Distinct().ToList();//TODO.OrderBy();
                
                //
                var standings = CompetitionLeagueTable.GetStandings(comp, (int)season);
                
                if (round == null)
                {
                    var chosen_match = context.Matches
                        .Where(m => m.Season == season &&
                                    m.CompetitionEvent.CompetitionId == comp.Id &&
                                    m.HasEnded == false)
                        .OrderBy(m => m.RoundNumber).FirstOrDefault();
                    if (chosen_match == null)
                    {
                        chosen_match = context.Matches
                            .Where(m => m.Season == season &&
                                        m.CompetitionEvent.CompetitionId == comp.Id &&
                                        m.HasEnded == true)
                            .OrderByDescending(m => m.RoundNumber).FirstOrDefault();
                    }
                    round = chosen_match?.RoundNumber;
                }
                
                var matches = context.Matches
                        .Where(m => m.Season == season &&
                                    m.CompetitionEvent.CompetitionId == comp.Id &&
                                    m.RoundNumber == round)
                        .Include(m => m.HomeClub)
                        .Include(m => m.AwayClub)
                        .ToList();
                
                await Clients.Caller.SendAsync("OnCompetitionDataRequest",
                    JsonConvert.SerializeObject(
                        new
                        {
                            Competition = new CompetitionViewModel(comp),
                            CompetitionSeasons = country_seasons,
                            LeagueTable = SerializeStandings(standings),
                            Matches = SerializeMatches(matches)
                        }
                    ));
            }

        }
/////////////////////////////////
        public async void OnWorldStateChanged(WorldState state)
        {
            await Clients.All.SendAsync("OnWorldStateChanged", JsonConvert.SerializeObject(state));
        }
        
        public async void OnCompetitionEventProcessing(CompetitionEvent evt)
        {
            //
        }

        public async void OnCompetitionEventProcessed(CompetitionEvent evt)
        {
            await Clients.All.SendAsync("OnCompetitionEventProcessed",
                JsonConvert.SerializeObject(
                    new 
                    {
                        Date = evt.Date,
                        Country = evt.Competition.Country.Name,
                        CompetitionId = evt.Competition.Id,
                        Competition = evt.Competition.Name,
                        Event = evt.Name
                    }
                ));
        }


        private List<CompetitionViewModel> SerializeCompetitions(IEnumerable<Competition> comps)
        {
            var vm_list = new List<CompetitionViewModel>();
            foreach (var comp in comps)
                vm_list.Add(new CompetitionViewModel(comp));
            return vm_list;
        }
        private List<CompetitionLeagueTableViewModel> SerializeStandings(IEnumerable<CompetitionLeagueTable> tables)
        {
            var vm_list = new List<CompetitionLeagueTableViewModel>();
            foreach (var item in tables)
                vm_list.Add(new CompetitionLeagueTableViewModel(item));
            return vm_list;
        }
        private List<MatchViewModel> SerializeMatches(IEnumerable<Match> matches)
        {
            var vm_list = new List<MatchViewModel>();
            foreach (var match in matches)
                vm_list.Add(new MatchViewModel(match));
            return vm_list;
        }
        public async void OnMatchesChanged(IEnumerable<Match> matches)
        {

            await Clients.All.SendAsync("OnMatchesChanged", 
                        JsonConvert.SerializeObject(SerializeMatches(matches))
                    );
        }
        
        public async void OnMatchesEnd(IEnumerable<Match> matches)
        {
            var competitions = matches.Select(o => o.CompetitionEvent.Competition).Distinct().ToList();
            await Clients.All.SendAsync("OnMatchesChanged",
                        JsonConvert.SerializeObject(SerializeMatches(matches))
                    );
            foreach (var comp in competitions)
                OnCompetitionStandingsChanged(comp);
        }

        private async void OnCompetitionStandingsChanged(Competition competition)
        {
            var standings = CompetitionLeagueTable.GetStandings(competition, (int)competition.Country.Season);
            await Clients.All.SendAsync("OnCompetitionStandingsChanged",
                JsonConvert.SerializeObject(
                    new
                    {
                        CompetitionId = competition.Id,
                        LeagueTable = SerializeStandings(standings)
                    }
                ));
        }

    }
}
