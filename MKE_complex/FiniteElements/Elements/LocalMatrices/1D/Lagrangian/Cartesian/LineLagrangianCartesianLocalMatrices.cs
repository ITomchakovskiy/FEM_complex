using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._1D.Lagrangian.Cartesian;
public class LineLagrangianCartesianLocalMatrices
{
    public static double[][] CalculateLocalStiffnessMatrix(int order, double h, double Coefficient)
    {
        switch(order)
        {
            case 1: return G1(h, Coefficient);
            case 2: return G2(h, Coefficient);
            case 3: return G3(h, Coefficient);
            default: throw new ArgumentException("wrong element order");
        }
    }
    public static double[][] CalculateLocalMassMatrix(int order, double h, double Coefficient)
    {
        switch(order)
        {
            case 1: return M1(h, Coefficient);
            case 2: return M2(h, Coefficient);
            case 3: return M3(h, Coefficient);
            default: throw new ArgumentException("wrong element order");
        }
    }
    private static double[][] G1(double h, double Coefficient)
    {
        double[][] G = [[1d],
                        [-1d, 1d]];
        for(int i = 0; i < G.GetLength(0); ++i) // * Coef / h
        {
            for (int j = 0; j <= i; ++j)
                G[i][j] *= Coefficient / h;
        }
        return G;
    }

    private static double[][] M1(double h, double Coefficient)
    {
        double[][] M = [[2d],
                        [1d, 2d]];

        for(int i = 0; i < M.GetLength(0); ++i) // * Coef * h / 6
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] *= Coefficient * h / 6d;
        }
        return M;
    }
    private static double[][] G2(double h, double Coefficient)
    {
        double[][] G = [[7d],
                        [-8d, 16d],
                        [1d, -8d, 7d]];
        for(int i = 0; i < G.GetLength(0); ++i) // * Coef / 3 / h
        {
            for (int j = 0; j <= i; ++j)
                G[i][j] *= Coefficient / 3d / h;
        }
        return G;
    }
    private static double[][] M2(double h, double Coefficient)
    {
        double[][] M = [[4d],
                        [2d, 16d],
                        [-1d, 2d, 4d]];
        for (int i = 0; i < M.GetLength(0); ++i) // *Coef*h/30
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] *= Coefficient * h / 30d;
        }
        return M;
    }

    private static double[][] G3(double h, double Coefficient)
    {
        double[][] G = [[148d],
                        [-189d, 432d],
                        [54d,  -297d, 432d],
                        [-13,   54d, -189d, 148d]];

        for(int i = 0; i < G.GetLength(0); ++i) // * Coef / 40 / h
        {
            for (int j = 0; j <= i; ++j)
                G[i][j] *= Coefficient / 40d / h;
        }
        return G;
    }

    private static double[][] M3(double h, double Coefficient)
    {
        double[][] M = [[128d],
                        [99d, 648d],
                        [-36d, -81d, 648d],
                        [19d, -36d, 99d, 128d]];

                        
        for (int i = 0; i < M.GetLength(0); ++i) // *Coef*h/1680
        {
            for (int j = 0; j <= i; ++j)
                M[i][j] *= Coefficient * h / 1680d;
        }
        return M;
    }
}