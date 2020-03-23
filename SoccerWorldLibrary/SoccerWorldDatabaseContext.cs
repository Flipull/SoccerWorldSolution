using Microsoft.EntityFrameworkCore;
using SoccerWorld.Models;
using SoccerWorld.Models.CompetitionEvents;
using System;

namespace SoccerWorld
{
    public class SoccerWorldDatabaseContext : DbContext
    {
        public static IServiceProvider _internal_serviceprovider;
        public SoccerWorldDatabaseContext(DbContextOptions<SoccerWorldDatabaseContext> options) : base(options)
        {
        }
        
        public static SoccerWorldDatabaseContext GetService()
        {
            return (SoccerWorldDatabaseContext) _internal_serviceprovider.GetService(typeof(SoccerWorldDatabaseContext));
        }

        public DbSet<WorldState> WorldState { get; set; }
        public DbSet<Continent> Continents { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionLeagueTable> CompetitionLeagueTable { get; set; }
        public DbSet<CompetitionEvent> CompetitionEvents { get; set; }
        public DbSet<CompetitionEventHistory> CompetitionEventHistory { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<CompetitionClubRelation> CompetitionClubRelations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefaultSeasonStartEvent>();
            modelBuilder.Entity<DefaultSeasonEndEvent>();
            modelBuilder.Entity<DrawDefaultCompetitionMatchesEvent>();
            modelBuilder.Entity<DefaultCompetitionPlayoffsStartEvent>();
            modelBuilder.Entity<DefaultCompetitionPlayoffsNextEvent>();
            modelBuilder.Entity<DefaultCompetitionPlayoffsFinalEvent>();
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Competitions)
                .WithOne(c => c.Country).HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Country>()
                .HasMany<Club>()
                .WithOne(c => c.Country).HasForeignKey(c => c.CountryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Competition>()
                .HasMany(c => c.Clubs)
                .WithOne(c => c.Competition).HasForeignKey(c => c.CompetitionId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CompetitionLeagueTable>()
                .HasOne(c => c.Competition).WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<CompetitionLeagueTable>()
                .HasOne(c => c.CompetitionEvent).WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeClub).WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayClub).WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            

            base.OnModelCreating(modelBuilder);
        }
    }
}