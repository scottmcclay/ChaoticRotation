//
// Program.cs
//
// Authors:
//  Scott McClay <scott.mcclay.csm@gmail.com>
//
// Copyright (C) 2017 Scott McClay
//
// You may use, distribute and modify this code under the terms of the
// GNU Affero General Public License Verson 3 (GNU AGPL V3).
// http://www.gnu.org/licenses/agpl.html
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using NDesk.Options;

namespace ChaoticRotation
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showHelp = false;
            int numRacers = 0;
            int numLanes = 0;
            bool verbose = false;

            var p = new OptionSet()
            {
                { "r=|racers=", "The number of racers.", (int r) => numRacers = r },
                { "l=|lanes=", "The number of lanes.", (int l) => numLanes = l },
                { "v|verbose", "If present, outputs details of every step as lane assignments are made.",
                    v => verbose = v != null },
                { "h|help", "Show this message and exit", h => showHelp = h != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("ChaoticRotation: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try 'ChaoticRotation --help' for more information.");
                return;
            }

            if (numRacers <= 0)
            {
                Console.WriteLine("Number of racers must be a positive integer.");
                showHelp = true;
            }

            if (numLanes <= 0)
            {
                Console.WriteLine("Number of lanes must be a positive integer.");
                showHelp = true;
            }

            if (showHelp)
            {
                Console.WriteLine();
                ShowHelp(p);
                return;
            }

            RaceGenerator rg = new RaceGenerator();
            rg.ShowOutput = verbose;

            try
            {
                int[,] races = rg.Solve(numRacers, numLanes);
                int numRaces = rg.GetNumRaces(numRacers, numLanes);

                Console.WriteLine();
                Console.WriteLine("Minimum number of steps possible: {0}", rg.MinSteps);
                Console.WriteLine("Number of steps actually taken:   {0}", rg.ActualSteps);
                Console.WriteLine("Number of backtracks:             {0}", rg.Backtracks);
                Console.WriteLine();

                PrintRaces(races, numRaces, numLanes);
                Console.WriteLine();
                PrintRacers(races, numRaces, numLanes, numRacers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Caught: {0}", ex.Message);
            }
        }
        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: ChaoticRotation -r|-racers numRacers -l|-lanes numLanes");
            Console.WriteLine();
            Console.WriteLine("Generates a set of races using Chaotic Rotation to assign racers to lanes");
            Console.WriteLine("such that all racers race the same number of times and each racer races");
            Console.WriteLine("once on every lane.");
            Console.WriteLine();
            Console.WriteLine("Constraint: numLanes <= numRacers");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static void PrintRaces(int[,] races, int numRaces, int numLanes)
        {
            Console.Write("         ");
            for (int laneNum = 0; laneNum < numLanes; laneNum++)
            {
                Console.Write("{0,2}   Lane {1,2}  ", "|", laneNum + 1);
            }
            Console.WriteLine();

            for (int race = 0; race < numRaces; race++)
            {
                Console.Write(string.Format("Race {0,2}:  ", race + 1));
                for (int lane = 0; lane < numLanes; lane++)
                {
                    Console.Write("    Racer {0,2}  ", races[race, lane]);
                }
                Console.WriteLine();
            }
        }

        private static void PrintRacers(int[,] races, int numRaces, int numLanes, int numRacers)
        {
            Console.Write("         ");
            for (int laneNum = 0; laneNum < numLanes; laneNum++)
            {
                Console.Write("{0,2}   Lane {1,2}  ", "|", laneNum + 1);
            }
            Console.WriteLine();

            // for each racer, make a list of the races they are assigned to, by lane
            Dictionary<int, int[]> lanesByRacer = new Dictionary<int, int[]>();
            for (int i = 1; i <= numRacers; i++)
            {
                lanesByRacer.Add(i, new int[numLanes]);
            }

            for (int raceNum = 1; raceNum <= numRacers; raceNum++)
            {
                for (int laneNum = 1; laneNum <= numLanes; laneNum++)
                {
                    int racer = races[raceNum - 1, laneNum - 1];
                    lanesByRacer[racer][laneNum - 1] = raceNum;
                }
            }

            foreach (int racer in lanesByRacer.Keys)
            {
                Console.Write(string.Format("Racer {0,2}: ", racer));
                for (int lane = 0; lane < numLanes; lane++)
                {
                    Console.Write("    Race {0,2}   ", lanesByRacer[racer][lane]);
                }
                Console.WriteLine();
            }
        }
    }
}
