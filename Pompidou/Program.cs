using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pompidou
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        [Test]
        public async void RunnerOdds()
        {
            var part1 = await GetOddsContentAsync("http://www.sportsinteraction.com/sportsbook/contentLoader.cfm?fixRewrite=true&section=events&eventTypeID=648&sectionRequested=events&ajaxRequest=true");
            var part2 = await GetOddsContentAsync("http://www.sportsinteraction.com/sportsbook/contentLoader.cfm?eventTypeID=648&batchLastRowID=119&sectionRequested=events&ajaxRequest=true&isBatchCall=true");

            var runnerOdds = GetRunnerOdds(part1).Operable() + GetRunnerOdds(part2);

            //Print(runnerOdds);

            foreach (var odds in runnerOdds)
            {
                Console.WriteLine("{0}\t{1}", odds.Name, odds.Odds.OrderBy(t => t.Round).Select(t => t.Price).Join("\t"));
            }
        }

        private static void Print(IEnumerable<RunnerOdds> runnerPrices)
        {
            Console.WriteLine("Count: {0}", runnerPrices.Count());

            foreach (var rp in runnerPrices)
            {
                Console.WriteLine(rp.Name);
                foreach (var p in rp.Odds)
                {
                    Console.WriteLine("\t {0}: {1}", p.Round, p.Price);
                }
            }
        }

        private async Task<string> GetOddsContentAsync(string requestUri)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(requestUri);
                var jsonContent = JsonConvert.DeserializeAnonymousType(json, new { content = null as string });
                return jsonContent.content;
            }
        }

        private static IEnumerable<RunnerOdds> GetRunnerOdds(string input)
        {
            var nameRegex = new Regex(@"data-runnerPrice=""(?<RunnerPrice>.+)""", RegexOptions.Compiled);
            var priceRegex = new Regex(@"<span itemprop=""name"">2014 World Cup - (?<RunnerName>.+)</span>", RegexOptions.Compiled);

            var runners = priceRegex.Matches(input).Cast<Match>().ToArray();
            var prices = nameRegex.Matches(input).Cast<Match>().ToArray();

            var runnerPrices = runners.Select((r, i) => new RunnerOdds {
                Name = r.Groups["RunnerName"].Value,
                Odds = prices.Where(p => r.Index < p.Index && (i == runners.Length - 1 || p.Index < runners[i + 1].Index))
                    .Select((p, j) => new EliminationRoundOdds {
                        Round = (EliminationRound) j,
                        Price = Convert.ToSingle(p.Groups["RunnerPrice"].Value)
                    })
            });
            return runnerPrices;
        }
    }

    class RunnerOdds
    {
        public string Name { get; set; }
        public IEnumerable<EliminationRoundOdds> Odds { get; set; }
    }

    struct EliminationRoundOdds
    {
        public EliminationRound Round { get; set; }
        public double Price { get; set; }
    }

    enum EliminationRound
    {
        GroupStage = 0,
        SecondRound = 1,
        QuarterFinals = 2,
        SemiFinals = 3,
        RunnerUp = 4,
        Winner = 5
    }

}