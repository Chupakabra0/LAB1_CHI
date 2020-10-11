using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Series;
using RKMApp.Basic;
using RKMApp.Logic;

using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace RKMApp.VM {
    public class WindowVM : BaseVM {
        public WindowVM() => this.MagicAsyncCommand = new AsyncCommand(async () => {
            try {
                this.IsMagicBusy = true;
                await Task.Run(() => {
                    var temp = this.ButtonString;
                    try {
                        this.ButtonString = "Checking input data";
                        if (CheckSyntax.IsDouble(this.FunctionString) || !this.Function.checkSyntax()) throw new Exception("F(x, y) syntax fail.");
                        if (CheckSyntax.IsDouble(this.HString)) throw new Exception("h syntax fail");
                        if (CheckSyntax.IsDouble(this.X0String)) throw new Exception("X0 syntax fail");
                        if (CheckSyntax.IsDouble(this.XnString)) throw new Exception("Xn syntax fail");
                        if (CheckSyntax.IsDouble(this.Y0String)) throw new Exception("Y0 syntax fail");
                        if (this.H == 0.0) throw new Exception("h can't be equal to 0.");
                        this.ButtonString = "Counting solutions";
                        this.IntegralDots = CreateIntegralTuples();
                        this.ExactDots    = CreateExactTuples();
                        this.ButtonString = "Counting error";
                        this.ErrorDots    = CreateErrorTuples();
                        this.ButtonString = "Building graphic";
                        this.Model.Series.Clear();
                        this.Model.Series.Add(LineCreator.CreateLine(this.IntegralDots, OxyColor.FromRgb(0, 0, 255), "RK3"));
                        this.Model.Series.Add(LineCreator.CreateLine(this.ExactDots, OxyColor.FromRgb(255, 0, 0), "RK4"));
                        this.Model.InvalidatePlot(true);
                        this.TableVisibility = Visibility.Visible;
                    }
                    catch (Exception exception) {
                        MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                    finally {
                        this.ButtonString = temp;
                    }
                });
            }
            finally {
                this.IsMagicBusy = false;
            }
        },  () => !this.IsMagicBusy);

        public PlotModel  Model           { get; set; } = new PlotModel { Title = "Lab #1", Subtitle = "Runge-Kutta's method", Series = { new AreaSeries() } };
        public Visibility TableVisibility { get; set; } = Visibility.Collapsed;
        public bool       IsAuto          { get; set; } = false;
        public string     FunctionString  { get; set; } = string.Empty;
        public string     HString         { get; set; } = string.Empty;
        public string     XnString        { get; set; } = string.Empty;
        public string     X0String        { get; set; } = string.Empty;
        public string     Y0String        { get; set; } = string.Empty;

        public string     ButtonString    { get; set; } = "Press it to magic";

        public ObservableCollection<Tuple<double, double>> ErrorDots    { get; set; }
        public ObservableCollection<Tuple<double, double>> IntegralDots { get; set; }
        public ObservableCollection<Tuple<double, double>> ExactDots    { get; set; }

        // ReSharper disable once RedundantDefaultMemberInitializer
        public bool IsMagicBusy { get; private set; } = false;

        public IAsyncCommand MagicAsyncCommand { get; }

        private Function Function => new Function($"F(x, y) = {this.FunctionString}");

        private Func<double, double, double> Func => (x, y) => new Expression($"F({x.ToString(CultureInfo.InvariantCulture)}, {y.ToString(CultureInfo.InvariantCulture)})", this.Function).calculate();

        private double H   => Convert.ToDouble(this.HString,  CultureInfo.InvariantCulture);
        private double Xn  => Convert.ToDouble(this.XnString, CultureInfo.InvariantCulture);
        private double X0  => Convert.ToDouble(this.X0String, CultureInfo.InvariantCulture);
        private double Y0  => Convert.ToDouble(this.Y0String, CultureInfo.InvariantCulture);

        private ObservableCollection<Tuple<double, double>> CreateErrorTuples()    => new ObservableCollection<Tuple<double, double>>(new ErrorBuilder(this.IntegralDots.ToList(), this.ExactDots.ToList()).Build());

        private ObservableCollection<Tuple<double, double>> CreateIntegralTuples() => new ObservableCollection<Tuple<double, double>>(new RK3(this.X0, this.Y0, this.Xn, this.H, this.Func).Calculate(this.IsAuto));
        
        private ObservableCollection<Tuple<double, double>> CreateExactTuples()    => new ObservableCollection<Tuple<double, double>>(new RK4(this.X0, this.Y0, this.Xn, this.H, this.Func).Calculate(this.IsAuto));
    }
}