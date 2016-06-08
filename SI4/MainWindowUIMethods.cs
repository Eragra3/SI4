using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SI4
{
    public partial class MainWindow
    {

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string usedAlgorithmText = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                if (string.IsNullOrEmpty(usedAlgorithmText))
                    UsedAlgorithm = AlgorythmTypeEnum.ClosestNeighboor;
                else
                    UsedAlgorithm = (AlgorythmTypeEnum)Enum.Parse(typeof(AlgorythmTypeEnum), usedAlgorithmText);
            }
            catch (Exception)
            {
                UsedAlgorithm = AlgorythmTypeEnum.ClosestNeighboor;
            }

            if (RansackTransformType != null)
            {
                RansackTransformType.Visibility = UsedAlgorithm == AlgorythmTypeEnum.Ransac ? Visibility.Visible : Visibility.Hidden;
            }
        }


        private void SetFeaturesFile1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Sift files (*.sift)|*.sift"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            FeaturesFile1Path.Text = dialog.FileName;

        }

        private void SetUsedImage1(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.png"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            UsedImage1Path.Text = dialog.FileName;

            BitmapImage bi = new BitmapImage(new Uri(UsedImage1Path.Text));
            Image1.Source = bi;

            image1Height = bi.PixelHeight;
            image1Width = bi.PixelWidth;
        }

        private void SetFeaturesFile2(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Sift files (*.sift)|*.sift"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            FeaturesFile2Path.Text = dialog.FileName;
        }

        private void SetUsedImage2(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.png"
            };


            if (!(dialog.ShowDialog() ?? false)) return;

            UsedImage2Path.Text = dialog.FileName;

            BitmapImage bi = new BitmapImage(new Uri(UsedImage2Path.Text));
            Image2.Source = bi;

            image2Height = bi.PixelHeight;
            image2Width = bi.PixelWidth;
        }
    }
}
