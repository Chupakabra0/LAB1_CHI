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
            this.Model = new PlotModel { Title = "YEAH!", Subtitle = "YES!", Series = { new AreaSeries() }};
        }

        public ICommand Magic => new RelayCommand( obj => {
            this.Model.Series.Add(this.CreateLine());
            this.Model.InvalidatePlot(true);
        });

        public  PlotModel Model          { get; set; }
        public  string    FunctionString { get; set; }
        private Function  Function       => new Function($"F(x, y) = {this.FunctionString}");

        private LineSeries CreateLine() {
            var result = new LineSeries();

            for (var x = 0.0; x < 10.0; x += 0.1) {
                for (var y = 0.0; y < 10.0; y += 0.1) {
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