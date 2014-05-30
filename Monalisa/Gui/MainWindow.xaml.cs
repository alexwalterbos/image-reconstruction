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
        CancellationTokenSource ctc;

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

        private void TextBox_OnlyNumeric(object sender, TextCompositionEventArgs e)
        {
            if (!IsNaturalNumber(e.Text))
            {
                e.Handled = true;
            }
        }

        private void TextBox_OnlyDecimal(object sender, TextCompositionEventArgs e)
        {
            if (!IsNaturalNumber(e.Text) && e.Text != ".")
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
            bool found = false;
            if (File.Exists("Seed.bmp"))
            {
                var image = new Bitmap("Seed.bmp");
                SaveBitmap(image, this.SeedImage);
                TextBox_CanvasSizeX.Text = image.Width.ToString();
                TextBox_CanvasSizeY.Text = image.Height.ToString();
                image.Dispose();
            }

            //   SeedImage.Source = new BitmapImage(new Uri("Seed.bmp"));
        }

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

                var EA = new EvolutionaryAlgorithm()
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

                int? maxRuntime = null;
                int? maxEpochs = null;
                int? maxStagnation = null;
                double? minFitness = null;
                if (CheckBox_MaxRuntime.IsChecked == true) maxRuntime = int.Parse(TextBox_MaxRuntime.Text);
                if (CheckBox_MaxEpochs.IsChecked == true) maxEpochs = int.Parse(TextBox_MaxEpochs.Text);
                if (Checkbox_MaxStagnation.IsChecked == true) maxStagnation = int.Parse(TextBox_MaxStagnation.Text);
                if (Checkbox_MinFitness.IsChecked == true) minFitness = double.Parse(Textbox_MinFitness.Text);

                try
                {
                    EA.AlgorithmStarted += (s, args) => Dispatcher.Invoke(new Action(() =>
                        {
                            try
                            {
                                EA.Seed.Save("Seed.bmp");
                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                        }));
                    EA.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        if (MainImage.Source == null) return;
                        var bmp1 = ToBitmap(MainImage.Source as BitmapImage).AsByteArray();
                        var bmp2 = ToBitmap(SeedImage.Source as BitmapImage).AsByteArray();
                        var bmp3 = new byte[bmp1.Length];
                        for (int i = 0; i < bmp1.Length; i += 4)
                        {
                            var dB = Math.Abs((int)bmp1[i] - (int)bmp2[i]);
                            var dG = Math.Abs((int)bmp1[i + 1] - (int)bmp2[i + 1]);
                            var dR = Math.Abs((int)bmp1[i + 2] - (int)bmp2[i + 2]);
                            var dT = (dR + dG + dB) / 3;
                            bmp3[i / 4] = (byte)(255-dT);
                        }

                        SaveBitmap(bmp3.AsBitmap(EA.CanvasWidth, EA.CanvasHeight), CompareImage);
                    }));
                    EA.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() => SaveBitmap(Painter.Paint(EA, EA.Population.CalculateFittest()), MainImage)));
                    EA.EpochCompleted += (s, args) => Dispatcher.Invoke(new Action(() => Label_Status.Content = string.Format("Epoch:      {0, 6}\nStagnation: {2, 6}\nFitness:    {1,0:N4}\nRuntime:     {3:mm\\:ss}", EA.Epoch, EA.Fitness, EA.StagnationCount, EA.TimeRan)));
                    EA.AlgorithmCompleted += (s, args) => Dispatcher.Invoke(new Action(() =>
                    {
                        Button_Run.Content = "Run";
                        Label_Status.Content += "\nFinished";
                        ctc = null;
                    }));

                    await EA.RunAsync(() => StopCondition(EA, maxRuntime, maxEpochs, maxStagnation, minFitness), ctc.Token);
                }
                catch (OperationCanceledException)
                {
                    Label_Status.Content += "\nCanceled";
                    Painter.Paint(EA, EA.Population.CalculateFittest()).Save("Fittest.bmp");
                }
            }
            //var program = new Program(int.Parse(TextBox_PolygonCount.Text));
            //program.EpochCompleted += (a, b) => painter.Paint(program.Canvas);
            ////program.EpochCompleted += (a, b) => Debug.WriteLine("Epoch completed " + DateTime.Now);
            ////program.AlgorithmCompleted += (a, b) => { Button_Run.Content = "Finished"; };
            //await program.RunAsync();
        }

        private bool StopCondition(EvolutionaryAlgorithm EA, int? maxRuntime, int? maxEpochs, int? maxStagnation, double? minFitness)
        {
            bool stop = false;
            if (maxRuntime.HasValue) stop |= EA.TimeRan > TimeSpan.FromSeconds(maxRuntime.Value);
            if (maxEpochs.HasValue) stop |= EA.Epoch >= maxEpochs.Value;
            if (maxStagnation.HasValue) stop |= EA.StagnationCount >= maxStagnation.Value;
            if (minFitness.HasValue) stop |= EA.Fitness >= minFitness.Value;
            return stop;
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

        private Bitmap ToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }


    }
}
