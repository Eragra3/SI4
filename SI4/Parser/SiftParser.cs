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
            using (var sr = new StreamReader(path))
            {
                var featuresCountText = sr.ReadLine();
                var featuresCount = int.Parse(featuresCountText);
                var keyPointsCountText = sr.ReadLine();
                var keyPointsCount = int.Parse(keyPointsCountText);

                keyPoints = new KeyPoint[keyPointsCount];

                string readedLine;
                var counter = 0;
                while ((readedLine = sr.ReadLine()) != null)
                {
                    var splittedLine = readedLine.Split(' ');

                    var x = double.Parse(splittedLine[0], CultureInfo.InvariantCulture);
                    var y = double.Parse(splittedLine[1], CultureInfo.InvariantCulture);

                    var paramA = double.Parse(splittedLine[2], CultureInfo.InvariantCulture);
                    var paramB = double.Parse(splittedLine[3], CultureInfo.InvariantCulture);
                    var paramC = double.Parse(splittedLine[4], CultureInfo.InvariantCulture);

                    var features = new int[featuresCount];

                    for (var i = 0; i < features.Length; i++)
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
