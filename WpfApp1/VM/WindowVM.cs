using System;
using System.Globalization;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;
using WpfApp1.Basic;

namespace WpfApp1.VM {
    public class WindowVM : BaseVM {
        public WindowVM() {
            CultureInfo.CurrentCulture = new CultureInfo("US-en");
            var series = new AreaSeries();
            this.Model = new PlotModel { Title = "YEAH!", Subtitle = "YES!", Series = {series} };
        }

        public ICommand Magic => new RelayCommand( obj => {
            this.Model.Series.Add(this.CreateLine());
            this.Model.InvalidatePlot(true);
        });

        public PlotModel Model          { get; set; }
        public string    FunctionString { get; set; }
        public string    AString        { get; set; }
        public string    BString        { get; set; }
        public string    X0String       { get; set; }
        public string    Y0String       { get; set; }

        private Function Function => new Function($"F(x, y) = {this.FunctionString}");
        private double   A        => Convert.ToDouble(this.AString);
        private double   B        => Convert.ToDouble(this.BString);
        private double   X0       => Convert.ToDouble(this.X0String);
        private double   Y0       => Convert.ToDouble(this.Y0String);

        private LineSeries CreateLine() {
            var result = new LineSeries();

            for (var x = this.A; x <= this.B; x += 0.05) {
                for (var y = this.A; y <= this.B; y += 0.05) {
                    var expression = new Expression($"F({x}, {y})", this.Function);
                    var point      = new DataPoint(x, expression.calculate());
                    if (!result.Points.Contains(point)) {
                        result.Points.Add(point);
                    }
                }
            }

            return result;
        }
    }
}