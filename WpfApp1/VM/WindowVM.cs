using System;
using System.Globalization;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;
using WpfApp1.Basic;
using WpfApp1.Logic;

namespace WpfApp1.VM {
    public class WindowVM : BaseVM {
        public WindowVM() {
            CultureInfo.CurrentCulture = new CultureInfo("US-en");
            this.Model = new PlotModel { Title = "YEAH!", Subtitle = "YES!", Series = { new AreaSeries() } };
        }

        public ICommand Magic =>
            new RelayCommand(obj => {
                this.Model.Series.Add(this.CreateIntegralLine());
                this.Model.Series.Add(this.CreateFunctionLine());
                this.Model.InvalidatePlot(true);
            });

        public PlotModel Model          { get; set; }
        public string    FunctionString { get; set; }
        public string    HString        { get; set; }
        public string    LimitString    { get; set; }
        public string    X0String       { get; set; }
        public string    Y0String       { get; set; }

        private Function Function => new Function($"F(x, y) = {this.FunctionString}");

        private Func<double, double, double> Func =>
            (x, y) => new Expression($"F({x}, {y})", this.Function).calculate();

        private double H     => Convert.ToDouble(this.HString);
        private double Limit => Convert.ToDouble(this.LimitString);
        private double X0    => Convert.ToDouble(this.X0String);
        private double Y0    => Convert.ToDouble(this.Y0String);

        private LineSeries CreateIntegralLine() {
            var dotsList = new MethodRungeCutt(this.X0, this.Y0, this.Limit, this.H, this.Func).Execute();
            var line     = new LineSeries();

            foreach (var dot in dotsList) {
                line.Points.Add(dot);
            }

            line.Color = OxyColor.FromRgb(0, 0, 255);

            return line;
        }

        private LineSeries CreateFunctionLine() {
            var line = new LineSeries();

            for (var x = this.X0; x <= this.Limit; x += this.H) {
                for (var y = -50.0; y <= 50.0; y += this.H) {
                    var point = new DataPoint(x, new Expression($"F({x}, {y})", this.Function).calculate());
                    if (!line.Points.Contains(point)) {
                        line.Points.Add(point);
                    }
                }
            }

            line.Color = OxyColor.FromRgb(0, 255, 0);

            return line;
        }
    }
}