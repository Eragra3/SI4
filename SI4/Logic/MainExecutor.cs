using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SI4.Models;
using SI4.Parser;

namespace SI4
{
    public static class MainExecutor
    {
        public static List<KeyPointsPair> FindKeyPointsPairs(KeyPoint[] image1KeyPoints, KeyPoint[] image2KeyPoints, string pairsFilePath = null)
        {
            if (pairsFilePath != null)
            {
                //look for cached
            }

            List<KeyPointsPair> keyPointsPairs = new List<KeyPointsPair>(image1KeyPoints.Length);

            int[] image1ClosestNeighboors = new int[image1KeyPoints.Length];
            int[] image2ClosestNeighboors = new int[image2KeyPoints.Length];

            Parallel.For(0, image1KeyPoints.Length, (i) =>
            {
                KeyPoint keyPoint = image1KeyPoints[i];

                int closestNeighboorIndex = -1;
                double smallestDistance = double.PositiveInfinity;

                for (int j = 0; j < image2KeyPoints.Length; j++)
                {
                    KeyPoint keyPoint2 = image2KeyPoints[j];

                    double distance = GetDistance(keyPoint, keyPoint2);

                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestNeighboorIndex = j;
                    }
                }
                image1ClosestNeighboors[i] = closestNeighboorIndex;
            });

            Parallel.For(0, image2KeyPoints.Length, (i) =>
            {
                KeyPoint keyPoint = image2KeyPoints[i];

                int closestNeighboorIndex = -1;
                double smallestDistance = double.PositiveInfinity;

                for (int j = 0; j < image1KeyPoints.Length; j++)
                {
                    KeyPoint keyPoint2 = image1KeyPoints[j];

                    double distance = GetDistance(keyPoint, keyPoint2);

                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        closestNeighboorIndex = j;
                    }
                }
                image2ClosestNeighboors[i] = closestNeighboorIndex;
            });

            for (int i = 0; i < image1ClosestNeighboors.Length; i++)
            {
                if (i != image2ClosestNeighboors[image1ClosestNeighboors[i]]) continue;

                KeyPoint kp1 = image1KeyPoints[image2ClosestNeighboors[image1ClosestNeighboors[i]]];
                KeyPoint kp2 = image2KeyPoints[image1ClosestNeighboors[i]];

                keyPointsPairs.Add(new KeyPointsPair(kp1, kp2));
            }

            return keyPointsPairs;
        }


        //optimize that
        public static double GetDistance(KeyPoint keyPoint1, KeyPoint keyPoint2)
        {
            int sum = 0;
            for (int i = 0; i < keyPoint1.Features.Length; i++)
            {
                sum += (keyPoint1.Features[i] - keyPoint2.Features[i]) * (keyPoint1.Features[i] - keyPoint2.Features[i]);
            }

            return sum;
        }
    }
}
