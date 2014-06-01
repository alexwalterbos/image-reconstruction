//-----------------------------------------------------------------------------
// <copyright 
//  file="MainWindow.xaml.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Gui
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using Microsoft.Win32;
    using Org.Monalisa.Algorithm;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Used for canceling interaction with Algorithm
        /// </summary>
        private CancellationTokenSource ctc;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory("img");
            Directory.CreateDirectory("saves");
            LoadPreviousSeed();
        }

        /// <summary>
        /// Uploads a new seed image
        /// </summary>
        /// <param name="sender">Event initiator</param>
        /// <param name="e">Event arguments</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png|" +
                "Bitmap (*.bmp)|*.bmp";
            if (op.ShowDialog() == true)
            {
                var image = new BitmapImage(new Uri(op.FileName));
                TextBox_CanvasSizeX.Text = image.PixelWidth.ToString();
                TextBox_CanvasSizeY.Text = image.PixelHeight.ToString();
                SeedImage.Source = image;
            }
        }

        /// <summary>
        /// Makes sure only numeric text can be entered
        /// </summary>
        /// <param name="sender">Event initiator</param>
        /// <param name="e">Event arguments</param>
        private void OnlyNumeric(object sender, TextCompositionEventArgs e)
        {
            if (!IsNaturalNumber(e.Text))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Makes sure only decimal text can be entered
        /// </summary>
        /// <param name="sender">Event Initiator</param>
        /// <param name="e">Event arguments</param>
        private void OnlyDecimal(object sender, TextCompositionEventArgs e)
        {
            if (!IsNaturalNumber(e.Text) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Returns true if it string can be parsed to integer.
        /// </summary>
        /// <param name="text">text to test.</param>
        /// <returns>True if is a natural number, false otherwise.</returns>
        private bool IsNaturalNumber(string text)
        {
            int result;
            return int.TryParse(text, out result);
        }

        /// <summary>
        /// Load previous seed if exists
        /// </summary>
        private void LoadPreviousSeed()
        {
            if (File.Exists("img/Seed.bmp"))
            {
                var image = new Bitmap("img/Seed.bmp");
                SaveBitmap(image, this.SeedImage);
                TextBox_CanvasSizeX.Text = image.Width.ToString();
                TextBox_CanvasSizeY.Text = image.Height.ToString();
                image.Dispose();
            }
        }

        /// <summary>
        /// Called when run button is clicked
        /// </summary>
        /// <param name="sender">Event initiator</param>
        /// <param name="e">Event arguments</param>
        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            if (SeedImage.Source == null)
            {
                Label_Status.Content = "No image set!";
                return;
            }

            if (ctc != null)
            {
                ctc.Cancel();
                ctc = null;
                Button_Run.Content = "Run";
            }
            else
            {
                Button_Run.Content = "Stop";
                ctc = new CancellationTokenSource();

                var algorithm = new EvolutionaryAlgorithm()
                {
                    CanvasWidth = int.Parse(TextBox_CanvasSizeX.Text),
                    CanvasHeight = int.Parse(TextBox_CanvasSizeY.Text),
                    CanvasCount = int.Parse(TextBox_PopulationSize.Text),
                    PolygonCount = int.Parse(TextBox_PolygonCount.Text),
                    PolygonEdgeCount = int.Parse(((ComboBoxItem)ComboBox_PolygonType.SelectedItem).Tag.ToString()),
                    WeightPositionChange = double.Parse(this.TextBox_PositionChange.Text),
                    WeightColorChange = double.Parse(this.TextBox_ColorChange.Text),
                    WeightIndexChange = double.Parse(this.TextBox_ZIndexChange.Text),
                    WeightRandomChange = double.Parse(this.TextBox_RandomChange.Text),
                    Seed = ToBitmap((BitmapImage)SeedImage.Source)
                };

                // If a saves/Serialized.canvas exists, load it.
                LoadSerialzedCanvasIfExists(algorithm);

                // Get user defined values from the GUI where the checkbox/textbox structure applies
                int? maxRuntime = GetIntIfFilledIn(CheckBox_MaxRuntime, TextBox_MaxRuntime);
                int? maxEpochs = GetIntIfFilledIn(CheckBox_MaxEpochs, TextBox_MaxEpochs);
                int? maxStagnation = GetIntIfFilledIn(Checkbox_MaxStagnation, TextBox_MaxStagnation);
                double? minFitness = GetDoubleIfFilledIn(Checkbox_MinFitness, Textbox_MinFitness);

                try
                {
                    algorithm.AlgorithmStarted += (s, args) => Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                algorithm.Seed.Save("img/Seed.bmp");
                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                        }));
                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        if (MainImage.Source == null)
                        {
                            return;
                        }

                        GenerateDiffImageAndSave(algorithm);
                    }));
                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() => Label_Similar.Content = string.Format("{0:P2}", algorithm.Fitness)));
                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        if (algorithm.Epoch % 1000 == 1)
                        {
                            Painter.Paint(algorithm.Population.CalculateFittest()).Save(string.Format("img/Epoch_{0}K.bmp", algorithm.Epoch / 1000));
                        }
                    }));

                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() => SaveBitmap(Painter.Paint(algorithm.Population.CalculateFittest()), MainImage)));
                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() => Label_Status.Content = string.Format("Epoch:      {0, 11}\nStagnation: {2, 11}\nFitness:    {1,11:N6}\nRuntime:     {3:d\\.hh\\:mm\\:ss}", algorithm.Epoch, algorithm.Fitness, algorithm.StagnationCount, algorithm.TimeRan)));
                    algorithm.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        if (algorithm.Epoch % 1000 == 0)
                        {
                            SaveSerializedCanvas(algorithm);
                        }
                        if (algorithm.Epoch % 100000 == 0)
                        {
                            SaveSerializedCanvas(algorithm, string.Format("saves/Epoch_{0}k", algorithm.Epoch));
                        }
                    }));
                    algorithm.AlgorithmCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        Painter.Paint(algorithm.Population.CalculateFittest()).Save(string.Format("img/Epoch_{0}K{1}.bmp", algorithm.Epoch / 1000, algorithm.Epoch % 1000));
                        Button_Run.Content = "Run";
                        Label_Status.Content += "\nFinished";
                        ctc = null;
                    }));

                    await algorithm.RunAsync(() => StopCondition(algorithm, maxRuntime, maxEpochs, maxStagnation, minFitness), ctc.Token);
                }
                catch (OperationCanceledException)
                {
                    Label_Status.Content += "\nCanceled";
                    Painter.Paint(algorithm.Population.CalculateFittest()).Save(string.Format("img/Epoch_{0}K{1}.bmp", algorithm.Epoch / 1000, algorithm.Epoch % 1000));
                }
            }
        }

        private void SaveSerializedCanvas(EvolutionaryAlgorithm algorithm, string fileName = "saves/Suspended.canvas")
        {
            var bytes = algorithm.Population.CalculateFittest().AsByteArray();
            File.WriteAllBytes(fileName, bytes);
        }

        private void LoadSerializedCanvas(EvolutionaryAlgorithm algorithm, string fileName = "saves/Suspended.canvas")
        {
            var canvas = File.ReadAllBytes(fileName).AsCanvas(algorithm);
            if (algorithm.Population == null || algorithm.Population.Count == 0)
            {
                algorithm.Population = new List<ICanvas>(){canvas};
            }
            else
            {
                algorithm.Population.Add(canvas);
            }
        }

        private void LoadSerialzedCanvasIfExists(EvolutionaryAlgorithm algorithm, string fileName = "saves/Suspended.canvas")
        {
            if (File.Exists(fileName))
            {
                LoadSerializedCanvas(algorithm, fileName);
            }
        }

        private void GenerateDiffImageAndSave(EvolutionaryAlgorithm algorithm)
        {
            var mainImage = ToBitmap(MainImage.Source as BitmapImage).AsByteArray();
            var seedImage = ToBitmap(SeedImage.Source as BitmapImage).AsByteArray();
            var diffImage = new byte[mainImage.Length];
            for (int i = 0; i < mainImage.Length; i += 4)
            {
                var dB = Math.Abs((int)mainImage[i] - (int)seedImage[i]);
                var dG = Math.Abs((int)mainImage[i + 1] - (int)seedImage[i + 1]);
                var dR = Math.Abs((int)mainImage[i + 2] - (int)seedImage[i + 2]);
                var dT = (dR + dG + dB) / 3;
                diffImage[i / 4] = (byte)(255 - dT);
            }

            SaveBitmap(diffImage.AsBitmap(algorithm.CanvasWidth, algorithm.CanvasHeight), CompareImage);
        }

        /// <summary>
        /// Checks if <paramref name="checkbox"/> is checked, and if so, it parses the value in <paramref name="textbox"/>
        /// </summary>
        /// <param name="checkbox">CheckBox that expresses whether the value in <paramref name="textbox"/> should be used</param>
        /// <param name="textbox">TextBox from which the value is taken if <paramref name="checkbox"/> is checked.</param>
        /// <returns></returns>
        private int? GetIntIfFilledIn(CheckBox checkbox, TextBox textbox)
        {
            if (checkbox.IsChecked == true)
            {
                return int.Parse(textbox.Text);
            }

            return null;
        }

        /// <summary>
        /// Same as GetIntIfFilledIn, but for double.
        /// </summary>
        /// <param name="checkbox"></param>
        /// <param name="textbox"></param>
        /// <returns></returns>
        private double? GetDoubleIfFilledIn(CheckBox checkbox, TextBox textbox)
        {
            if (checkbox.IsChecked == true)
            {
                return double.Parse(textbox.Text);
            }

            return null;
        }

        /// <summary>
        /// Return true if one of the constraints gets violated
        /// </summary>
        /// <param name="ea">Algorithm to check constraints on</param>
        /// <param name="maxRuntime">Maximum algorithm runtime</param>
        /// <param name="maxEpochs">Maximum algorithm iterations</param>
        /// <param name="maxStagnation">Maximum time no change occurs</param>
        /// <param name="minFitness">Minimum achieved fitness</param>
        /// <returns>True if algorithm should stop</returns>
        private bool StopCondition(
            EvolutionaryAlgorithm ea, 
            int? maxRuntime,
            int? maxEpochs, 
            int? maxStagnation, 
            double? minFitness)
        {
            bool stop = false;
            if (maxRuntime.HasValue)
            {
                stop |= ea.TimeRan > TimeSpan.FromSeconds(maxRuntime.Value);
            }

            if (maxEpochs.HasValue)
            {
                stop |= ea.Epoch >= maxEpochs.Value;
            }

            if (maxStagnation.HasValue)
            {
                stop |= ea.StagnationCount >= maxStagnation.Value;
            }

            if (minFitness.HasValue)
            {
                stop |= ea.Fitness >= minFitness.Value;
            }

            return stop;
        }

        /// <summary>
        /// Displays a bitmap on screen using memory stream.
        /// </summary>
        /// <param name="bitmap">Bitmap to show</param>
        /// <param name="imageCanvas">Image holder to show it in</param>
        private void SaveBitmap(
            Bitmap bitmap,
            System.Windows.Controls.Image imageCanvas)
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

        /// <summary>
        /// Converts a <see cref="BitmapImage"/> to a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmapImage">Bitmap image to convert</param>
        /// <returns>The converted bitmap</returns>
        private Bitmap ToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                var bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
