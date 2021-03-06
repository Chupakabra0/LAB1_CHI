﻿using System;
using System.Collections.Generic;

namespace RKMApp.Logic {
    public class RK4 {
        public RK4(double x, double y, double limit, double h, Func<double, double, double> function) {
            this.Function = function;
            this.X        = x;
            this.Limit    = limit;
            this.Y        = y;
            this.H        = h;

            if (this.Limit >= this.X) {
                this._NextStep = (val,   h) => val   + h;
                this._Delta    = (limit, x) => limit - x;
            }
            else {
                this._NextStep = (val,   h) => val - h;
                this._Delta    = (limit, x) => x   - limit;
            }

            this.CountFunc = (x, y, h) => this._NextStep(y, h * (this.CountK1(x, y, h) + 2.0 * this.CountK2(x, y, h) + 2.0 * this.CountK3(x, y, h) + this.CountK4(x, y, h)) / 6.0);
            this.CountK1 = (x, y, h) => this.Function(x, y);
            this.CountK2 = (x, y, h) => this.Function(this._NextStep(x, 0.5 * h), this._NextStep(y, 0.5 * h * this.CountK1(x, y, h)));
            this.CountK3 = (x, y, h) => this.Function(this._NextStep(x, 0.5 * h), this._NextStep(y, 0.5 * h * this.CountK2(x, y, h)));
            this.CountK4 = (x, y, h) => this.Function(this._NextStep(x, h), this._NextStep(y, h * this.CountK3(x, y, h)));
        }

        public Func<double, double, double>         Function  { get; set; }
        public Func<double, double, double, double> CountFunc { get; }
        public Func<double, double, double, double> CountK1   { get; }
        public Func<double, double, double, double> CountK2   { get; }
        public Func<double, double, double, double> CountK3   { get; }
        public Func<double, double, double, double> CountK4   { get; }

        public       double X     { get; set; }
        public       double Y     { get; set; }
        public       double H     { get; set; }
        public       double Limit { get; set; }
        public const int    K        = 4;
        public const double MAX_STEP = 1000.0;
        public const double MIN_STEP = 0.0001;

        public List<Tuple<double, double>> Calculate(bool isAuto = false) {
            var list = new List<Tuple<double, double>>();

            var currentX = this.X;
            var currentY = this.Y;

            var currentH = this.H;
            var halfH    = this.H / 2.0;

            list.Add(new Tuple<double, double>(currentX, currentY));

            while (this._Delta(this.Limit, currentX) >= Math.Pow(10.0, -6.0)) {
                if (this._Delta(this.Limit, currentX) < currentH) {
                    currentH = this._Delta(this.Limit, currentX);
                }

                // Делаем обычный шаг
                var stepNormH = this.CountFunc(currentX, currentY, currentH);
                // Делаем половинный шаг
                var stepHalfH = this.CountFunc(this._NextStep(currentX, halfH),
                                               this.CountFunc(currentX, currentY, halfH), halfH);

                var epsilonNormH = Math.Abs((stepHalfH - stepNormH) * Math.Pow(2, RK4.K) /
                                            (Math.Pow(2, RK4.K) - 1));
                var epsilonHalfH = Math.Abs((stepHalfH - stepNormH) / (Math.Pow(2, RK4.K) - 1));

                if (isAuto) {
                    if (epsilonHalfH > Math.Pow(10.0, -RK4.K)) {
                        currentH =  halfH;
                        halfH    /= 2.0;
                        continue;
                    }
                }

                currentX = this._NextStep(currentX, currentH);
                currentY = stepNormH;

                list.Add(new Tuple<double, double>(currentX, currentY));

                if (isAuto) {
                    if (epsilonNormH <= Math.Pow(10.0, -RK4.K)) {
                        halfH    =  currentH;
                        currentH *= 2.0;
                    }

                    if (currentH > RK4.MAX_STEP) {
                        currentH = RK4.MAX_STEP;
                    }
                    else if (currentH < RK4.MIN_STEP) {
                        currentH = RK4.MIN_STEP;
                    }
                }
            }

            return list;
        }

        private Func<double, double, double> _NextStep;
        private Func<double, double, double> _Delta;
    }
}