using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI4.Parser
{
    public class SiftParser
    {

        public static KeyPoint[] Parse(string path)
        {

            KeyPoint[] keyPoints;
            using (StreamReader sr = new StreamReader(path))
            {
                string featuresCountText = sr.ReadLine();
                int featuresCount = int.Parse(featuresCountText);
                string keyPointsCountText = sr.ReadLine();
                int keyPointsCount = int.Parse(keyPointsCountText);

                keyPoints = new KeyPoint[keyPointsCount];

                string readedLine;
                int counter = 0;
                while ((readedLine = sr.ReadLine()) != null)
                {
                    string[] splittedLine = readedLine.Split(' ');

                    double x = double.Parse(splittedLine[0], CultureInfo.InvariantCulture);
                    double y = double.Parse(splittedLine[1], CultureInfo.InvariantCulture);

                    double paramA = double.Parse(splittedLine[2], CultureInfo.InvariantCulture);
                    double paramB = double.Parse(splittedLine[3], CultureInfo.InvariantCulture);
                    double paramC = double.Parse(splittedLine[4], CultureInfo.InvariantCulture);

                    int[] features = new int[featuresCount];

                    for (int i = 0; i < features.Length; i++)
                    {
                        features[i] = int.Parse(splittedLine[i +5]);
                    }

                    keyPoints[counter++] = new KeyPoint
                    {
                        X = x,
                        Y = y,
                        A = paramA,
                        B = paramB,
                        C = paramC,
                        Features = features
                    };
                }
            }

            return keyPoints;
        }


    }
}
