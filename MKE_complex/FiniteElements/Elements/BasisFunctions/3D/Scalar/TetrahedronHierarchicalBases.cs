using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar;
public class TetrahedronHierarchicalBases
{
    private const int PrecalculatedOrders = 3;
    private const int PrecalculatedFunctionsCount = 20;

    public static int CalcDofsCount(int order) => 4 + 6 * (order - 1) + 
                                                   2 * (order - 1) * (order - 2) +
                                                   (order - 1) * (order - 2) * (order - 3) / 6;

    public static int NewDofsOnEdgesCountForOrder() => 6;
    public static int NewDofsOnFacesCountForOrder(int order) => 4 * (order - 2);
    public static int NewDofsOnElementCountForOrder(int order) => (order - 2) * (order - 3) / 2;
    
    private static Func<double, double> Xp(Func<double, double> LineBasis) => 
        LineHierarchicalBases.HierarchicalPolinomialWithNonZeroEdges(LineBasis);

    private static Func<double[], double>[] PreCalculatedHierarchicalBases =
    [
        L => L[0],
        L => L[1],
        L => L[2], 
        L => L[3],
        L => L[0] * L[1],
        L => L[0] * L[2],
        L => L[0] * L[3],
        L => L[1] * L[2],
        L => L[1] * L[3],
        L => L[2] * L[3],
        L => L[0] * L[1] * (L[0] - L[1]),
        L => L[0] * L[2] * (L[0] - L[2]),
        L => L[0] * L[3] * (L[0] - L[3]),
        L => L[1] * L[2] * (L[1] - L[2]),
        L => L[1] * L[3] * (L[1] - L[3]),
        L => L[2] * L[3] * (L[2] - L[3]),
        L => L[0] * L[1] * L[2],
        L => L[0] * L[1] * L[3],
        L => L[0] * L[2] * L[3],
        L => L[1] * L[2] * L[3]
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
            Result[start + 1] = L => L[0] * L[2] * Xp(LineBases[iOrderCopy])(L[0] - L[2]);
            Result[start + 2] = L => L[0] * L[3] * Xp(LineBases[iOrderCopy])(L[0] - L[3]);
            Result[start + 3] = L => L[1] * L[2] * Xp(LineBases[iOrderCopy])(L[1] - L[2]);
            Result[start + 4] = L => L[1] * L[3] * Xp(LineBases[iOrderCopy])(L[1] - L[3]);
            Result[start + 5] = L => L[2] * L[3] * Xp(LineBases[iOrderCopy])(L[2] - L[3]);

            for(int i = 1; i <= iOrder - 2; ++i)
            {
                int iCopy = i;
                Result[start + 5 + i] =                    L => L[0]*L[1]*L[2]*Math.Pow(L[0]-L[1], iCopy - 1) *
                                                                               Math.Pow(2d*L[2] - 1d, iOrderCopy-2-iCopy);
                Result[start + 5 + i + (iOrder - 2)] =     L => L[0]*L[1]*L[3]*Math.Pow(L[0]-L[1], iCopy - 1) *
                                                                               Math.Pow(2d*L[3] - 1d, iOrderCopy-2-iCopy);
                Result[start + 5 + i + 2 * (iOrder - 2)] = L => L[0]*L[2]*L[3]*Math.Pow(L[0]-L[2], iCopy - 1) *
                                                                               Math.Pow(2d*L[3] - 1d, iOrderCopy-2-iCopy);
                Result[start + 5 + i + 3 * (iOrder - 2)] = L => L[1]*L[2]*L[3]*Math.Pow(L[1]-L[2], iCopy - 1) *
                                                                               Math.Pow(2d*L[3] - 1d, iOrderCopy-2-iCopy);
            }

            int ElementDofsCount = NewDofsOnElementCountForOrder(iOrder);

            for(int i = 1; i <= iOrder - 3; ++i)
            {
                int iCopy = i;
                for(int j = 1; j <= iOrder - 2 - i; ++j)
                {
                    int jCopy = j;
                    int index = start - 1 + NewDofsOnEdgesCountForOrder() + NewDofsOnFacesCountForOrder(iOrder) + 
                                (2 * iOrder - 4 - i) * (i - 1) / 2 + j;
                    Result[index] = L => L[0] * L[1] * L[2] * L[3] * Math.Pow(L[0] - L[1], iCopy - 1) * 
                                                                     Math.Pow(2d*L[2] - 1d, jCopy - 1) * 
                                                                     Math.Pow(2d*L[3] - 1d, iOrderCopy - 2 - jCopy - iCopy);
                }
            }
                
        }

        return Result;
    }
}