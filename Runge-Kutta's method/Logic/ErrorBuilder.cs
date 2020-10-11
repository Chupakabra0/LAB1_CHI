using System;
using System.Collections.Generic;

namespace WpfApp1.Logic
{
    public class ErrorBuilder {
        public ErrorBuilder(List<Tuple<double, double>> firstFunction, List<Tuple<double, double>> secondFunction) {
            this.firstFunction  = firstFunction;
            this.secondFunction = secondFunction;
        }

        public List<Tuple<double, double>> Build() {
            var list = new List<Tuple<double, double>>();

            var firstIndex     = 0;
            var secondIndex    = 0;
            var isFirstGreater = this.firstFunction.Count < this.secondFunction.Count;

            var limit       = isFirstGreater ? this.firstFunction.Count : this.secondFunction.Count;

            while (isFirstGreater ? firstIndex < limit : secondIndex < limit) {
                if (Math.Abs(this.firstFunction[firstIndex].Item1 - this.secondFunction[secondIndex].Item1) < 0.000001) {
                    list.Add(new Tuple<double, double>(this.firstFunction[firstIndex].Item1, this.firstFunction[firstIndex++].Item2 - this.secondFunction[secondIndex++].Item2));
                }
                else if (this.firstFunction[firstIndex].Item1 < this.secondFunction[secondIndex].Item1) {
                    ++firstIndex;
                }
                else {
                    ++secondIndex;
                }
            }

            return list;
        }

        public List<Tuple<double, double>> firstFunction  { get; set; }
        public List<Tuple<double, double>> secondFunction { get; set; }
    }
}
