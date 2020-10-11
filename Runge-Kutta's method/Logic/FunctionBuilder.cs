using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;

namespace WpfApp1.Logic {
    public class FunctionBuilder {

        public FunctionBuilder(double x0, double xn, double y0, double yn, double h, string functionString) {
            this.X0             = x0;
            this.Xn             = xn;
            this.Y0             = y0;
            this.Yn             = yn;
            this.H              = h;
            this.FunctionString = functionString;
        }

        public double X0;
        public double Xn;
        public double Y0;
        public double Yn;
        public double H;
        public string FunctionString;

        public List<Tuple<double, double>> Build() {
            var list = new List<Tuple<double, double>>();

            Func<double, double, bool>   compareX;
            Func<double, double, double> nextX;

            if (this.X0 <= this.Xn) {
                compareX = (first, second) => first <= second;
                nextX    = (first, second) => first + second;
            }
            else {
                compareX = (first, second) => first >= second;
                nextX    = (first, second) => first - second;
            }

            Func<double, double, bool>   compareY;
            Func<double, double, double> nextY;

            if (this.Y0 <= this.Yn) {
                compareY = (first, second) => first <= second;
                nextY    = (first, second) => first + second;
            }
            else {
                compareY = (first, second) => first >= second;
                nextY    = (first, second) => first - second;
            }

            for (var x = this.X0; compareX(x, this.Xn); x = nextX(x, this.H)) {
                for (var y = this.Y0; compareY(y, this.Yn); y = nextY(y, this.H)) {
                    var point = new Tuple<double, double>(x, new Expression($"F({x}, {y})", this._Func).calculate());
                    if (!list.Contains(point)) {
                        list.Add(point);
                    }
                }
            }

            return list;
        }

        private Function _Func => new Function($"F(x, y) = {this.FunctionString}");
    }
}