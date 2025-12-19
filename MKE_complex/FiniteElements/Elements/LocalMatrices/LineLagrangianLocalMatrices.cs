using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices;

public static class LineLagrangianLocalMatrices
{
    public static class QuadraticMatrices
    {
        public static double[][] GetStiffnessMatrix(double h)
        {
            double[][] G = [[7d],
                            [-8d, 16d],
                            [1d, -8d, 7d]];
            for(int i = 0; i < G.GetLength(0); ++i) // /(3*h)
            {
                for (int j = 0; j <= i; ++j)
                    G[i][j] /= 3d * h;
            }
            return G;
        }
        public static double[][] GetMassMatrix(double h)
        {
            double[][] M = [[4d],
                            [2d, 16d],
                            [-1d, 2d, 4d]];
            for (int i = 0; i < M.GetLength(0); ++i) // *h/30
            {
                for (int j = 0; j <= i; ++j)
                    M[i][j] = M[i][j] * h / 30d;
            }
            return M;
        }
    }
}
