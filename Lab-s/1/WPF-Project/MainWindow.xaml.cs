using ScottPlot;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using colors = System.Drawing.Color;

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
            Init();
        }

        static double[] Data =
        {
            61.2, 61.4, 60.2, 61.2, 61.3, 60.4, 61.4, 60.8, 61.2, 60.6,
            61.6, 60.2, 61.3, 60.3, 60.7, 60.9, 61.2, 60.5, 61.0, 61.4,
            61.1, 60.9, 61.5, 61.4, 60.6, 61.2, 60.1, 61.3, 61.1, 61.3,
            60.3, 61.3, 60.6, 61.7, 60.6, 61.2, 60.8, 61.3, 61.0, 61.2,
            60.5, 61.4, 60.7, 61.3, 60.9, 61.2, 61.1, 61.3, 60.9, 61.4,
            60.7, 61.2, 60.3, 61.1, 61.0, 61.5, 61.3, 61.9, 61.4, 61.3,
            61.6, 61.0, 61.7, 61.1, 60.9, 61.5, 61.6, 61.4, 61.5, 61.2,
            61.6, 61.3, 61.8, 61.1, 61.7, 60.9, 62.2, 61.1, 62.1, 61.0,
            61.5, 61.7, 62.3, 62.3, 61.7, 62.9, 62.5, 62.8, 62.6, 61.5,
            62.1, 62.6, 61.6, 62.5, 62.4, 62.3, 62.1, 62.3, 62.2, 62.1
        };

        void BuildDiscreteSeries()
        {
            var arr = Data.OrderBy(x => x).ToArray();

            var temp = arr.GroupBy(x => x)
                .Select(group => new { Value = group.Key, Count = (double)group.Count() })
                .ToArray();

            var len = temp.Count();
            var res = new double[][]
            {
                temp.Select(x => x.Value).ToArray(),
                temp.Select(x => x.Count).ToArray()
            };

            InsertDataInUniformedGrid(outDiscreteSeries, res);

            var plt = outDiscreteSeriesPlot.Plot;
            plt.Title("Discrete series plot");
            plt.AddLollipop(res[1], res[0], colors.Green)
                .ShowValuesAboveBars = true;
            plt.YAxis.Label("Frequency");
            plt.XAxis.Label("Value");

            outDiscreteSeriesPlot.Refresh();
        }

        void BuildIntervalSeries()
        {
            var arr = Data.OrderBy(x => x).ToArray();
            var R = arr.Last() - arr.First();
            var k = Math.Sqrt(arr.Length);

            var h = Math.Round(R / k, 1);
            var x0 = arr.First() - 0.5 * h;
            var len = (int)Math.Round(k, MidpointRounding.AwayFromZero);

            var resSessions = new (double sessionStart, double sessionEnd)[len];
            var resCount = new int[len];
            var res = new string[len * 2];

            var positions = new double[len];
            var startPosition = x0 + 0.5 * h;

            for (int i = 0; i < len; i++)
            {
                var sessionEnd = x0 + h;
                resSessions[i] = (x0, sessionEnd);
                var count = arr.Count(x => x >= x0 && x < sessionEnd);
                positions[i] = startPosition;
                resCount[i] = count;
                res[i] = $"{x0:f2} - {sessionEnd:f2}";
                res[len + i] = count.ToString();
                x0 += h;
                startPosition += h;
            }

            InsertDataInUniformedGrid(outIntervalSeries, res, (2, len));

            var plt = outIntervalSeriesPlot.Plot;
            plt.Title("Interval series plot");
            var bar = plt.AddBar(resCount.Select(x => (double)x).ToArray(),
                positions, colors.Purple);
            bar.BarWidth = h;
            bar.ShowValuesAboveBars = true;
            outIntervalSeriesPlot.Refresh();
        }

        static void InsertDataInUniformedGrid<T>(UniformGrid grid, T[] arr, (int rowsAmount, int columnAmount) size)
        {
            grid.Children.Clear();

            grid.Rows = size.rowsAmount;
            grid.Columns = size.columnAmount;

            foreach (var ar in arr)
            {
                grid.Children.Add(GetTextBlock(ar!));
            }
        }

        static void InsertDataInUniformedGrid<T>(UniformGrid grid, T[][] arr)
        {
            grid.Children.Clear();

            grid.Rows = arr.Length;
            grid.Columns = arr[0].Length;

            foreach (var ar in arr)
            {
                foreach (var item in ar)
                {
                    grid.Children.Add(GetTextBlock(item!));
                }
            }
        }

        void Init()
        {
            InsertDataInUniformedGrid(outInitialData, Data, (10, 10));

            BuildDiscreteSeries();
            BuildIntervalSeries();
        }

        static TextBlock GetTextBlock(object content)
        {
            return new TextBlock
            {
                Foreground = Brushes.White,
                Padding = new Thickness(5),
                Background = Brushes.DimGray,
                FontSize = 14,
                TextAlignment = System.Windows.TextAlignment.Center,
                Text = content.ToString()
            };
        }
    }
}
