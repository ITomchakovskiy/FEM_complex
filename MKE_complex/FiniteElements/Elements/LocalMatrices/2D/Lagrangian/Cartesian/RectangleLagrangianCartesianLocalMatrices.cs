using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
using MKE_complex.FiniteElements.Elements.LocalMatrices._1D.Lagrangian.Cartesian;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Lagrangian.Cartesian;
public class RectangleLagrangianCartesianLocalMatrices
{
    public static double[][] CalculateLocalStiffnessMatrix(int order, double Coefficient, double hx, double hy)
    {
        if(order < 1) throw new ArgumentException("wrong element order");

        var Gx = LineLagrangianCartesianLocalMatrices.CalculateLocalStiffnessMatrix(order, hx, Coefficient);

        var Gy = LineLagrangianCartesianLocalMatrices.CalculateLocalStiffnessMatrix(order, hy, Coefficient);

        var Mx = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(order, hx, Coefficient);

        var My = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(order, hy, Coefficient);

        int DofsCount = (order + 1) * (order + 1);

        var G = new double[DofsCount][];

        for(int i = 0; i < DofsCount; ++i)
        {
            G[i] = new double[i + 1];
            for(int j = 0; j <= i; ++j)
            {
                (int i, int j) Mu = (RectangleLagrangianBases.localXDofNum(i, order), 
                                         RectangleLagrangianBases.localXDofNum(j, order));
                Mu = Mu.j < Mu.i ? Mu : (Mu.j, Mu.i);

                (int i, int j) Nu = (RectangleLagrangianBases.localYDofNum(i, order),
                                         RectangleLagrangianBases.localYDofNum(j, order));
                Nu = Nu.j < Nu.i ? Nu : (Nu.j, Nu.i);

                G[i][j] = Gx[Mu.i][Mu.j] * My[Nu.i][Nu.j] + Mx[Mu.i][Mu.j] * Gy[Nu.i][Nu.j];
            }
        }
        return G;
    }

    public static double[][] CalculateLocalMassMatrix(int order, double Coefficient, double hx, double hy)
    {
        if(order < 1) throw new ArgumentException("wrong element order");

        var Mx = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(order, hx, Coefficient);

        var My = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(order, hy, Coefficient);

        int DofsCount = (order + 1) * (order + 1);

        var M = new double[DofsCount][];

        for (int i = 0; i < DofsCount; ++i)
        {
            M[i] = new double[i + 1];
            for (int j = 0; j <= i; ++j)
            {
                (int i, int j) Mu = (RectangleLagrangianBases.localXDofNum(i, order),
                                     RectangleLagrangianBases.localXDofNum(j, order));
                Mu = Mu.j < Mu.i ? Mu : (Mu.j, Mu.i);

                (int i, int j) Nu = (RectangleLagrangianBases.localYDofNum(i, order),
                                         RectangleLagrangianBases.localYDofNum(j, order));
                Nu = Nu.j < Nu.i ? Nu : (Nu.j, Nu.i);

                M[i][j] = Mx[Mu.i][Mu.j] * My[Nu.i][Nu.j];
            }
        }
        return M;
    }
}