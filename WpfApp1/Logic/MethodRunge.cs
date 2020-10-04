using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace WpfApp1.Logic {
    public class MethodRungeCutt {
        public MethodRungeCutt(double                       x,
                               double                       y,
                               double                       a,
                               double                       b,
                               double                       h,
                               Func<double, double, double> function,
                               double                       epsilon) {
            this.Function = function;
            this.X        = x;
            this.A        = a;
            this.B        = b;
            this.Y        = y;
            this.H        = h;
            this.epsilon  = epsilon;

            this.CountFunc = (x, y, h) => y + 0.25 * (this.CountFi0(x, y, h) + 3 * this.CountFi2(x, y, h));
            this.CountFi0  = (x, y, h) => h * this.Function(x,                 y);
            this.CountFi1  = (x, y, h) => h * this.Function(x + h       / 3.0, y + this.CountFi0(x, y, h)       / 3.0);
            this.CountFi2  = (x, y, h) => h * this.Function(x + 2.0 * h / 3.0, y + 2.0 * this.CountFi1(x, y, h) / 3.0);

            //this.CountFunc = (x, y, h) => y + (this.CountFi0(x, y, h) + 2 * this.CountFi1(x, y, h) + 2 * this.CountFi2(x, y, h) + this.CountFi3(x, y, h)) / 6.0;
            //this.CountFi0 = (x, y, h)  => h * this.Function(x, y);
            //this.CountFi1 = (x, y, h)  => h * this.Function(x + h / 2.0, this.CountFi0(x, y, h) / 2.0);
            //this.CountFi2 = (x, y, h)  => h * this.Function(x + h / 2.0, this.CountFi1(x, y, h) / 2.0);
            //this.CountFi3 = (x, y, h)  => h * this.Function(x + h, this.CountFi2(x, y, h) + h);
        }

        public Func<double, double, double>         Function  { get; set; }
        public Func<double, double, double, double> CountFunc { get; }
        public Func<double, double, double, double> CountFi0  { get; }
        public Func<double, double, double, double> CountFi1  { get; }

        public Func<double, double, double, double> CountFi2 { get; }
        //public Func<double, double, double, double> CountFi3  { get; }

        public       double X       { get; set; }
        public       double Y       { get; set; }
        public       double H       { get; set; }
        public       double A       { get; set; }
        public       double B       { get; set; }
        public       double epsilon { get; set; }
        public const int    K        = 4;
        public const double MAX_STEP = 1.0;
        public const double MIN_STEP = 0.001;

        public List<List<double>> Execute() {
            var list = new List<List<double>> {
                new List<double>(), // x
                //new List<double>(), // y точное
                new List<double>(), // y приблизительное
                //new List<double>()  // погрешность
            };

            var currentX = this.A;
            var currentY = this.Y;

            var currentH = this.H;
            var halfH    = this.H / 2.0;

            list[0].Add(currentX);
            list[1].Add(currentY);

            do {
                // Делаем обычный шаг
                var stepNormH = this.CountFunc(currentX, currentY, currentH);
                // Делаем половинный шаг
                var stepHalfH = this.CountFunc(currentX + halfH, this.CountFunc(currentX, currentY, halfH), halfH);

                var epsilonNormH = Math.Abs((stepHalfH - stepNormH) * Math.Pow(2, MethodRungeCutt.K) /
                                            (Math.Pow(2, MethodRungeCutt.K) - 1));
                var epsilonHalfH = Math.Abs((stepHalfH - stepNormH) / (Math.Pow(2, MethodRungeCutt.K) - 1));

                if (epsilonHalfH > this.epsilon) {
                    currentH =  halfH;
                    halfH    /= 2.0;
                    continue;
                }

                currentX += currentH;
                currentY =  stepNormH;

                if (epsilonNormH <= this.epsilon) {
                    halfH    =  currentH;
                    currentH *= 2.0;
                }

                if (currentH > MethodRungeCutt.MAX_STEP) {
                    currentH = MethodRungeCutt.MAX_STEP;
                }
                else if (currentH < MethodRungeCutt.MIN_STEP) {
                    currentH = MethodRungeCutt.MIN_STEP;
                }

                list[0].Add(currentX);
                list[1].Add(currentY);
            } while (currentX < this.B);

            return list;
        }
    }
}

}