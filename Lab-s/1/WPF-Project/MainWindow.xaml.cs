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
            seriesFrequencies = GetSeriesFrequencies();
            intervalsInfo = GetIntervalsInfo();

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

        #region Vars...
        (double[] series, double[] frequency,
            double[] relatedFrequensy,
            double[] accumulatedRelatedFrequency) seriesFrequencies;
        (double x0, double h, int len,
            (double start, double end)[] sessions,
            double[] count, double[] positions) intervalsInfo;
        #endregion

        void BuildDiscreteSeries()
        {
            var arr = seriesFrequencies.series;
            var fr = seriesFrequencies.frequency;
            InsertDataInUniformedGrid(outDiscreteSeries, new double[][] { arr, fr } );

            var plt = outDiscreteSeriesPlot.Plot;
            plt.Title("Discrete series plot");
            plt.AddLollipop(fr, arr, colors.Green)
                .ShowValuesAboveBars = true;
            plt.YAxis.Label("Frequency");
            plt.XAxis.Label("Value");

            outDiscreteSeriesPlot.Refresh();
        }

        void BuildIntervalSeries()
        {
            var len = intervalsInfo.len;
            var x0 = intervalsInfo.x0;
            var h = intervalsInfo.h;
            var arr = Data.OrderBy(x => x).ToArray();

            var res = new string[len * 2];

            for (int i = 0; i < len; i++)
            {
                res[i] = $"{x0:f2} - {intervalsInfo.sessions[i].end:f2}";
                res[len + i] = intervalsInfo.count[i].ToString();
            }

            InsertDataInUniformedGrid(outIntervalSeries, res, (2, len));

            var count = intervalsInfo.count;

            var plt = outIntervalSeriesPlot.Plot;
            plt.Title("Interval series plot");
            var bar = plt.AddBar(count, intervalsInfo.positions, colors.Purple);
            bar.BarWidth = h;
            bar.ShowValuesAboveBars = true;
            plt.AddScatter(intervalsInfo.positions, count, colors.Gray).LineWidth = 2;
            outIntervalSeriesPlot.Refresh();
        }

        void BuildAccumulatedFrequencies()
        {
            var arr = seriesFrequencies.series;
            var acFr = seriesFrequencies.accumulatedRelatedFrequency;

            InsertDataInUniformedGrid(outAccumulatedFrequencies,
                new double[][] { seriesFrequencies.relatedFrequensy, acFr });

            var plt = outAccumulatedFrequenciesPlot.Plot;
            plt.Title("Sum curve");
            var bar = plt.AddBar(acFr,
                arr, colors.RosyBrown);
            bar.BarWidth = 0.1;
            plt.AddScatter(arr, acFr,
                colors.Black).LineWidth = 2;

            outAccumulatedFrequenciesPlot.Refresh();
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
            BuildAccumulatedFrequencies();
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

        (double[], double[], double[], double[]) GetSeriesFrequencies()
        {
            var arr = Data.OrderBy(x => x).ToArray();
            var fullLen = Data.Length;

            var temp = arr.GroupBy(x => x)
                .Select(group => new
                {
                    Value = group.Key,
                    Frequency = (double)group.Count(),
                    RelatedFrequency = Math.Round(group.Key / fullLen, 3)
                })
                .ToArray();

            var len = temp.Count();
            var accumulatedRelativeFrequencies = new double[len];
            accumulatedRelativeFrequencies[0] = temp[0].RelatedFrequency;

            for (int i = 1; i < len; i++)
            {
                accumulatedRelativeFrequencies[i] =
                    Math.Round(accumulatedRelativeFrequencies[i - 1] + temp[i].RelatedFrequency, 3);
            }

            return (temp.Select(x => x.Value).ToArray(),
                    temp.Select(x => x.Frequency).ToArray(),
                    temp.Select(x => x.RelatedFrequency).ToArray(),
                    accumulatedRelativeFrequencies
                    );
        }

        (double, double, int,
            (double, double)[],
            double[], double[]) GetIntervalsInfo()
        {
            var arr = Data.OrderBy(x => x).ToArray();
            var R = arr.Last() - arr.First();
            var k = Math.Sqrt(arr.Length);

            var h = Math.Round(R / k, 1);
            var x0 = arr.First() - 0.5 * h;
            var len = (int)Math.Round(k, MidpointRounding.AwayFromZero);
            var positions = new double[len];
            var startPosition = x0 + 0.5 * h;

            var sessions = new (double start, double end)[len];
            var count = new double[len];

            for (int i = 0; i < len; i++)
            {
                var sessionEnd = x0 + h;
                sessions[i] = (x0, sessionEnd);
                count[i] = arr.Count(x => x >= x0 && x < sessionEnd);
                positions[i] = startPosition;
                x0 += h;
                startPosition += h;
            }

            return (x0, h, len, sessions, count, positions);
        }
    }
}
