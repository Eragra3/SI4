using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SI4.Models;
using SI4.Parser;
using static SI4.MainExecutor;

namespace SI4.Logic
{
    public static class CoherenceAnalysisExecutor
    {
        public static List<KeyPointsPair> FindCoherentPairs(List<KeyPointsPair> pairs, int neighboursCount = 200, double neighboursCountThreshold = 0.5)
        {
            List<KeyPointsPair> coherentPairs = new List<KeyPointsPair>();

            object lockObject = new object();

            Parallel.For(0, pairs.Count, i =>
            {
                KeyPointsPair pair = pairs[i];

                KeyPoint keyPoint1 = pair.KeyPoint1;

                KeyPointsPair[] neighbours1 = new KeyPointsPair[neighboursCount];
                List<double> neighbours1Distances = Enumerable.Repeat(0.0, neighboursCount).ToList();
                int neighbours1Count = 0;
                for (int j = 0; j < pairs.Count; j++)
                {
                    KeyPointsPair p = pairs[j];
                    double distance = GetDistance(keyPoint1, p.KeyPoint1);

                    if (neighbours1Count < neighboursCount)
                    {
                        neighbours1[neighbours1Count] = p;
                        neighbours1Distances[neighbours1Count] = distance;
                        neighbours1Count++;
                    }
                    else if (distance < neighbours1Distances.Max())
                    {
                        int farthestIndex = neighbours1Distances.IndexOf(neighbours1Distances.Max());
                        neighbours1[farthestIndex] = p;
                        neighbours1Distances[farthestIndex] = distance;
                    }
                }

                KeyPoint keyPoint2 = pair.KeyPoint2;

                KeyPointsPair[] neighbours2 = new KeyPointsPair[neighboursCount];
                List<double> neighbours2Distances = Enumerable.Repeat(0.0, neighboursCount).ToList();
                int neighbours2Count = 0;
                for (int j = 0; j < pairs.Count; j++)
                {
                    KeyPointsPair p = pairs[j];
                    double distance = GetDistance(keyPoint2, p.KeyPoint2);

                    if (neighbours2Count < neighboursCount)
                    {
                        neighbours2[neighbours2Count] = p;
                        neighbours2Distances[neighbours2Count] = distance;
                        neighbours2Count++;
                    }
                    else if (distance < neighbours2Distances.Max())
                    {
                        int farthestIndex = neighbours2Distances.IndexOf(neighbours2Distances.Max());
                        neighbours2[farthestIndex] = p;
                        neighbours2Distances[farthestIndex] = distance;
                    }
                }

                int coherentPairsCount = 0;
                foreach (KeyPointsPair neighbour in neighbours1)
                {
                    bool match = false;
                    for (int j = 0; j < neighbours2.Length; j++)
                    {
                        if (neighbour != neighbours2[j]) continue;

                        match = true;
                        break;
                    }

                    if (match)
                    {
                        coherentPairsCount++;
                    }
                }

                if (coherentPairsCount >= neighboursCountThreshold * neighboursCount)
                {
                    lock (lockObject)
                    {
                        coherentPairs.Add(pair);
                    }
                }
            });

            return coherentPairs;
        }
    }
}
