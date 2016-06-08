using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using SI4.Models;

namespace SI4.Logic
{
    public class RansacExecutor
    {
        public static List<KeyPointsPair> FindCoherentPairs(List<KeyPointsPair> pairs, int iterationsCount = 100, bool usePerspectiveTransform = false)
        {
            List<KeyPointsPair> coherentPairs = new List<KeyPointsPair>();

            object lockObject = new object();

            Parallel.For(0, pairs.Count, i =>
            {
                Matrix bestModel = null;
                int bestScore = 0;

                for (int j = 0; j < iterationsCount; j++)
                {
                    Matrix model = null;

                }


            });

            return coherentPairs;
        }
    }
}