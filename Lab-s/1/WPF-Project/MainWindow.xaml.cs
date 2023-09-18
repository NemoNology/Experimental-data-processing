using ScottPlot;
using System;
using System.Data;
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
            intervalsInfo = GetIntervalsInfo();
            seriesFrequencies = GetSeriesFrequencies();

            Init();
        }

        static readonly double[] Data =
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
        readonly (double[] series, double[] frequency,
            double[] relatedFrequency,
            double[] accumulatedRelatedFrequency,
            double MoX, double MeX) seriesFrequencies;
        readonly (double x0, double h, int k,
            (double start, double end)[] sessions,
            double[] count, double[] positions) intervalsInfo;
        #endregion

        void BuildDiscreteSeries()
        {
            var arr = seriesFrequencies.series;
            var fr = seriesFrequencies.frequency;
            InsertDataInUniformedGrid(outDiscreteSeries,
                new double[][] { arr, fr }, 
                rowNames: new string[] { "Xi:", "Frequency:" });

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
            var len = intervalsInfo.k;
            var x0 = intervalsInfo.x0;
            var h = intervalsInfo.h;
            var arr = Data.OrderBy(x => x).ToArray();
            var dataLen = Data.Length;
            var count = intervalsInfo.count.Select(x => x / dataLen).ToArray();

            var res = new string[len * 2];

            for (int i = 0; i < len; i++)
            {
                res[i] = $"{x0:f2} - {intervalsInfo.sessions[i].end:f2}";
                res[len + i] = count[i].ToString();
            }

            InsertDataInUniformedGrid(outIntervalSeries, Array1DToArray2D(res, (2, len)), 
                rowNames: new string[] { "Intervals:", "Frequency:" });

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
            var colNames = Enumerable.Range(1, arr.Length).Select(x => x.ToString()).ToArray();

            InsertDataInUniformedGrid(outAccumulatedFrequencies,
                new double[][] { seriesFrequencies.relatedFrequency, acFr }, 
                rowNames: new string[] { "Related frequency:", "Accumulated frequency:" });

            var plt = outAccumulatedFrequenciesPlot.Plot;
            plt.Title("Sum curve");
            var bar = plt.AddBar(acFr,
                arr, colors.RosyBrown);
            bar.BarWidth = 0.1;
            plt.AddScatter(arr, acFr,
                colors.Black).LineWidth = 2;

            outAccumulatedFrequenciesPlot.Refresh();
        }

        void BuildTask4()
        {
            outMoX.Text = seriesFrequencies.MoX.ToString();
            outMeX.Text = seriesFrequencies.MeX.ToString();

            var columnNames = new string[] { "xi", "ni", "ui", "ni*ui", "ni*ui^2", "ni*ui^3", "ni*ui^4" };
            var Xi = intervalsInfo.positions;
            var cL = columnNames.Length;

            var res = new double[cL][];
            var temp = new double[cL];

        }

        void Init()
        {
            var rowNames = Enumerable.Range(1, 10).Select(x => x.ToString()).ToArray();
            var columnNames = new string[] { " " }.Concat(rowNames).ToArray();
            InsertDataInUniformedGrid(outInitialData, Array1DToArray2D(Data, (10, 10)), columnNames, rowNames);

            BuildDiscreteSeries();
            BuildIntervalSeries();
            BuildAccumulatedFrequencies();
            BuildTask4();
        }

        (double[], double[], double[], double[], double, double) GetSeriesFrequencies()
        {
            var arr = Data.OrderBy(x => x).ToArray();
            var fullLen = Data.Length;

            var temp = arr.GroupBy(x => x)
                .Select(group => new
                {
                    Value = group.Key,
                    Frequency = (double)group.Count(),
                    RelatedFrequency = Math.Round((double)group.Count() / fullLen, 3)
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
                    accumulatedRelativeFrequencies,
                    temp.MaxBy(x => x.Frequency)!.Value,
                    Math.Round((temp[intervalsInfo.k].Value + temp[intervalsInfo.k + 1].Value) / 2, 3)
                    );
        }

        static (double, double, int,
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

        #region Extended
        static T[][] Array1DToArray2D<T>(T[] array, (int rowCount, int columnCount) size)
        {
            var rC = size.rowCount;
            var cC = size.columnCount;
            var res = new T[rC][];

            for (int i = 0; i < rC; i++)
            {
                var temp = new T[cC];
                var Dj = i * cC;

                for (int j = 0; j < cC; j++)
                {
                    temp[j] = array[j + Dj];
                }

                res[i] = temp;
            }

            return res;
        }

        static void InsertDataInUniformedGrid<T>(UniformGrid grid, T[][] arr, string[]? columnNames = null, string[]? rowNames = null)
        {
            grid.Children.Clear();
            grid.Rows = rowNames is null ? arr.Length : arr.Length + 1;
            grid.Columns = ReferenceEquals(rowNames, columnNames) ?
                arr[0].Length : arr[0].Length + 1;

            if (columnNames is not null)
            {
                foreach (var colName in columnNames)
                {
                    grid.Children.Add(GetTextBlock(colName, true));
                }
            }

            var len = arr.Length;

            for (int i = 0; i < len; i++)
            {
                if (rowNames is not null)
                {
                    grid.Children.Add(GetTextBlock(rowNames[i], true));
                }

                foreach (var item in arr[i])
                {
                    grid.Children.Add(GetTextBlock(item!));
                }
            }
        }

        static TextBlock GetTextBlock(object content, bool IsHeader = false)
        {
            return new TextBlock
            {
                Foreground = Brushes.White,
                Padding = new Thickness(2),
                Background = IsHeader ? Brushes.SlateGray : Brushes.DimGray,
                FontSize = IsHeader ? 15 : 14,
                TextAlignment = IsHeader ?
                System.Windows.TextAlignment.Right : System.Windows.TextAlignment.Center,
                FontWeight = IsHeader ? FontWeights.Bold : FontWeights.Normal,
                Text = content.ToString()
            };
        }
        #endregion
    }
}
