using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Hierarchical;
public static class TriangleHierarchicalBases
{
    private const int PrecalculatedOrders = 3;
    private const int PrecalculatedFunctionsCount = 10;

    public static int CalcDofsCount(int order) => 
        3 * order + (order - 1)*(order - 2)/2;
    
    public static int NewDofsOnEdgesCountForOrder() => 3;

    public static int NewDofsOnElementCountForOrder(int order) => order - 2;
    
    private static Func<double, double> Xp(Func<double, double> LineBasis)
    {
        double eps = 1.0E-10;
        return xi => xi + 1d > eps && 1d - xi > eps ? 
               LineBasis((xi + 1d) / 2d) / (xi*xi - 1d) :
               LineBasis(((1d - eps) * Math.Sign(xi) + 1d) / 2d) / ((1d - eps)*
                                                                    (1d - eps) - 1d);
    }

    private static Func<double[], double>[] PreCalculatedHierarchicalBases =
    [
        (L) => L[0],
        (L) => L[1],
        (L) => L[2],
        (L) => L[0]*L[1],
        (L) => L[1]*L[2],
        (L) => L[0]*L[2],
        (L) => L[0]*L[1]*(L[0] - L[1]),
        (L) => L[1]*L[2]*(L[1] - L[2]),
        (L) => L[0]*L[2]*(L[0] - L[2]),
        (L) => L[0]*L[1]*L[2]
    ];


    public static Func<double[], double>[] BasisFunctions(int order, PolinomialType polinomial)
    {
        var LineBases = LineHierarchicalBases.BasisFunctions(order, polinomial);

        int N = CalcDofsCount(order);
        int CalculatedBasesCount = Math.Max(0, N - PrecalculatedFunctionsCount);
        int PreCalculatedBasesCount = Math.Min(PrecalculatedFunctionsCount, N);

        var Result = PreCalculatedHierarchicalBases.Take(PreCalculatedBasesCount).Concat(
                new Func<double[], double>[CalculatedBasesCount]).ToArray();

        for(int iOrder = PrecalculatedOrders + 1; iOrder <= order; ++iOrder)
        {
            int start = CalcDofsCount(iOrder - 1);
            int iOrderCopy = iOrder;
            Result[start] = (L) =>   L[0] * L[1] * Xp(LineBases[iOrderCopy])(L[0] - L[1]);
            Result[start+1] = (L) => L[1] * L[2] * Xp(LineBases[iOrderCopy])(L[1] - L[2]);
            Result[start+2] = (L) => L[0] * L[2] * Xp(LineBases[iOrderCopy])(L[0] - L[2]);

            for(int i = 1; i <= NewDofsOnElementCountForOrder(iOrder); ++i)
            {
                int iCopy = i;
                Result[start+2+i] = L => L[0]*L[1]*L[2]*Math.Pow(L[0]-L[1], iCopy - 1) *
                                                        Math.Pow(2d*L[2] - 1d, iOrderCopy-2-iCopy);
            }
                
        }

        return Result;
    }
}