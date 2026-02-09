using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Lagrangian;
public class TriangleLagrangianLocalMatrices
{
    private static double[][] G1(double[,] alphas, double detD, double Coefficient)
    {
        double[][] G = [new double[1],
                        new double[2],
                        new double[3]];
        for(int i = 0; i < G.GetLength(0); ++i)
        {
            for(int j = 0; j <= i; ++j)
                G[i][j] = (alphas[i,1] * alphas[j,1] + alphas[i,2] * alphas[j,2]) * detD * Coefficient/2d;
        }
        return G;
    }

    private static double[][] M1(double[,] alphas, double detD, double Coefficient)
    {
        double[][] M = [[2d],
                        [1d, 2d],
                        [1d, 1d, 2d]];
                        
        for(int i = 0; i < M.GetLength(0); ++i)
        {
            for(int j = 0; j <= i; ++j)
                M[i][j] *= Coefficient * detD / 24d;
        }
        return M;
    }

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

    private static double[][] G2(double[,] alphas, double detD, double Coefficient)
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

        for(int i = 0; i < G.GetLength(0); ++i)
        {
            for(int j = 0; j <= i; ++j)
                G[i][j] *= Coefficient * detD;
        }
        return G;
    }

    private static double[][] M2(double[,] alphas, double detD, double Coefficient)
    {
        double[] el = [0d, 1d / 60d, -1d / 360d, 4d / 45d, -1d / 90d, 2d / 45d];
        double[][] M = [[el[1]],
                        [el[2], el[1]],
                        [el[2], el[2], el[1]],
                        [el[0], el[0], el[4], el[3]],
                        [el[4], el[0], el[0], el[5], el[3]],
                        [el[0], el[4], el[0], el[5], el[5], el[3]]];

        for(int i = 0; i < M.GetLength(0); ++i)
        {
            for(int j = 0; j <= i; ++j)
                M[i][j] *= Coefficient * detD;
        }
        return M;
    }

    private static double[][] M3(double[,] alphas, double detD, double Coefficient)
    {
        double[] el = [19d/3360d, 11d/13440d, 3d/2240d, 0d, 9d/4480d, 9d/224d, -9d/640d,-9d/896d,9d/448d,-9d/2240d, 3d/1120d, 27d/2240d, 81d/560d];
        int[][] indices = [[0],
                           [1, 0],
                           [1, 1, 0],
                           [2, 3, 4, 5],
                           [3, 2, 4, 6, 5],
                           [4, 2, 3, 7, 8, 5],
                           [4, 3, 2, 9, 7, 6, 5],
                           [3, 4, 2, 7, 9, 7, 8, 5],
                           [2, 4, 3, 8, 7, 9, 7, 6, 5],
                           [10, 10, 10, 11, 11, 11, 11, 11, 11, 12]
                           ];
        double[][] M = new double[indices.GetLength(0)][];
        
        for(int i = 0; i < M.GetLength(0); ++i)
            M[i] = indices[i].Select(i => el[i] * Coefficient * detD).ToArray();
        return M;
    }
}