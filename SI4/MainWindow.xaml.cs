using System;
using System.Collections.Generic;
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
using SI4.Models;

namespace SI4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AlgorythmTypeEnum UsedAlgorithm;

        public MainWindow()
        {
            InitializeComponent();

            FeaturesFile1Path.Text = @"C:\School\AI4\1\castle1.png.haraff.sift";
            UsedImage1Path.Text = @"C:\School\AI4\1\castle1.png";

            FeaturesFile2Path.Text = @"C:\School\AI4\1\castle2.png.haraff.sift";
            UsedImage2Path.Text = @"C:\School\AI4\1\castle2.png";

            Image1.Source = new BitmapImage(new Uri(UsedImage1Path.Text));
            Image2.Source = new BitmapImage(new Uri(UsedImage2Path.Text));
        }

        private void SetFeaturesFile1(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Sift files (*.sift)|*.sift"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            FeaturesFile1Path.Text = dialog.FileName;

        }

        private void SetUsedImage1(object sender, RoutedEventArgs e)
        {

            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.png"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            UsedImage1Path.Text = dialog.FileName;

            Image1.Source = new BitmapImage(new Uri(UsedImage1Path.Text));
        }

        private void SetFeaturesFile2(object sender, RoutedEventArgs e)
        {

            var dialog = new OpenFileDialog
            {
                Filter = "Sift files (*.sift)|*.sift"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            FeaturesFile2Path.Text = dialog.FileName;
        }

        private void SetUsedImage2(object sender, RoutedEventArgs e)
        {

            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.png"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            UsedImage2Path.Text = dialog.FileName;

            Image2.Source = new BitmapImage(new Uri(UsedImage2Path.Text));
        }

        private void RunAlgorithm(object sender, RoutedEventArgs e)
        {
            var file1Path = UsedImage1Path.Text;
            var file2Path = UsedImage2Path.Text;

            var file1SiftPath = FeaturesFile1Path.Text;
            var file2SiftPath = FeaturesFile2Path.Text;

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

            var image1Features = SiftParser.Parse(file1SiftPath);
            var image2Features = SiftParser.Parse(file2SiftPath);

            foreach (var keyPoint in image1Features)
            {
                DrawPoint(keyPoint.X, keyPoint.Y, Brushes.Red);
            }
            foreach (var keyPoint in image2Features)
            {
                DrawPoint(keyPoint.X, keyPoint.Y, Brushes.Red);
            }


            List<KeyPointsPair> keyPointPairs;
            switch (UsedAlgorithm)
            {
                case AlgorythmTypeEnum.ClosestNeighboor:
                    keyPointPairs = Executor.FindKeyPointsPairs(image1Features, image2Features);
                    break;
                case AlgorythmTypeEnum.Ransac:
                    keyPointPairs = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            foreach (var keyPoint in keyPointPairs)
            {
                DrawPoint(keyPoint.X1, keyPoint.Y1, Brushes.Blue);
                DrawPoint(keyPoint.X2, keyPoint.Y2, Brushes.Blue);
            }
        }

        private void DisplayError(string msg)
        {
            ErrorTextBox.Text = msg;
        }

        private void DrawPoint(double x, double y, Brush color, bool topImage = true)
        {
            if (topImage)
            {
                var leftOffset = (Canvas.ActualWidth - Image1.ActualWidth) / 2;
                x += leftOffset;
            }
            else
            {
                var leftOffset = (Canvas.ActualWidth - Image2.ActualWidth) / 2;
                var topOffset = Image1.ActualHeight;
                x += leftOffset;
                y += topOffset;
            }

            var circle = new Ellipse()
            {
                Width = 3,
                Height = 3,
                Fill = color
            };

            Canvas.Children.Add(circle);
            Canvas.SetLeft(circle, x);
            Canvas.SetTop(circle, y);
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush color)
        {
            var leftOffset1 = (Canvas.ActualWidth - Image1.ActualWidth) / 2;
            x1 += leftOffset1;

            var leftOffset2 = (Canvas.ActualWidth - Image2.ActualWidth) / 2;
            var topOffset2 = Image1.ActualHeight;
            x2 += leftOffset2;
            y2 += topOffset2;

            var line = new Line()
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

        public void Update()
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var usedAlgorithmText = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                if (string.IsNullOrEmpty(usedAlgorithmText))
                    UsedAlgorithm = AlgorythmTypeEnum.ClosestNeighboor;
                else
                    UsedAlgorithm = (AlgorythmTypeEnum)Enum.Parse(typeof(AlgorythmTypeEnum), usedAlgorithmText);
            }
            catch (Exception)
            {
                UsedAlgorithm = AlgorythmTypeEnum.ClosestNeighboor;
            }
        }
    }
}
