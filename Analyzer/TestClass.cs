using dnAnalytics;
using dnAnalytics.LinearAlgebra;
using dnAnalytics.LinearAlgebra.Decomposition;
using dnAnalytics.LinearAlgebra.Solvers.Direct;

namespace Analyzer {
    class TestClass {
        private Matrix matrix = new DenseMatrix(3);
        private Vector vector = new DenseVector(3, 1);
        public void doWork() {
            matrix.SetDiagonal(vector);
            Householder decomposition = new Householder(matrix);
            if (!decomposition.IsFullRank())
                return;
            Vector result = decomposition.Solve(vector);// actually decompose here
            if (result != vector)
                throw new MatrixNotSquareException("HA-HA!");
        }
    }
}
