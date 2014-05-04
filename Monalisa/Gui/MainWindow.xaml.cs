using Microsoft.Win32;
using org.monalisa.algorithm;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace org.monalisa.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Painter painter;

        public MainWindow()
        {
            InitializeComponent();
            SetToDefaults();
            painter = new Painter(MainCanvas);
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

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            painter.Paint(new Program(int.Parse(TextBox_PolygonCount.Text)));
        }
    }

    class Program : ICanvas
    {
        public int PolygonCount { get; set; }
        public Program(int polygonCount = 3)
        {
            PolygonCount = polygonCount;

        }

        public ICollection<IShape> Elements
        {
            get
            {
                var elements = new List<IShape>(PolygonCount);
                var random = new Random();
                for (int i = 0; i < PolygonCount; i++)
                    elements.Add(new RandomTriangle(r: random));
                return elements;
            }
        }
    }

    class RandomTriangle : IPolygon
    {
        public RandomTriangle(int maxWidth = 320, int maxHeight = 240, Random r = null)
        {
            if (r == null) r = new Random();
            var p1 = new Tuple<int, int>(r.Next(maxWidth), r.Next(maxHeight));
            var p2 = new Tuple<int, int>(r.Next(maxWidth), r.Next(maxHeight));
            var p3 = new Tuple<int, int>(r.Next(maxWidth), r.Next(maxHeight));
            Coordinates = new List<Tuple<int, int>>(new Tuple<int, int>[] { p1, p2, p3 });
            Alpha = (byte)r.Next(255);
            Red = (byte)r.Next(255);
            Green = (byte)r.Next(255);
            Blue = (byte)r.Next(255);
        }

        public ICollection<Tuple<int, int>> Coordinates { get; private set; }
        public byte Alpha { get; private set; }
        public byte Red { get; private set; }
        public byte Green { get; private set; }
        public byte Blue { get; private set; }
    }
}
