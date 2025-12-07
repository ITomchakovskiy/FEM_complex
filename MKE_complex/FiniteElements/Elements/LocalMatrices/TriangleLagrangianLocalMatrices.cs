using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices;

public static class TriangleLagrangianLinearLocalMatrices
{
    public static double[][] GetStiffnessMatrix(double[,] alphas)
    {
        double[][] G = [[0], [0, 0], [0, 0, 0]];
        for(int i = 0; i < 3; ++i)
        {
            for (int j = 0; j <= i; ++j)
                G[i][j] = (alphas[i, 1] * alphas[j, 1] + alphas[i, 2] * alphas[j, 2]) / 2d;
        }
        return G;
    }
    public static double[][] GetMassMatrix()
    {
        double[][] M = [[2d], 
                        [1d, 2d], 
                        [1d, 1d, 2d]];
        for(int i = 0; i < 3; ++i) // /24
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] /= 24d;
        }
        return M;
    }
}
