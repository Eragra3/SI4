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

namespace SI4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FeaturesFile1Path.Text = @"C:\School\AI4\1\castle1.png.haraff.sift";
            UsedImage1Path.Text = @"C:\School\AI4\1\castle1.png";

            FeaturesFile2Path.Text = @"C:\School\AI4\1\castle2.png.haraff.sift";
            UsedImage2Path.Text = @"C:\School\AI4\1\castle2.png";
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

            Executor.Run(file1Path, file1SiftPath, file2Path, file2SiftPath);

        }

        private void DisplayError(string msg)
        {
            ErrorTextBox.Text = msg;
        }

        public void Update()
        {

        }
    }
}
