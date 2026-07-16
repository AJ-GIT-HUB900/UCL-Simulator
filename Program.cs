using System;
using System.Collections.Generic;
using System.Linq;

namespace ChampionsLeagueSimulator
{
    class Program
    {
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("   UEFA CHAMPIONS LEAGUE MONTE CARLO SIMULATOR");
            Console.WriteLine("==================================================\n");

            // Initialize the real 2025-2026 Quarter-Finalists
            var teams = SetupTeams();

            Console.WriteLine("🏆 QUARTER-FINALISTS:");
            foreach (var t in teams) Console.WriteLine($"- {t.Name}");
            Console.WriteLine();

            // Simulate Quarter-Finals
            var semiFinalists = PlayKnockoutRound(teams, "QUARTER-FINALS");
            
            // Simulate Semi-Finals
            var finalists = PlayKnockoutRound(semiFinalists, "SEMI-FINALS");
            
            // Simulate The Final (Single Match)
            var winner = PlayFinal(finalists[0], finalists[1]);

            Console.WriteLine("\n==================================================");
            Console.WriteLine($"👑 CHAMPIONS: {winner.Name.ToUpper()}");
            Console.WriteLine("==================================================");

            // Calculate Top Scorer
            var allPlayers = teams.SelectMany(t => t.Roster).OrderByDescending(p => p.TournamentGoals).ToList();
            var topScorer = allPlayers.First();
            
            Console.WriteLine($"\n👟 GOLDEN BOOT WINNER: {topScorer.Name} ({topScorer.TeamName}) - {topScorer.TournamentGoals} Goals");
            
            Console.WriteLine("\nTop 5 Goalscorers:");
            for(int i = 0; i < 5; i++) 
            {
                Console.WriteLine($"{i + 1}. {allPlayers[i].Name} - {allPlayers[i].TournamentGoals} goals");
            }
            
            Console.ReadLine();
        }

        static List<Team> PlayKnockoutRound(List<Team> teams, string roundName)
        {
            Console.WriteLine($"\n--- {roundName} ---");
            var winners = new List<Team>();
            
            // Shuffle teams for a random draw
            teams = teams.OrderBy(x => rnd.Next()).ToList();

            for (int i = 0; i < teams.Count; i += 2)
            {
                var team1 = teams[i];
                var team2 = teams[i+1];
                
                // Leg 1 (Team 1 Home) & Leg 2 (Team 2 Home)
                var leg1 = SimulateMatch(team1, team2);
                var leg2 = SimulateMatch(team2, team1);

                int agg1 = leg1.Item1 + leg2.Item2;
                int agg2 = leg1.Item2 + leg2.Item1;

                Console.WriteLine($"{team1.Name} vs {team2.Name} | Agg: {agg1} - {agg2} (L1: {leg1.Item1}-{leg1.Item2}, L2: {leg2.Item1}-{leg2.Item2})");

                if (agg1 > agg2) winners.Add(team1);
                else if (agg2 > agg1) winners.Add(team2);
                else 
                {
                    // Aggregate tied, simulate penalties
                    int pen1, pen2;
                    do {
                        pen1 = rnd.Next(3, 6);
                        pen2 = rnd.Next(3, 6);
                    } while (pen1 == pen2);
                    
                    if (pen1 > pen2) {
                        Console.WriteLine($"   -> {team1.Name} advances {pen1}-{pen2} on penalties.");
                        winners.Add(team1);
                    } else {
                        Console.WriteLine($"   -> {team2.Name} advances {pen2}-{pen1} on penalties.");
                        winners.Add(team2);
                    }
                }
            }
            return winners;
        }

        static Team PlayFinal(Team team1, Team team2)
        {
            Console.WriteLine($"\n--- FINAL ---");
            var result = SimulateMatch(team1, team2, neutralVenue: true);
            Console.WriteLine($"{team1.Name} {result.Item1} - {result.Item2} {team2.Name}");

            if (result.Item1 > result.Item2) return team1;
            if (result.Item2 > result.Item1) return team2;
            
            // Penalties
            int pen1, pen2;
            do {
                pen1 = rnd.Next(3, 6);
                pen2 = rnd.Next(3, 6);
            } while (pen1 == pen2);
            Console.WriteLine($"   -> Match goes to Penalties: {team1.Name} {pen1} - {pen2} {team2.Name}");
            
            return pen1 > pen2 ? team1 : team2;
        }

        static Tuple<int, int> SimulateMatch(Team home, Team away, bool neutralVenue = false)
        {
            // Calculate Lambda (Expected Goals)
            double homeAdvantage = neutralVenue ? 1.0 : 1.2;
            double homeLambda = home.AttackStrength * away.DefenseWeakness * homeAdvantage; 
            double awayLambda = away.AttackStrength * home.DefenseWeakness;

            int homeGoals = GetPoisson(homeLambda);
            int awayGoals = GetPoisson(awayLambda);

            AssignGoals(home, homeGoals);
            AssignGoals(away, awayGoals);

            return new Tuple<int, int>(homeGoals, awayGoals);
        }

        static void AssignGoals(Team team, int goals)
        {
            // Distributes the generated match goals to players based on their scoring weights
            for (int i = 0; i < goals; i++)
            {
                double totalWeight = team.Roster.Sum(p => p.ScoringWeight);
                double rand = rnd.NextDouble() * totalWeight;
                
                foreach (var player in team.Roster)
                {
                    rand -= player.ScoringWeight;
                    if (rand <= 0)
                    {
                        player.TournamentGoals++;
                        break;
                    }
                }
            }
        }

        static int GetPoisson(double lambda)
        {
            // Knuth's algorithm for generating Poisson random numbers
            double L = Math.Exp(-lambda);
            int k = 0;
            double p = 1.0;
            do
            {
                k++;
                p *= rnd.NextDouble();
            } while (p > L);
            return k - 1;
        }

        static List<Team> SetupTeams()
        {
            var teams = new List<Team>();

            // Setup real 25/26 Quarter-Finalists with Base Stats
            var psg = new Team("Paris Saint-Germain", 2.1, 1.1);
            psg.Roster.Add(new Player("Khvicha Kvaratskhelia", "Paris Saint-Germain", 0.5) { TournamentGoals = 8 });
            psg.Roster.Add(new Player("Ousmane Dembélé", "Paris Saint-Germain", 0.3) { TournamentGoals = 6 });
            teams.Add(psg);

            var arsenal = new Team("Arsenal", 1.9, 0.8);
            arsenal.Roster.Add(new Player("Viktor Gyökeres", "Arsenal", 0.4) { TournamentGoals = 4 });
            arsenal.Roster.Add(new Player("Bukayo Saka", "Arsenal", 0.3) { TournamentGoals = 3 });
            teams.Add(arsenal);

            var bayern = new Team("FC Bayern Munich", 2.1, 1.0);
            bayern.Roster.Add(new Player("Harry Kane", "FC Bayern Munich", 0.6) { TournamentGoals = 10 });
            bayern.Roster.Add(new Player("Michael Olise", "FC Bayern Munich", 0.2) { TournamentGoals = 4 });
            teams.Add(bayern);

            var realMadrid = new Team("Real Madrid", 2.2, 1.1);
            realMadrid.Roster.Add(new Player("Kylian Mbappé", "Real Madrid", 0.6) { TournamentGoals = 11 });
            realMadrid.Roster.Add(new Player("Vinícius Júnior", "Real Madrid", 0.3) { TournamentGoals = 4 });
            teams.Add(realMadrid);

            var atletico = new Team("Atlético Madrid", 1.6, 0.7);
            atletico.Roster.Add(new Player("Julián Álvarez", "Atlético Madrid", 0.5) { TournamentGoals = 7 });
            atletico.Roster.Add(new Player("Antoine Griezmann", "Atlético Madrid", 0.2) { TournamentGoals = 2 });
            teams.Add(atletico);

            var liverpool = new Team("Liverpool", 1.9, 1.0);
            liverpool.Roster.Add(new Player("Luis Díaz", "Liverpool", 0.4) { TournamentGoals = 6 });
            liverpool.Roster.Add(new Player("Dominik Szoboszlai", "Liverpool", 0.3) { TournamentGoals = 4 });
            teams.Add(liverpool);

            var barcelona = new Team("FC Barcelona", 2.1, 1.2);
            barcelona.Roster.Add(new Player("Lamine Yamal", "FC Barcelona", 0.4) { TournamentGoals = 5 });
            barcelona.Roster.Add(new Player("Robert Lewandowski", "FC Barcelona", 0.4) { TournamentGoals = 4 });
            teams.Add(barcelona);
            
            var sporting = new Team("Sporting CP", 1.5, 1.3);
            sporting.Roster.Add(new Player("Luis Suárez", "Sporting CP", 0.5) { TournamentGoals = 5 });
            sporting.Roster.Add(new Player("Trincão", "Sporting CP", 0.3) { TournamentGoals = 3 });
            teams.Add(sporting);

            return teams;
        }
    }

    class Team
    {
        public string Name { get; set; }
        public double AttackStrength { get; set; }
        public double DefenseWeakness { get; set; }
        public List<Player> Roster { get; set; }

        public Team(string name, double attack, double defenseWeakness)
        {
            Name = name;
            AttackStrength = attack;
            DefenseWeakness = defenseWeakness;
            Roster = new List<Player>();
        }
    }

    class Player
    {
        public string Name { get; set; }
        public string TeamName { get; set; }
        public double ScoringWeight { get; set; }
        public int TournamentGoals { get; set; }

        public Player(string name, string teamName, double weight)
        {
            Name = name;
            TeamName = teamName;
            ScoringWeight = weight;
        }
    }
}