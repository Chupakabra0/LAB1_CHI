using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;
using WpfApp1.Basic;
using WpfApp1.Logic;

using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace WpfApp1.VM {
    public class WindowVM : BaseVM {
        public WindowVM() => this.Model = new PlotModel { Title = "Lab #1", Subtitle = "Runge-Kutta's method", Series = { new AreaSeries() } };

        public ICommand Magic =>
            new RelayCommand(obj => {
                this.IntegralDots = CreateIntegralTuples();
                this.ExactDots    = CreateExactTuples();
                this.ErrorDots    = CreateErrorTuples();
                this.Model.Series.Clear();
                //this.Model.Series.Add(this.CreateLine(this.ErrorDots, OxyColor.FromRgb(0, 255, 0), "Function"));
                this.Model.Series.Add(this.CreateLine(this.IntegralDots, OxyColor.FromRgb(0, 0, 255), "Integral"));
                this.Model.Series.Add(this.CreateLine(this.ExactDots, OxyColor.FromRgb(255, 0, 0), "Exact"));
                this.Model.InvalidatePlot(true);
                this.TableVisibility = Visibility.Visible;
            });

        public PlotModel  Model           { get; set; }
        public Visibility TableVisibility { get; set; } = Visibility.Collapsed;
        public bool       IsAuto          { get; set; }
        public string     FunctionString  { get; set; }
        public string     HString         { get; set; }
        public string     LimitString     { get; set; }
        public string     X0String        { get; set; }
        public string     Y0String        { get; set; }

        public ObservableCollection<Tuple<double, double>> ErrorDots { get; set; }
        public ObservableCollection<Tuple<double, double>> IntegralDots { get; set; }
        public ObservableCollection<Tuple<double, double>> ExactDots    { get; set; }

        private Function Function => new Function($"F(x, y) = {this.FunctionString}");

        private Func<double, double, double> Func => (x, y) => new Expression($"F({x.ToString(CultureInfo.InvariantCulture)}, {y.ToString(CultureInfo.InvariantCulture)})", this.Function).calculate();

        private double H     => Convert.ToDouble(this.HString,     CultureInfo.InvariantCulture);
        private double Limit => Convert.ToDouble(this.LimitString, CultureInfo.InvariantCulture);
        private double X0    => Convert.ToDouble(this.X0String,    CultureInfo.InvariantCulture);
        private double Y0    => Convert.ToDouble(this.Y0String,    CultureInfo.InvariantCulture);

        private ObservableCollection<Tuple<double, double>> CreateErrorTuples() => new ObservableCollection<Tuple<double, double>>(new ErrorBuilder(this.IntegralDots.ToList(), this.ExactDots.ToList()).Build());

        private ObservableCollection<Tuple<double, double>> CreateIntegralTuples() => new ObservableCollection<Tuple<double, double>>(new RK3(this.X0, this.Y0, this.Limit, this.H, this.Func).Calculate(this.IsAuto));
        
        private ObservableCollection<Tuple<double, double>> CreateExactTuples() => new ObservableCollection<Tuple<double, double>>(new RK4(this.X0, this.Y0, this.Limit, this.H, this.Func).Calculate(this.IsAuto));

        private LineSeries CreateLine(IEnumerable<Tuple<double, double>> dotsList, OxyColor color, string title) {
            var line     = new LineSeries { Color = color, Title = title };

            foreach (var (x, y) in dotsList) {
                line.Points.Add(new DataPoint(x, y));
            }

            return line;
        }
    }
}