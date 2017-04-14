//
// RaceGeneratorTests.cs
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

using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

namespace ChaoticRotation.Tests
{
    [TestFixture()]
    public class RaceGeneratorTests
    {
        [Test, TestCaseSource("TestCases")]
        public void TestRaceGeneration(int numRacers, int numLanes)
        {
            RaceGenerator rg = new RaceGenerator();
            int[,] races = rg.Solve(numRacers, numLanes);

            // for each racer, make a list of the races they are assigned to, by lane
            Dictionary<int, int[]> lanesByRacer = new Dictionary<int, int[]>();
            for (int i = 1; i <= numRacers; i++)
            {
                lanesByRacer.Add(i, new int[numLanes]);
            }

            int numRaces = rg.GetNumRaces(numRacers, numLanes);

            for (int raceNum = 1; raceNum <= numRacers; raceNum++)
            {
                for (int laneNum = 1; laneNum <= numLanes; laneNum++)
                {
                    int racer = races[raceNum - 1, laneNum - 1];
                    lanesByRacer[racer][laneNum - 1] = raceNum;
                }
            }

            // validate all the assignments
            foreach (int racer in lanesByRacer.Keys)
            {
                // make sure the racer has a chance to run in each lane
                Assert.That(lanesByRacer[racer], Has.None.EqualTo(0));

                // make sure the racer isn't scheduled for more than one lane in a single race
                Assert.That(lanesByRacer[racer], Is.Unique);

                // make sure all race assignments are valid
                Assert.That(lanesByRacer[racer], Has.None.GreaterThan(numRaces));
            }
        }

        public static IEnumerable TestCases
        {
            get
            {
                int minRacers = 2;
                int maxRacers = 50;
                int minLanes = 2;
                int maxLanes = 6;

                for (int numRacers = minRacers; numRacers < maxRacers; numRacers++)
                {
                    for (int numLanes = minLanes; numLanes < maxLanes; numLanes++)
                    {
                        if (numRacers >= numLanes)
                        {
                            yield return new TestCaseData(numRacers, numLanes)
                                .SetName(string.Format("{0} Racers, {1} Lanes", numRacers.ToString("D2"), numLanes))
                                .SetCategory(string.Format("{0} Lanes", numLanes));
                        }
                    }
                }
            }
        }
    }
}