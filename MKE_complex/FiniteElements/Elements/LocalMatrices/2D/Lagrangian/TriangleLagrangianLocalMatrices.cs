using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Lagrangian;
public class TriangleLagrangianLocalMatrices
{

    public static double[][] CalculateLocalStiffnessMatrix(int order, double[,] alphas, double detD, double Coefficient)
    {
        switch(order)
        {
            case 1: return G1(alphas, detD, Coefficient);
            case 2: return G2(alphas, detD, Coefficient);
            case 3: return G3(alphas, detD, Coefficient);
            default: throw new ArgumentException("wrong element order");
        }
    }
    public static double[][] CalculateLocalMassMatrix(int order, double[,] alphas, double detD, double Coefficient)
    {
        switch(order)
        {
            case 1: return M1(alphas, detD, Coefficient);
            case 2: return M2(alphas, detD, Coefficient);
            case 3: return M3(alphas, detD, Coefficient);
            default: throw new ArgumentException("wrong element order");
        }
    }
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

    private static double connection21(int[] nums, double[,] alphas)
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
    private static double connection22(int[] nums, double[,] alphas)
    {
        double expr1 = alphas[nums[0],1] * alphas[nums[0], 1] + alphas[nums[0], 1] * alphas[nums[1], 1] + alphas[nums[1], 1] * alphas[nums[1], 1];
        double expr2 = alphas[nums[0], 2] * alphas[nums[0], 2] + alphas[nums[0], 2] * alphas[nums[1], 2] + alphas[nums[1], 2] * alphas[nums[1], 2];

        return expr1 + expr2;
     }
    private static double connection23(int[] nums, double[,] alphas)
    {
        double expr1 = (alphas[nums[0],1] + alphas[nums[2], 1]) * (alphas[nums[2], 1] + alphas[nums[1], 1]) + alphas[nums[0],1] * alphas[nums[1], 1];
        double expr2 = (alphas[nums[0], 2] + alphas[nums[2], 2]) * (alphas[nums[2], 2] + alphas[nums[1], 2]) + alphas[nums[0], 2] * alphas[nums[1], 2];
        return expr1 + expr2;
    }

    private static double[][] G2(double[,] alphas, double detD, double Coefficient)
    {
        double[][] G =
        [
            [1d / 2d * connection21([0,0],alphas)],
            [-1d / 6d * connection21([0,1],alphas),
                    1d / 2d * connection21([1,1],alphas)],
            [-1d / 6d * connection21([0,2],alphas),
                    -1d / 6d * connection21([1,2],alphas),
                    1d / 2d * connection21([2,2],alphas)],
            [2d / 3d * connection21([0,1],alphas),
                    2d / 3d * connection21([0,1],alphas),
                    0d,
                    4d/3d * connection22([0,1],alphas)],
            [0d,
                    2d / 3d * connection21([1,2],alphas),
                    2d / 3d * connection21([1,2],alphas),
                    2d / 3d * connection23([0,2,1],alphas),
                    4d / 3d * connection22([1,2],alphas)],
            [2d / 3d * connection21([0,2],alphas),
                    0d,
                    2d / 3d * connection21([0,2],alphas),
                    2d / 3d * connection23([1,2,0],alphas),
                    2d / 3d * connection23([0,1,2],alphas),
                    4d / 3d * connection22([0,2],alphas)],
        ];
        for (int i = 0; i < G.GetLength(0); ++i)
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

    private static double connection31(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 1) throw new ArgumentException();
        int num = nums[0];

        double expr1 = alphas[num,1] * alphas[num,1];
        double expr2 = alphas[num,2] * alphas[num,2];

        return expr1 + expr2;
    }

    private static double connection32(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 2) throw new ArgumentException();
        
        double expr1 = alphas[nums[0],1] * alphas[nums[1],1];
        double expr2 = alphas[nums[0],2] * alphas[nums[1],2];

        return expr1 + expr2;
    }

    private static double connection33(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 2) throw new ArgumentException();
        
        double expr1 = connection31([nums[0]], alphas);
        double expr2 = 19d * connection32(nums, alphas);
        
        return expr1 + expr2;
    }

    private static double connection34(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 2) throw new ArgumentException();
        
        double expr1 = connection31([nums[0]], alphas);
        double expr2 = -8d * connection32(nums, alphas);
        
        return expr1 + expr2;
    }

    private static double connection35(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 3) throw new ArgumentException();
        
        double expr1 = connection32([nums[0], nums[1]], alphas);
        double expr2 = connection32([nums[0], nums[2]], alphas);

        return expr1 + expr2;
    }

    private static double connection36(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 2) throw new ArgumentException();
        
        double expr1 = connection31([nums[0]],alphas);
        double expr2 = connection31([nums[1]],alphas);
        double expr3 = connection32(nums,alphas);

        return expr1 + expr2 + expr3;
    }

    private static double connection37(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 2) throw new ArgumentException();
        
        double expr1 = alphas[nums[0], 1] - alphas[nums[1], 1];
        double expr2 = expr1 * expr1;
        double expr3 = alphas[nums[0], 2] - alphas[nums[1], 2];
        double expr4 = expr3 * expr3;

        return expr2 + expr4;
    }

    private static double connection38(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 3) throw new ArgumentException();
        
        double expr1 = connection31([nums[0]],alphas);
        double expr2 = 2d * connection32([nums[1], nums[2]],alphas);
        double expr3 = connection32([nums[0], nums[1]],alphas);
        double expr4 = connection32([nums[0], nums[2]],alphas);

        return expr1 + expr2 + expr3 + expr4;
    }

    private static double connection39(int[] nums, double[,] alphas)
    {
        if(nums.Count() != 1) throw new ArgumentException();
        
        double expr1 = alphas[nums[0],1] * (alphas[0,1] + alphas[1,1] + alphas[2,1]);
        double expr2 = alphas[nums[0],2] * (alphas[0,2] + alphas[1,2] + alphas[2,2]);
        return expr1 + expr2;
    }

    private static double connection310(double[,] alphas)
    {
        double expr1 = connection31([0],alphas) +
                       connection31([1],alphas) +
                       connection31([2],alphas);
        double expr2 = connection32([0, 1], alphas) +
                       connection32([1, 2], alphas) +
                       connection32([0, 2], alphas);
        return expr1 + expr2;
    }

    private static double[][] G3(double[,] alphas, double detD, double Coefficient)
    {
        double[][] G =
        [
            [17d/40d * connection31([0], alphas)],
            [
                7d/80d * connection32([0,1],alphas),
                17d/40d * connection31([1], alphas)
            ],
            [
                7d/80d * connection32([0,2],alphas),
                7d/80d * connection32([1,2],alphas),
                17d/40d * connection31([2], alphas)
            ],
            [
                3d/80d * connection33([0,1],alphas),
                3d/80d * connection34([1,0],alphas),
                3d/80d * connection35([2,0,1],alphas),
                27d/16d * connection36([0,1],alphas)
            ],
            [
                3d/80d * connection34([0,1],alphas),
                3d/80d * connection33([1,0],alphas),
                3d/80d * connection35([2,0,1],alphas),
                -27d/80d * connection37([0,1],alphas),
                27d/16d * connection36([0,1],alphas)
            ],
            [
                3d/80d * connection35([0,1,2],alphas),
                3d/80d * connection33([1,2],alphas),
                3d/80d * connection34([2,1],alphas),
                -27d/160d * connection38([1, 0, 2],alphas),
                27d/32d * connection38([1, 0, 2],alphas),
                27d/16d * connection36([1,2],alphas)
            ],
            [
                3d/80d * connection35([0,1,2],alphas),
                3d/80d * connection34([1,2],alphas),
                3d/80d * connection33([2,1],alphas),
                -27d/160d * connection38([1, 0, 2],alphas),
                -27d/160d * connection38([1, 0, 2],alphas),
                -27d/80d * connection37([1,2],alphas),
                27d/16d * connection36([1,2],alphas)
            ],
            [
                3d/80d * connection34([0,2],alphas),
                3d/80d * connection35([1,0,2],alphas),
                3d/80d * connection33([2,0],alphas),
                -27d/160d * connection38([0, 1, 2],alphas),
                -27d/160d * connection38([0, 1, 2],alphas),
                -27d/160d * connection38([2, 1, 0],alphas),
                27d/32d * connection38([2, 0, 1],alphas),
                27d/16d * connection36([0,2],alphas)
            ],
            [
                3d/80d * connection33([0,2],alphas),
                3d/80d * connection35([1,0,2],alphas),
                3d/80d * connection34([2,0],alphas),
                27d/32d * connection38([0, 1, 2],alphas),
                -27d/160d * connection38([0, 1, 2],alphas),
                -27d/160d * connection38([2, 0, 1],alphas),
                -27d/160d * connection38([2, 0, 1],alphas),
                -27d/80d * connection37([0,2],alphas),
                27d/16d * connection36([0,2],alphas)
            ],
            [
                9d/80d * connection39([0],alphas),
                9d/80d * connection39([1],alphas),
                9d/80d * connection39([2],alphas),
                81d/80d * connection38([1, 0, 2],alphas),
                81d/80d * connection38([0, 1, 2],alphas),
                81d/80d * connection38([2, 1, 0],alphas),
                81d/80d * connection38([1, 2, 0],alphas),
                81d/80d * connection38([0, 2, 1],alphas),
                81d/80d * connection38([2, 0, 1],alphas),
                81d/20d * connection310(alphas)
            ]
        ];
        for (int i = 0; i < G.GetLength(0); ++i)
        {
            for(int j = 0; j <= i; ++j)
                G[i][j] *= Coefficient * detD;
        }
        return G;
    }
}