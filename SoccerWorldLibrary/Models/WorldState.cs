using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SoccerWorld.Models
{
    public interface IRealtimeCallback
    {
        void OnWorldStateChanged(WorldState state);
        void OnCompetitionEventProcessed(CompetitionEvent evt);
        void OnMatchesChanged(IEnumerable<Match> matches);
        void OnMatchesEnd(IEnumerable<Match> matches);
    }

    public class WorldState
    {

        [Key]
        public int Id { get; set; }
        public int AsyncProcessesCount { get; set; }
        public DateTime CurrentDateTime { get; set; }

        //Use a private singleton-like DBContext
        static private SoccerWorldDatabaseContext _context;
        static public SoccerWorldDatabaseContext GetDatabaseContext()
        {
            if (_context == null)
                _context = SoccerWorldDatabaseContext.GetService();
            return _context;
            //return SoccerWorldDatabaseContext.GetService();
        }

        static private WorldState _world;
        static public WorldState GetWorldState()
        {
            if (_world == null)
            {
                var ctx = GetDatabaseContext();
                _world = ctx.WorldState.First();
            }
            return _world;
        }
        static public bool IsProcessingMatches()
        {
            return GetWorldState().AsyncProcessesCount > 0;
        }
        static public void ProcessWorld(IRealtimeCallback _callback)
        {
            //if another Process is taking care of it, return
            if (IsProcessingMatches())
                return;

            var _context = GetDatabaseContext();
            //skipping time == jumping to the first upcoming event(s) and processes those
            //
            DateTime next_date = GetEarliestEvent(_context);


            GetWorldState().AsyncProcessesCount++;

            _callback.OnWorldStateChanged(GetWorldState());
            //set world date
            GetWorldState().CurrentDateTime = next_date;
            _context.SaveChanges();

            ProcessGameTick(_context, _callback, true);

            GetWorldState().AsyncProcessesCount--;
            _context.SaveChanges();
            _callback.OnWorldStateChanged(GetWorldState());
        }
        static private void ProcessWorld(SoccerWorldDatabaseContext _context, IRealtimeCallback _callback, DateTime target_date)
        {
            //if another Process is taking care of it, return
            if (IsProcessingMatches())
                return;

            //as long we did not reach target date
            while (GetWorldState().CurrentDateTime < target_date)
            {
                DateTime eventdate = GetEarliestEvent(_context);
                //update world date with the first next date (target or event)
                GetWorldState().CurrentDateTime = (eventdate < target_date ? eventdate : target_date);


                GetWorldState().AsyncProcessesCount++;
                _context.SaveChanges();
                _callback.OnWorldStateChanged(GetWorldState());
                ProcessGameTick(_context, _callback, true);
                GetWorldState().AsyncProcessesCount--;
                _context.SaveChanges();
            }
        }

        static private DateTime GetEarliestEvent(SoccerWorldDatabaseContext _context)
        {
            var world_state = GetWorldState();
            var next_event =
                _context.CompetitionEvents
                        .OrderBy(o => o.Date)
                        .FirstOrDefault(o => //o.Date != null && 
                                        o.Date > world_state.CurrentDateTime);
            var next_match =
                _context.Matches
                        .OrderBy(o => o.Date)
                        .FirstOrDefault(o => //o.Date != null && 
                                        o.Date > world_state.CurrentDateTime);
            
            if (next_match == null && next_event == null)
                throw new ArgumentNullException();

            if (next_match == null)
                return (DateTime)next_event.Date;
            if (next_event == null)
                return next_match.Date;

            if (next_event.Date < next_match.Date)
                return (DateTime)next_event.Date;
            else 
                return next_match.Date;

        }


        static private List<Match> _MATCHES = new List<Match>();
        static private Random dice = new Random();
        
        static private void ProcessGameTick(SoccerWorldDatabaseContext _context, IRealtimeCallback _callback, bool first_run = false)
        {
            //check if new CompetitionEvents are found, and activate them
            ExecuteCompetitionEvents(_context, _callback, GetWorldState().CurrentDateTime);
            
            //check if new matches found on current time, to add to matches-pool
            UpdateMatchPool(_context, _callback, GetWorldState().CurrentDateTime);
            //if matches-pool is empty
            if (_MATCHES.Count == 0)
                //  end Process
                return;
            
            //else process all matches
            ProcessMatchPool();
            GetWorldState().CurrentDateTime = GetWorldState().CurrentDateTime.AddMinutes(1);
            GetDatabaseContext().SaveChanges();
            if (_MATCHES.Count > 0)
                _callback.OnMatchesChanged(_MATCHES);

            _callback.OnWorldStateChanged(GetWorldState());
            
            Thread.Sleep(2000);
            ProcessGameTick(_context, _callback);
        }

        static private bool ExecuteCompetitionEvents(SoccerWorldDatabaseContext _context, IRealtimeCallback _callback, DateTime date)
        {
            //get all events for current date
            ICollection<CompetitionEvent> events =
                _context.CompetitionEvents.Where(o => o.Date == date)
                                        .Include(o => o.NextCompetitionEvent)
                                        .Include(o => o.Competition)
                                        .ThenInclude(o => o.Clubs)
                                        .ThenInclude(o => o.Country)
                                        .OrderByDescending(o => o.Id)
                                        .ToList();
            if (events.Count == 0)
                return false;
            
            //process each event sequential
            foreach (CompetitionEvent comp_event in events)
            {
                comp_event.Execute();
            }
            _context.SaveChanges();
            foreach (CompetitionEvent comp_event in events)
                _callback.OnCompetitionEventProcessed(comp_event);
            return true;
        }

        static private void UpdateMatchPool(SoccerWorldDatabaseContext _context, IRealtimeCallback _callback, DateTime date)
        {
            //select ended matches and send callback
            _callback.OnMatchesEnd(_MATCHES.Where(o => o.HasEnded));

            //check if matches ended, to remove from matches-pool
            _MATCHES = _MATCHES.Where(o => o.HasEnded == false).ToList();

            //check if matches started, to add them to matches-pool
            List<Match> new_matches =
                _context.Matches.Where(o => o.Date == date)
                                .Include(o => o.HomeClub)
                                .Include(o => o.AwayClub)
                                .Include(o => o.CompetitionEvent)
                                .ThenInclude(e => e.Competition)
                                .ThenInclude(c => c.Country)
                                .ToList();
            _MATCHES.AddRange(new_matches);
            
        }

        static private void ProcessMatchPool()
        {
            foreach (Match match in _MATCHES)
            {
                //run game
                match.ExecuteGameTick(dice);
            }
        }


        

    }
}