using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SI4.Models;
using SI4.Parser;

namespace SI4.Logic
{
    public class RansacExecutor
    {
        [ThreadStatic]
        private static Random _random = new Random();
        private static Random Rng
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }

                return _random;
            }
        }


        public static List<KeyPointsPair> FindCoherentPairs(List<KeyPointsPair> pairs, int maxError, int iterationsCount = 50, bool usePerspectiveTransform = false)
        {
            List<KeyPointsPair> coherentPairs = new List<KeyPointsPair>();

            object lockObject = new object();
            int samplesCount = usePerspectiveTransform ? 4 : 3;

            Matrix<double> bestModel = null;
            int bestScore = 0;

            Parallel.For(0, pairs.Count, i =>
            {

                List<KeyPointsPair> sampleKeyPoints = new List<KeyPointsPair>(3);

                for (int j = 0; j < iterationsCount; j++)
                {
                    Matrix<double> model = null;
                    while (model == null)
                    {
                        sampleKeyPoints.Clear();

                        KeyPointsPair randomPair;

                        for (int k = 0; k < samplesCount; k++)
                        {
                            do
                            {
                                randomPair = pairs[Rng.Next(0, pairs.Count)];
                            } while (sampleKeyPoints.Contains(randomPair));
                            sampleKeyPoints.Add(randomPair);
                        }

                        if (usePerspectiveTransform)
                        {
                            model = CalculatePerspectiveMatrix(sampleKeyPoints[0], sampleKeyPoints[1], sampleKeyPoints[2], sampleKeyPoints[3]);
                        }
                        else
                        {
                            model = CalculateAffineMatrix(sampleKeyPoints[0], sampleKeyPoints[1], sampleKeyPoints[2]);
                        }

                    }

                    int score = 0;
                    foreach (KeyPointsPair pair in pairs)
                    {
                        var error = GetError(model, pair);

                        if (error < maxError)
                        {
                            score++;
                        }
                    }

                    lock (lockObject)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestModel = model;
                        }
                    }
                }
            });

            foreach (KeyPointsPair pair in pairs)
            {
                var error = GetError(bestModel, pair);

                if (error < maxError)
                {
                    coherentPairs.Add(pair);
                }
            }

            return coherentPairs;
        }

        public static double GetError(Matrix<double> transformationMatrix, KeyPointsPair pair)
        {
            Matrix<double> point1 = Matrix.Build.Dense(3, 1);

            point1[0, 0] = pair.KeyPoint1.X;
            point1[1, 0] = pair.KeyPoint1.Y;
            point1[2, 0] = 1;

            Matrix<double> projectedPoint = transformationMatrix.Multiply(point1);

            return pair.KeyPoint2.GetDistance(projectedPoint[0, 0] / projectedPoint[2, 0],
                projectedPoint[1, 0] / projectedPoint[2, 0]);
        }

        public static Matrix<double> CalculateAffineMatrix(KeyPointsPair pair1, KeyPointsPair pair2, KeyPointsPair pair3)
        {
            Matrix<double> lMatrix = Matrix.Build.Dense(6, 6, 0);

            lMatrix[0, 0] = pair1.KeyPoint1.X;
            lMatrix[1, 0] = pair2.KeyPoint1.X;
            lMatrix[2, 0] = pair3.KeyPoint1.X;

            lMatrix[0, 1] = pair1.KeyPoint1.Y;
            lMatrix[1, 1] = pair2.KeyPoint1.Y;
            lMatrix[2, 1] = pair3.KeyPoint1.Y;

            lMatrix[0, 2] = 1;
            lMatrix[1, 2] = 1;
            lMatrix[2, 2] = 1;

            lMatrix[3, 3] = pair1.KeyPoint1.X;
            lMatrix[4, 3] = pair2.KeyPoint1.X;
            lMatrix[5, 3] = pair3.KeyPoint1.X;

            lMatrix[3, 4] = pair1.KeyPoint1.Y;
            lMatrix[4, 4] = pair2.KeyPoint1.Y;
            lMatrix[5, 4] = pair3.KeyPoint1.Y;

            lMatrix[3, 5] = 1;
            lMatrix[4, 5] = 1;
            lMatrix[5, 5] = 1;

            Matrix<double> lMatrixInverse = lMatrix.Inverse();

            if (lMatrixInverse == null)
            {
                return null;
            }

            Matrix<double> rMatrix = Matrix.Build.Dense(6, 1);

            rMatrix[0, 0] = pair1.KeyPoint2.X;
            rMatrix[1, 0] = pair2.KeyPoint2.X;
            rMatrix[2, 0] = pair3.KeyPoint2.X;
            rMatrix[3, 0] = pair1.KeyPoint2.Y;
            rMatrix[4, 0] = pair2.KeyPoint2.Y;
            rMatrix[5, 0] = pair3.KeyPoint2.Y;

            Matrix<double> parametersMatrix = lMatrixInverse.Multiply(rMatrix);

            Matrix<double> transformationMatrix = Matrix.Build.Dense(3, 3);

            transformationMatrix[0, 0] = parametersMatrix[0, 0];
            transformationMatrix[0, 1] = parametersMatrix[1, 0];
            transformationMatrix[0, 2] = parametersMatrix[2, 0];

            transformationMatrix[1, 0] = parametersMatrix[3, 0];
            transformationMatrix[1, 1] = parametersMatrix[4, 0];
            transformationMatrix[1, 2] = parametersMatrix[5, 0];

            transformationMatrix[2, 0] = 0;
            transformationMatrix[2, 1] = 0;
            transformationMatrix[2, 2] = 1;

            return transformationMatrix;
        }


        public static Matrix<double> CalculatePerspectiveMatrix(KeyPointsPair pair1, KeyPointsPair pair2, KeyPointsPair pair3, KeyPointsPair pair4)
        {
            Matrix<double> lMatrix = Matrix.Build.Dense(8, 8, 0);

            lMatrix[0, 0] = pair1.KeyPoint1.X;
            lMatrix[1, 0] = pair2.KeyPoint1.X;
            lMatrix[2, 0] = pair3.KeyPoint1.X;
            lMatrix[3, 0] = pair4.KeyPoint1.X;

            lMatrix[0, 1] = pair1.KeyPoint1.Y;
            lMatrix[1, 1] = pair2.KeyPoint1.Y;
            lMatrix[2, 1] = pair3.KeyPoint1.Y;
            lMatrix[3, 1] = pair4.KeyPoint1.Y;

            lMatrix[0, 2] = 1;
            lMatrix[1, 2] = 1;
            lMatrix[2, 2] = 1;
            lMatrix[3, 2] = 1;

            lMatrix[4, 3] = pair1.KeyPoint1.X;
            lMatrix[5, 3] = pair2.KeyPoint1.X;
            lMatrix[6, 3] = pair3.KeyPoint1.X;
            lMatrix[7, 3] = pair4.KeyPoint1.X;

            lMatrix[4, 4] = pair1.KeyPoint1.Y;
            lMatrix[5, 4] = pair2.KeyPoint1.Y;
            lMatrix[6, 4] = pair3.KeyPoint1.Y;
            lMatrix[7, 4] = pair4.KeyPoint1.Y;

            lMatrix[4, 5] = 1;
            lMatrix[5, 5] = 1;
            lMatrix[6, 5] = 1;
            lMatrix[7, 5] = 1;

            lMatrix[0, 6] = -pair1.KeyPoint2.X * pair1.KeyPoint1.X;
            lMatrix[1, 6] = -pair2.KeyPoint2.X * pair2.KeyPoint1.X;
            lMatrix[2, 6] = -pair3.KeyPoint2.X * pair3.KeyPoint1.X;
            lMatrix[3, 6] = -pair4.KeyPoint2.X * pair4.KeyPoint1.X;
            lMatrix[4, 6] = -pair1.KeyPoint2.Y * pair1.KeyPoint1.X;
            lMatrix[5, 6] = -pair2.KeyPoint2.Y * pair2.KeyPoint1.X;
            lMatrix[6, 6] = -pair3.KeyPoint2.Y * pair3.KeyPoint1.X;
            lMatrix[7, 6] = -pair4.KeyPoint2.Y * pair4.KeyPoint1.X;

            lMatrix[0, 7] = -pair1.KeyPoint2.X * pair1.KeyPoint1.Y;
            lMatrix[1, 7] = -pair2.KeyPoint2.X * pair2.KeyPoint1.Y;
            lMatrix[2, 7] = -pair3.KeyPoint2.X * pair3.KeyPoint1.Y;
            lMatrix[3, 7] = -pair4.KeyPoint2.X * pair4.KeyPoint1.Y;
            lMatrix[4, 7] = -pair1.KeyPoint2.Y * pair1.KeyPoint1.Y;
            lMatrix[5, 7] = -pair2.KeyPoint2.Y * pair2.KeyPoint1.Y;
            lMatrix[6, 7] = -pair3.KeyPoint2.Y * pair3.KeyPoint1.Y;
            lMatrix[7, 7] = -pair4.KeyPoint2.Y * pair4.KeyPoint1.Y;

            Matrix<double> lMatrixInverse = lMatrix.Inverse();

            if (lMatrixInverse == null)
            {
                return null;
            }

            Matrix<double> rMatrix = Matrix.Build.Dense(8, 1);

            rMatrix[0, 0] = pair1.KeyPoint2.X;
            rMatrix[1, 0] = pair2.KeyPoint2.X;
            rMatrix[2, 0] = pair3.KeyPoint2.X;
            rMatrix[3, 0] = pair4.KeyPoint2.X;
            rMatrix[4, 0] = pair1.KeyPoint2.Y;
            rMatrix[5, 0] = pair2.KeyPoint2.Y;
            rMatrix[6, 0] = pair3.KeyPoint2.Y;
            rMatrix[7, 0] = pair4.KeyPoint2.Y;

            Matrix<double> parametersMatrix = lMatrixInverse.Multiply(rMatrix);

            Matrix<double> transformationMatrix = Matrix.Build.Dense(3, 3);

            transformationMatrix[0, 0] = parametersMatrix[0, 0];
            transformationMatrix[0, 1] = parametersMatrix[1, 0];
            transformationMatrix[0, 2] = parametersMatrix[2, 0];

            transformationMatrix[1, 0] = parametersMatrix[3, 0];
            transformationMatrix[1, 1] = parametersMatrix[4, 0];
            transformationMatrix[1, 2] = parametersMatrix[5, 0];

            transformationMatrix[2, 0] = parametersMatrix[6, 0];
            transformationMatrix[2, 1] = parametersMatrix[7, 0];
            transformationMatrix[2, 2] = 1;

            return transformationMatrix;
        }
    }
}