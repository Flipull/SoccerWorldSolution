using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoccerWorldSignalR.Models;
using SoccerWorld;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using SoccerWorld.Models;
using Microsoft.EntityFrameworkCore;

namespace SoccerWorldSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly SoccerWorldDatabaseContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SoccerWorldDatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {

            var worlddate = _context.WorldState.First().CurrentDateTime;

            var continents = _context.Continents;
            var selectedcontinent = continents.First();
            List<SelectListItem> continentlist = new List<SelectListItem>();
            foreach (var continent in continents)
            {
                continentlist.Add(new SelectListItem()
                {
                    Text = continent.Name, Value = continent.Id.ToString()
                });
            }

            var countries = _context.Countries.Where(o => o.Continent == selectedcontinent);
            var selectedcountry = countries.First();
            List<SelectListItem> countrylist = new List<SelectListItem>();
            foreach (var country in countries)
            {
                countrylist.Add(new SelectListItem()
                {
                    Text = country.Name,
                    Value = country.Id.ToString()
                });

            }
            var country_seasons = _context.CompetitionLeagueTable.Select(o => o.Season).Distinct();//TODO.OrderBy();
            List<SelectListItem> countryseasonslist = new List<SelectListItem>();
            foreach (var season in country_seasons)
            {
                countryseasonslist.Add(new SelectListItem()
                {
                    Text = season.ToString(),
                    Value = season.ToString(),
                    Selected = (season == selectedcountry.Season)
                });

            }

            var competitions = _context.Competitions.Where(o => o.Country == selectedcountry);
            var selectedcompetition = competitions.First();
            List<SelectListItem> competitionslist = new List<SelectListItem>();
            foreach (var competition in competitions)
            {
                competitionslist.Add(new SelectListItem()
                {
                    Text = competition.Name,
                    Value = competition.Id.ToString()
                });
            }


            IEnumerable<CompetitionLeagueTable> compstandings = null;
            if (selectedcountry.Season != null)
            {
                compstandings = CompetitionLeagueTable.GetStandings(selectedcompetition, (int)selectedcountry.Season);
            }
            else
            {
                compstandings = new List<CompetitionLeagueTable>();
            }


            var chosen_match = _context.Matches
                .Where(m => m.Season == selectedcountry.Season &&
                            m.CompetitionEvent.CompetitionId == selectedcompetition.Id &&
                            m.HasEnded == false)
                .OrderBy(m => m.RoundNumber).FirstOrDefault();
            if (chosen_match == null)
            {
                chosen_match = _context.Matches
                    .Where(m => m.Season == selectedcountry.Season &&
                                m.CompetitionEvent.CompetitionId == selectedcompetition.Id &&
                                m.HasEnded == true)
                    .OrderByDescending(m => m.RoundNumber).FirstOrDefault();
            }

            IEnumerable<Match> matches;
            if (chosen_match != null)
            {
                var round = chosen_match.RoundNumber;

                matches = _context.Matches
                        .Where(m => m.Season == selectedcountry.Season &&
                                    m.CompetitionEvent.CompetitionId == selectedcompetition.Id &&
                                    m.RoundNumber == round)
                        .Include(m => m.HomeClub)
                        .Include(m => m.AwayClub)
                        .ToList();
            }
            else
                matches = new List<Match>();

            var VM = new HomeViewModel()
            {
                WorldDate = worlddate,
                Season = selectedcountry.Season,
                Continents = continentlist,
                ContinentCountries = countrylist,
                CountrySeasons = countryseasonslist,
                CountryCompetitions = competitionslist,
                CompetitionStandings = compstandings.ToList(),
                CompetitionRoundMatches = matches.ToList()
            };
            
            
            return View(VM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
