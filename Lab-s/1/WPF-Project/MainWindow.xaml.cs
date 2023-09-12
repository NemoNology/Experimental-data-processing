using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            Insert2DArrayInTextBlock(outInitialData, Data, "", 7);
            CalculateVariantsFrequency();

            // TODO: ScottPlot.WPF
        }

        public static double[,] Data { get; } =
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

        void Insert2DArrayInTextBlock<T>(TextBlock view, T[,] rows, string separator = "", int padRight = 0)
        {
            for (int i = 0; i < rows.GetLength(0); i++)
            {
                var len = rows.GetLength(1);

                for (int j = 0; j < len; j++)
                {
                    if (j + 1 != len)
                    {
                        view.Text += (rows[i, j]!.ToString()! + separator).PadRight(padRight);
                    }
                }

                view.Text += "\n";
            }
        }

        void InsertArrayInTextBlock<T>(TextBlock view, T[] arr, string separator = "", int padRight = 0)
        {
            foreach (var t in arr)
            {
                view.Text += (t!.ToString() + separator).PadRight(padRight);
            }
        }

        T[] Array2DToArray1D<T>(T[,] arr)
        {
            var len1 = arr.GetLength(0);
            var len2 = arr.GetLength(1);

            var res = new T[len1 * len2];

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    res[i * len1 + j] = arr[i, j];
                }
            }

            return res;
        }

        void CalculateVariantsFrequency()
        {
            var temp = Array2DToArray1D(Data);

            Array.Sort(temp);

            var res = from num in temp
                      group num by num into nums
                      let frequency = nums.Count()
                      select new { Value = nums.Key, Frequency = frequency };

            InsertArrayInTextBlock(outVariants, res.Select(x => x.Value).ToArray(), "", 7);
            InsertArrayInTextBlock(outVariantsFrequency, res.Select(x => x.Frequency).ToArray(), "", 10);
        }
    }
}
