using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using dnAnalytics;
using dnAnalytics.LinearAlgebra;
using dnAnalytics.LinearAlgebra.Decomposition;
using dnAnalytics.LinearAlgebra.Solvers.Direct;

namespace Flavor.Common.Library {
    /// <summary>
    ///      Proxy to dnAnalytics matrix equation library.
    /// </summary>
    class Matrix: DenseMatrix {
        private readonly Householder decomposition;
        public Matrix(double[,] array)
            : base(array) {
            decomposition = new Householder(this);
        }
        public Matrix(int order)
            : base(order) {
            decomposition = new Householder(this);
        }
        public void Init() {
            decomposition.Solve(new DenseVector(this.Columns, 0));
        }
        public double[] Solve(List<double> input) {
            if (this.Columns != input.Count) {
                // length mismatch
                // TODO: throw smth
            }
            // TODO: implement
            Vector output = decomposition.Solve(new DenseVector(input));
            return output.ToArray();
        }
    }
}
