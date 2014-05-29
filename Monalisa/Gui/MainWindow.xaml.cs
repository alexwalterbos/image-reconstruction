using Microsoft.Win32;
using org.monalisa.algorithm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing.Imaging;

namespace org.monalisa.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetToDefaults();
        }

        // Image uploader
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                SeedImage.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void TextBox_OnlyNumeric(object sender, TextCompositionEventArgs e)
        {
            if (!IsNaturalNumber(e.Text))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Returns true iff is parsable to int.
        /// </summary>
        /// <param name="text">text to test.</param>
        /// <returns>True if is a natural number, false otherwise.</returns>
        private bool IsNaturalNumber(string text)
        {
            int result;
            return int.TryParse(text, out result);
        }

        /// <summary>
        /// Hardcoded defaults
        /// TODO: Not hardcode the defaults ;)
        /// </summary>
        public void SetToDefaults()
        {
            TextBox_PopulationSize.Text = "50";
            TextBox_CanvasSizeX.Text = "320";
            TextBox_CanvasSizeY.Text = "240";
            TextBox_PolygonCount.Text = "3";
            ComboBox_PolygonType.SelectedIndex = 0;
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            var EA = new EvolutionaryAlgorithm();
            EA.AlgorithmStarted +=   (s, args) => Dispatcher.Invoke(new Action(() => SaveBitmap(EA.Seed, SeedImage)));
            EA.EpochCompleted +=     (s, args) => Dispatcher.Invoke(new Action(() => SaveBitmap(Painter.Paint(EA, EA.Population.CalculateFittest()), MainImage)));
            EA.EpochCompleted +=     (s, args) => Dispatcher.Invoke(new Action(() => Label_Status.Content = string.Format("Epoch:      {0, 5}\nStagnation: {2, 5}\nFitness:    {1,0:N3}\nRuntime:    {3:mm\\:ss}", EA.Epoch, EA.Fitness, EA.StagnationCount, EA.TimeRan)));
            EA.AlgorithmCompleted += (s, args) => Dispatcher.Invoke(new Action(() => Button_Run.Content = "Finished"));
            await EA.RunAsync(() => EA.StagnationCount > 20);
            
            //var program = new Program(int.Parse(TextBox_PolygonCount.Text));
            //program.EpochCompleted += (a, b) => painter.Paint(program.Canvas);
            ////program.EpochCompleted += (a, b) => Debug.WriteLine("Epoch completed " + DateTime.Now);
            ////program.AlgorithmCompleted += (a, b) => { Button_Run.Content = "Finished"; };
            //await program.RunAsync();
        }

        private void SaveBitmap(Bitmap bitmap, System.Windows.Controls.Image imageCanvas)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                imageCanvas.Source = bitmapImage;
            }
        }
    }
}
