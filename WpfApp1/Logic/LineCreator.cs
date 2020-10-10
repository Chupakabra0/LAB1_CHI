using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace WpfApp1.Logic {
    public static class LineCreator {
        public static LineSeries CreateLine(IEnumerable<Tuple<double, double>> dotsList, OxyColor color, string title) {
            var line = new LineSeries { Color = color, Title = title };

            foreach (var (x, y) in dotsList) {
                line.Points.Add(new DataPoint(x, y));
            }

            return line;
        }
    }
}
