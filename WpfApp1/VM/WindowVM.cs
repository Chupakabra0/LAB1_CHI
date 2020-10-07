using System;
using System.Collections.ObjectModel;
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
            this.Model = new PlotModel { Title = "Lab #1", Subtitle = "Runge-Kutta's method", Series = { new AreaSeries() } };
        }

        public ICommand Magic =>
            new RelayCommand(obj => {
                this.FunctionDots = CreateFunctionTuples();
                this.IntegralDots = CreateIntegralTuples();
                this.Model.Series.Add(this.CreateLine(this.FunctionDots, OxyColor.FromRgb(0, 255, 0), "Function"));
                this.Model.Series.Add(this.CreateLine(this.IntegralDots, OxyColor.FromRgb(0, 0, 255), "Integral"));
                this.Model.InvalidatePlot(true);
            });

        public PlotModel Model          { get; set; }
        public bool      IsAuto         => true;
        public string    FunctionString { get; set; }
        public string    HString        { get; set; }
        public string    LimitString    { get; set; }
        public string    X0String       { get; set; }
        public string    Y0String       { get; set; }

        public ObservableCollection<Tuple<double, double>> FunctionDots;
        public ObservableCollection<Tuple<double, double>> IntegralDots;

        private Function Function => new Function($"F(x, y) = {this.FunctionString}");

        private Func<double, double, double> Func =>
            (x, y) => new Expression($"F({x}, {y})", this.Function).calculate();

        private double H     => Convert.ToDouble(this.HString);
        private double Limit => Convert.ToDouble(this.LimitString);
        private double X0    => Convert.ToDouble(this.X0String);
        private double Y0    => Convert.ToDouble(this.Y0String);

        private ObservableCollection<Tuple<double, double>> CreateFunctionTuples() => new ObservableCollection<Tuple<double, double>>(new FunctionBuilder(this.X0, this.Limit, this.X0, this.Limit, this.H, this.FunctionString).Build());

        private ObservableCollection<Tuple<double, double>> CreateIntegralTuples() => new ObservableCollection<Tuple<double, double>>(new RK3(this.X0, this.Y0, this.Limit, this.H, this.Func).Calculate(this.IsAuto));

        private LineSeries CreateLine(ObservableCollection<Tuple<double, double>> dotsList, OxyColor color, string title) {
            var line     = new LineSeries { Color = color, Title = title };

            foreach (var (x, y) in dotsList) {
                line.Points.Add(new DataPoint(x, y));
            }

            return line;
        }
    }
}