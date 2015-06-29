using System;
using System.Collections.Generic;
using dnAnalytics.LinearAlgebra;
using dnAnalytics.LinearAlgebra.Decomposition;

namespace Flavor.Common.Library {
    /// <summary>
    ///      Proxy to dnAnalytics matrix equation library.
    /// </summary>
    class Matrix: DenseMatrix {
        readonly LU decomposition;
        public Matrix(double[,] array)
            : base(array) {
            decomposition = new LU(this);
        }
        public void Init() {
            decomposition.Solve(new DenseVector(Columns, 0));
        }
        public double[] Solve(List<double> input) {
            if (Columns != input.Count) {
                // length mismatch
                throw new RankException(string.Format("Matrix rank is {0}, input vector length is {1}", Columns, input.Count));
            }
            return decomposition.Solve(new DenseVector(input)).ToArray();
        }
    }
}
