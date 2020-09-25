using System;
using System.Windows;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApp1.Basic;
using WpfApp1.Logic;

namespace WpfApp1.VM {
    public class WindowVM : BaseVM {
        public WindowVM() {
            this.Model = new PlotModel { Title = "YEAH!", Subtitle = "YES!", Series = { new AreaSeries() }};
            var r = new MethodRungeCutt(0.0, 2.0, 0.5, (x, y) => 0.9 * x * y + 3.5 * y - 2.1);
            MessageBox.Show($"{r.ChooseEpsilon()}");
        }

        public ICommand Magic => new RelayCommand(obj => {
            this.Function = v => Math.Sin(v) * 3.0 - 1.0;
            this.Model.Series.Add(new FunctionSeries(this.Function, 0.0, 5.0, 0.01, "It's perfect!"));
            this.Model.InvalidatePlot(true);
        });

        public PlotModel            Model { get; set; }
        public Func<double, double> Function;
    }
}