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

public static class TriangleLagrangianQuadraticLocalMatrices
{
    private static double connection1(int[] nums, double[,] alphas)
    {
        double expr1 = 1d;
        double expr2 = 1d;
        foreach (var num in nums)
        {
            expr1 *= alphas[num, 1];
            expr2 *= alphas[num, 2];
        }
        return expr1 + expr2;
    }
    private static double connection2(int[] nums, double[,] alphas)
    {
        double expr1 = alphas[nums[0],1] * alphas[nums[0], 1] + alphas[nums[0], 1] * alphas[nums[1], 1] + alphas[nums[1], 1] * alphas[nums[1], 1];
        double expr2 = alphas[nums[0], 2] * alphas[nums[0], 2] + alphas[nums[0], 2] * alphas[nums[1], 2] + alphas[nums[1], 2] * alphas[nums[1], 2];

        return expr1 + expr2;
    }
    private static double connection3(int[] nums, double[,] alphas)
    {
        double expr1 = (alphas[nums[0],1] + alphas[nums[2], 1]) * (alphas[nums[2], 1] + alphas[nums[1], 1]) + alphas[nums[0],1] * alphas[nums[1], 1];
        double expr2 = (alphas[nums[0], 2] + alphas[nums[2], 2]) * (alphas[nums[2], 2] + alphas[nums[1], 2]) + alphas[nums[0], 2] * alphas[nums[1], 2];
        return expr1 + expr2;
    }
    public static double[][] GetStiffnessMatrix(double[,] alphas)
    {
        double[][] G = new double[6][];
        G[0] = [1d / 2d * connection1([0,0],alphas)];
        G[1] = [-1d / 6d * connection1([0,1],alphas),
                1d / 2d * connection1([1,1],alphas)];
        G[2] = [-1d / 6d * connection1([0,2],alphas),
                -1d / 6d * connection1([1,2],alphas),
                1d / 2d * connection1([2,2],alphas)];
        G[3] = [2d / 3d * connection1([0,1],alphas),
                2d / 3d * connection1([0,1],alphas),
                0d,
                4d/3d * connection2([0,1],alphas)];
        G[4] = [0d,
                2d / 3d * connection1([1,2],alphas),
                2d / 3d * connection1([1,2],alphas),
                2d / 3d * connection3([0,2,1],alphas),
                4d / 3d * connection2([1,2],alphas)];
        G[5] = [2d / 3d * connection1([0,2],alphas),
                0d,
                2d / 3d * connection1([0,2],alphas),
                2d / 3d * connection3([1,2,0],alphas),
                2d / 3d * connection3([0,1,2],alphas),
                4d / 3d * connection2([0,2],alphas)];
        return G;
    }
    public static double[][] GetMassMatrix()
    {
        double[] el = [0d, 1d / 60d, -1d / 360d, 4d / 45d, -1d / 90d, 2d / 45d];
        double[][] M = [[el[1]],
                        [el[2], el[1]],
                        [el[2], el[2], el[1]],
                        [el[0], el[0], el[4], el[3]],
                        [el[4], el[0], el[0], el[5], el[3]],
                        [el[0], el[4], el[0], el[5], el[5], el[3]]];
        
        return M;
    }
}
