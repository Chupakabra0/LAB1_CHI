using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace WpfApp1.Logic {
    public class MethodRungeCutt {

        public MethodRungeCutt(double x, double y, double h, Func<double, double, double> function, double epsilon = 0.001) {
            this.Function = function;
            this.x = x;
            this.y = y;
            this.h = h;
            this.epsilon = epsilon;

            this.CountFunc = (x, y, h) => y + 0.25 * (this.CountFi0(x, y, h) + 3 * this.CountFi2(x, y, h));
            this.CountFi0 = (x, y, h) => h * this.Function(x, y);
            this.CountFi1 = (x, y, h) => h * this.Function(x + h / 3.0, y + this.CountFi0(x, y, h) / 3.0);
            this.CountFi2 = (x, y, h) => h * this.Function(x + 2.0 * h / 3.0, y + 2.0 * this.CountFi1(x, y, h) / 3.0);
        }

        public Func<double, double, double> Function { get; set; }
        public Func<double, double, double, double> CountFunc { get; }
        public Func<double, double, double, double> CountFi0 { get; }
        public Func<double, double, double, double> CountFi1 { get; }
        public Func<double, double, double, double> CountFi2 { get; }

        public double x { get; set; }
        public double y { get; set; }
        public double h { get; set; }
        public const int k = 3;
        public double epsilon { get; set; }

        public double ChooseEpsilon() {
            var epsilonOne = 0.0;
            var epsilonHalf = 0.0;
            var h = this.h * 2.0;
            do {
                h /= 2.0;
                var yOne = this.Function(this.x + h, this.y);
                var tempX = this.x + 0.5 * h;
                var yHalf = this.Function(tempX + 0.5 * h, this.y);
                epsilonHalf = (yHalf - yOne) * Math.Pow(2.0, k) / (Math.Pow(2, k) - 1.0);
                epsilonOne = (yHalf - yOne) / (Math.Pow(2, k) - 1.0);
            } while (Math.Abs(epsilonHalf) > epsilonOne);

            return epsilonHalf;
        }
    }
}
