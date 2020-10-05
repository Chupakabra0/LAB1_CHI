using System;
using System.Collections.Generic;
using OxyPlot;

namespace WpfApp1.Logic {
    public class MethodRungeCutt {
        public MethodRungeCutt(double                       x,
                               double                       y,
                               double                       limit,
                               double                       h,
                               Func<double, double, double> function) {
            this.Function = function;
            this.X        = x;
            this.Limit    = limit;
            this.Y        = y;
            this.H        = h;

            this.CountFunc = (x, y, h) => y + 0.25 * (this.CountFi0(x, y, h) + 3 * this.CountFi2(x, y, h));
            this.CountFi0  = (x, y, h) => h * this.Function(x,                 y);
            this.CountFi1  = (x, y, h) => h * this.Function(x + h       / 3.0, y + this.CountFi0(x, y, h)       / 3.0);
            this.CountFi2  = (x, y, h) => h * this.Function(x + 2.0 * h / 3.0, y + 2.0 * this.CountFi1(x, y, h) / 3.0);
        }

        public Func<double, double, double>         Function  { get; set; }
        public Func<double, double, double, double> CountFunc { get; }
        public Func<double, double, double, double> CountFi0  { get; }
        public Func<double, double, double, double> CountFi1  { get; }
        public Func<double, double, double, double> CountFi2  { get; }

        public       double X     { get; set; }
        public       double Y     { get; set; }
        public       double H     { get; set; }
        public       double Limit { get; set; }
        public const int    K        = 3;
        public const double epsilon  = 0.001;
        public const double MAX_STEP = 1.0;
        public const double MIN_STEP = 0.01;

        public List<DataPoint> Execute(bool isAuto = false) {
            var list = new List<DataPoint>();

            var currentX = this.X;
            var currentY = this.Y;

            var currentH = this.H;
            var halfH    = this.H / 2.0;

            list.Add(new DataPoint(currentX, currentY));

            while (this.Limit - currentX >= Math.Pow(10.0, -6.0)) {
                // Делаем обычный шаг
                var stepNormH = this.CountFunc(currentX, currentY, currentH);
                // Делаем половинный шаг
                var stepHalfH = this.CountFunc(currentX + halfH, this.CountFunc(currentX, currentY, halfH), halfH);

                var epsilonNormH = Math.Abs((stepHalfH - stepNormH) * Math.Pow(2, MethodRungeCutt.K) /
                                            (Math.Pow(2, MethodRungeCutt.K) - 1));
                var epsilonHalfH = Math.Abs((stepHalfH - stepNormH) / (Math.Pow(2, MethodRungeCutt.K) - 1));

                if (isAuto) {
                    if (epsilonHalfH > MethodRungeCutt.epsilon) {
                        currentH =  halfH;
                        halfH    /= 2.0;
                        continue;
                    }
                }

                currentX += currentH;
                currentY =  stepHalfH;

                list.Add(new DataPoint(currentX, currentY));

                if (isAuto) {
                    if (epsilonNormH <= MethodRungeCutt.epsilon) {
                        halfH    =  currentH;
                        currentH *= 2.0;
                    }

                    if (currentH > MethodRungeCutt.MAX_STEP) {
                        currentH = MethodRungeCutt.MAX_STEP;
                    }
                    else if (currentH < MethodRungeCutt.MIN_STEP) {
                        currentH = MethodRungeCutt.MIN_STEP;
                    }
                }

                if (this.Limit - currentX < currentH) {
                    currentH = this.Limit - currentX;
                }
            }

            return list;
        }
    }
}