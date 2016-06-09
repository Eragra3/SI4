using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using SI4.Parser;
using System.Windows.Shapes;
using SI4.Logic;
using SI4.Models;

namespace SI4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AlgorythmTypeEnum UsedAlgorithm;

        private double image1Width = 0;
        private double image1Height = 0;
        private double image2Width = 0;
        private double image2Height = 0;

        private bool _usePerspective = false;

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += HideCanvasHandler;

            FeaturesFile1Path.Text = @"C:\School\AI4\1\castle1.png.haraff.sift";
            UsedImage1Path.Text = @"C:\School\AI4\1\castle1.png";

            FeaturesFile2Path.Text = @"C:\School\AI4\1\castle2.png.haraff.sift";
            UsedImage2Path.Text = @"C:\School\AI4\1\castle2.png";

            BitmapImage bi1 = new BitmapImage(new Uri(UsedImage1Path.Text));
            image1Height = bi1.PixelHeight;
            image1Width = bi1.PixelWidth;
            BitmapImage bi2 = new BitmapImage(new Uri(UsedImage2Path.Text));
            image2Height = bi2.PixelHeight;
            image2Width = bi2.PixelWidth;

            Image1.Source = bi1;
            Image2.Source = bi2;

            NeighboursCount.Text = "100";
            NeighboursThreshold.Text = "0,6";
            IterationsCount.Text = "25";
            MaxError.Text = "100";
        }

        private async void RunAlgorithm(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();

            string file1Path = UsedImage1Path.Text;
            string file2Path = UsedImage2Path.Text;

            string file1SiftPath = FeaturesFile1Path.Text;
            string file2SiftPath = FeaturesFile2Path.Text;

            if (!File.Exists(file1Path))
            {
                DisplayError($"Can't find file {file1Path}");
                return;
            }
            if (!File.Exists(file2Path))
            {
                DisplayError($"Can't find file {file2Path}");
                return;
            }
            if (!File.Exists(file1SiftPath))
            {
                DisplayError($"Can't find file {file1SiftPath}");
                return;
            }
            if (!File.Exists(file2SiftPath))
            {
                DisplayError($"Can't find file {file2SiftPath}");
                return;
            }

            KeyPoint[] image1Features = await Task.Factory.StartNew(() => SiftParser.Parse(file1SiftPath));

            KeyPoint[] image2Features = await Task.Factory.StartNew(() => SiftParser.Parse(file2SiftPath));


            Console.WriteLine($"Image 1 features count : {image1Features.Length}");
            Console.WriteLine($"Image 2 features count : {image2Features.Length}");

            foreach (var keyPoint in image1Features)
            {
                DrawPoint(keyPoint.X, keyPoint.Y, Brushes.Red);
            }

            foreach (var keyPoint in image2Features)
            {
                DrawPoint(keyPoint.X, keyPoint.Y, Brushes.Red, false);
            }


            List<KeyPointsPair> keyPointPairs = await Task.Factory.StartNew(() => MainExecutor.FindKeyPointsPairs(image1Features, image2Features));

            Console.WriteLine($"Paired points count : {keyPointPairs.Count}");

            //foreach (KeyPointsPair keyPoint in keyPointPairs)
            //{
            //    DrawPoint(keyPoint.X1, keyPoint.Y1, Brushes.Blue, true, 2);
            //    DrawPoint(keyPoint.X2, keyPoint.Y2, Brushes.Blue, false, 2);
            //    DrawLine(keyPoint.X1, keyPoint.Y1, keyPoint.X2, keyPoint.Y2, Brushes.Yellow);
            //}

            var neighboursCount = int.Parse(NeighboursCount.Text);
            var neighboursThreshold = double.Parse(NeighboursThreshold.Text.Replace(',', '.'), CultureInfo.InvariantCulture);


            var maxError = int.Parse(MaxError.Text);
            var iterationsCount = int.Parse(IterationsCount.Text);

            var useKeyPointPickingHeuristic = KeyPointsPickingHeuristic.IsChecked != null &&
                                              KeyPointsPickingHeuristic.IsChecked.Value;

            List<KeyPointsPair> coherentPairs = await Task.Factory.StartNew(() =>
            {
                switch (UsedAlgorithm)
                {
                    case AlgorythmTypeEnum.ClosestNeighboor:
                        return CoherenceAnalysisExecutor.FindCoherentPairs(
                            keyPointPairs,
                            neighboursCount,
                            neighboursThreshold
                            );
                    case AlgorythmTypeEnum.Ransac:
                        return RansacExecutor.FindCoherentPairs(
                            keyPointPairs,
                            maxError,
                            iterationsCount,
                            _usePerspective,
                            useKeyPointPickingHeuristic,
                            0.01 * image1Width * 0.01 * image1Width,
                            0.3 * image1Width * 0.3 * image1Width);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            );

            Console.WriteLine($"Coherent pairs : {coherentPairs.Count}");

            foreach (var pair in coherentPairs)
            {
                DrawPoint(pair.KeyPoint1.X, pair.KeyPoint1.Y, Brushes.Blue, true, 2);
                DrawPoint(pair.KeyPoint2.X, pair.KeyPoint2.Y, Brushes.Blue, false, 2);
                DrawLine(pair.KeyPoint1.X, pair.KeyPoint1.Y, pair.KeyPoint2.X, pair.KeyPoint2.Y, Brushes.Yellow);
            }
        }

        private void DisplayError(string msg)
        {
            ErrorTextBox.Text = msg;
        }

        private void DrawPoint(double x, double y, Brush color, bool topImage = true, int radius = 3)
        {
            if (topImage)
            {
                double scale = Image1.ActualWidth / image1Width;
                x *= scale;
                y *= scale;

                double leftOffset = (Canvas.ActualWidth - Image1.ActualWidth) / 2;
                x += leftOffset;
            }
            else
            {
                double scale = Image2.ActualWidth / image2Width;
                x *= scale;
                y *= scale;

                double leftOffset = (Canvas.ActualWidth - Image2.ActualWidth) / 2;
                double topOffset = Image1.ActualHeight;
                x += leftOffset;
                y += topOffset;
            }

            Ellipse circle = new Ellipse()
            {
                Width = radius,
                Height = radius,
                Fill = color
            };

            Canvas.Children.Add(circle);
            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush color)
        {
            double scale1 = Image1.ActualWidth / image1Width;
            x1 *= scale1;
            y1 *= scale1;

            double leftOffset1 = (Canvas.ActualWidth - Image1.ActualWidth) / 2;
            x1 += leftOffset1;

            double scale2 = Image2.ActualWidth / image2Width;
            x2 *= scale2;
            y2 *= scale2;

            double leftOffset2 = (Canvas.ActualWidth - Image2.ActualWidth) / 2;
            double topOffset2 = Image1.ActualHeight;
            x2 += leftOffset2;
            y2 += topOffset2;

            Line line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = color
            };

            Canvas.Children.Add(line);
        }

        private void UsePerspective(object sender, RoutedEventArgs e)
        {
            _usePerspective = true;
        }

        private void UseAffine(object sender, RoutedEventArgs e)
        {
            _usePerspective = false;
        }

        private void HideCanvasHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.H) return;

            Canvas.Visibility = Canvas.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
