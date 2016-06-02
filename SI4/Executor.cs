using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SI4.Models;
using SI4.Parser;

namespace SI4
{
    public static class Executor
    {

        public static void Run(string file1Path, string file1SiftPath, string file2Path, string file2SiftPath)
        {
            var image1Features = SiftParser.Parse(file1SiftPath);
            var image2Features = SiftParser.Parse(file2SiftPath);
        }

        public static List<KeyPointsPair> FindKeyPointsPairs(KeyPoint[] image1KeyPoints, KeyPoint[] image2KeyPoints)
        {
            var keyPointsPairs = new List<KeyPointsPair>(image1KeyPoints.Length);

            for (var i = 0; i < image1KeyPoints.Length; i++)
            {
                KeyPoint keyPoint1 = image1KeyPoints[i];

                KeyPoint keyPoint2 = image2KeyPoints[0];
                double smallestDistance = double.PositiveInfinity;

                for (var j = 0; j < image2KeyPoints.Length; j++)
                {

                    var distance = GetDistance(keyPoint1, keyPoint2);

                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        keyPoint2 = image2KeyPoints[j];
                    }
                }

                keyPointsPairs.Add(new KeyPointsPair(keyPoint1.X, keyPoint1.Y, keyPoint2.X, keyPoint2.Y));
            }

            return keyPointsPairs;
        }

        public static double GetDistance(KeyPoint keyPoint1, KeyPoint keyPoint2)
        {
            var sum = 0;
            for (var i = 0; i < keyPoint1.Features.Length; i++)
            {
                sum += (keyPoint1.Features[i] - keyPoint2.Features[i]) * (keyPoint1.Features[i] - keyPoint2.Features[i]);
            }

            return Math.Sqrt(sum);
        }
    }
}
