using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using dnAnalytics;
using dnAnalytics.LinearAlgebra;
using dnAnalytics.LinearAlgebra.Decomposition;
using dnAnalytics.LinearAlgebra.Solvers.Direct;

namespace Flavor.Common.Library {
    class Matrix: DenseMatrix {
        public Matrix(double[,] array)
            : base(array) {
        }
        public Matrix(int order)
            : base (order) {
        }
    }
}
