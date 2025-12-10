using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices;

public class EdgeLagrangianLinearLocalMatrices
{
    public static double[][] GetMassMatrix()
    {
        double[][] M = [[2d], 
                        [1d, 2d]];
        for(int i = 0; i < 2; ++i) // /6
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] /= 6d;
        }
        return M;
    }
}

public class EdgeLagrangianQuadraticLocalMatrices
{
    public static double[][] GetMassMatrix()
    {
        double[][] M = [[4d], 
                        [2d, 16d],
                        [-1d, 2d, 4d]];
        for(int i = 0; i < 3; ++i) // /30
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] /= 30d;
        }
        return M;
    }
}
