using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPF_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // TODO: ScottPlot.WPF
            InsertArray2DInUniformedGrid(outInitialData, Data);
        }

        public static double[,] Data =
        {
            { 61.2, 61.4, 60.2, 61.2, 61.3, 60.4, 61.4, 60.8, 61.2, 60.6 },
            { 61.6, 60.2, 61.3, 60.3, 60.7, 60.9, 61.2, 60.5, 61.0, 61.4 },
            { 61.1, 60.9, 61.5, 61.4, 60.6, 61.2, 60.1, 61.3, 61.1, 61.3 },
            { 60.3, 61.3, 60.6, 61.7, 60.6, 61.2, 60.8, 61.3, 61.0, 61.2 },
            { 60.5, 61.4, 60.7, 61.3, 60.9, 61.2, 61.1, 61.3, 60.9, 61.4 },
            { 60.7, 61.2, 60.3, 61.1, 61.0, 61.5, 61.3, 61.9, 61.4, 61.3 },
            { 61.6, 61.0, 61.7, 61.1, 60.9, 61.5, 61.6, 61.4, 61.5, 61.2 },
            { 61.6, 61.3, 61.8, 61.1, 61.7, 60.9, 62.2, 61.1, 62.1, 61.0 },
            { 61.5, 61.7, 62.3, 62.3, 61.7, 62.9, 62.5, 62.8, 62.6, 61.5 },
            { 62.1, 62.6, 61.6, 62.5, 62.4, 62.3, 62.1, 62.3, 62.2, 62.1 }
        };

        void InsertArrayInTextBlock<T>(TextBlock view, T[] arr, string separator = "", int padRight = 0)
        {
            foreach (var t in arr)
            {
                view.Text += (t!.ToString() + separator).PadRight(padRight);
            }
        }

        T[] Array2DToArray1D<T>(T[,] arr)
        {
            var res = new T[arr.Length];
            var counter = 0;

            foreach (var item in arr)
            {
                res[counter] = item;
                counter++;
            }

            return res;
        }

        void CalculateVariantsFrequency()
        {
            var temp = Array2DToArray1D(Data);

            Array.Sort(temp);

            // TODO: calculating variants and their frequency
            //var newLen = temp.Count();
            
            //double[,] res = ;

            //InsertArray2DInUniformedGrid(outVariants, res);
        }

        void InsertArray2DInUniformedGrid<T>(UniformGrid grid, T[,] arr)
        {
            grid.Rows = arr.GetLength(0);
            grid.Columns = arr.GetLength(1);

            foreach (var ar in arr)
            {
                var cell = new TextBlock();
                cell.Foreground = Brushes.White;
                cell.Padding = new Thickness(5);
                cell.Background = Brushes.DimGray;
                cell.Text = ar!.ToString();

                grid.Children.Add(cell);
            }
        }

        private void On_Calculate_Click(object sender, RoutedEventArgs e)
        {
            CalculateVariantsFrequency();
        }
    }
}
