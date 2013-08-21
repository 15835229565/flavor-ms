using System;
using Meta.Numerics.Matrices;

namespace Analyzer {
    class TestClass2 {
        static double[,] array =
        {{0, 0, 19000, 75000, 22500, 88000, 0, 0},
        {100, 2400, 0, 0, 0, 0, 0, 120},
        {0, 0, 0, 0, 0, 0, 11500, 0},
        {0, 0, 47500, 0, 12800, 0, 0, 0},
        {0, 0, 72500, 0, 197000, 19500, 0, 0},
        {0, 0, 5150, 25, 1200, 7100, 0, 20000},
        {890, 0, 780, 0, 1000, 640, 0, 180},
        {0, 0, 72000, 0, 98000, 64400, 0, 0}};
        private SquareMatrix matrix = new SquareMatrix(8);
        static double[] array2 = { 1300, 1575, 5068, 27000, 57800, 4380, 227261, 15367, };
        private ColumnVector vector = new ColumnVector(array2);
        public void doWork() {
            matrix.Fill((j, i) => array[i, j]);
            SquareQRDecomposition qr = matrix.QRDecomposition();
            SquareLUDecomposition lu = matrix.LUDecomposition();

            ColumnVector qrresult = qr.Solve(vector);
            ColumnVector luresult = lu.Solve(vector);

            ColumnVector c6 = matrix.Column(6);
            RowVector r2 = matrix.Row(2);
            SquareMatrix inv = matrix.Inverse();

            ColumnVector result = new ColumnVector(inv.Dimension);
            for (int i = 0; i < matrix.Dimension; ++i) {
                for (int j = 0; j < matrix.Dimension; ++j) {
                    result[i] += matrix[i, j] * qrresult[j];
                }
            }
        }
    }
}
