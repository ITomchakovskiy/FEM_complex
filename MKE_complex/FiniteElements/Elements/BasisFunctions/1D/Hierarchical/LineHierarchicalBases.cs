using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;

public enum PolinomialType {Simple, AlternatingSimple, Legendre}

public class LineHierarchicalBases
{
    private static Func<double, double>[] PreCalculatedLegendrePolinomials;

    private static Func<double, double>[] PreCalculatedHierarchicalSimpleBases;
    
    private static Func<double, double>[] PreCalculatedHierarchicalAlternatingSimpleBases;

    private static Func<double, double>[] PreCalculatedHierarchicalLegendreBases;

    private const int PrecalculatedFunctionsCount = 6;

    public static Func<double, double> HierarchicalPolinomialWithNonZeroEdges(Func<double, double> LineBasis) // phi / (xi^2 - 1)
    {
        double eps = 1.0E-10;
        return xi => xi + 1d > eps && 1d - xi > eps ? 
               LineBasis((xi + 1d) / 2d) / (xi*xi - 1d) :
               LineBasis(((1d - eps) * Math.Sign(xi) + 1d) / 2d) / ((1d - eps)*
                                                                    (1d - eps) - 1d);
    }

    private static Func<double, double>[] GetLegendreBases(int order)
    {
        int CalculatedBasesCount = Math.Max(0, order - PrecalculatedFunctionsCount + 1);
        int PreCalculatedBasesCount = Math.Min(PrecalculatedFunctionsCount, order + 1);
        var LegendrePolinomials = PreCalculatedLegendrePolinomials.Take(PreCalculatedBasesCount).Concat(
                     new Func<double, double>[CalculatedBasesCount]).ToArray();

        for(int i = 0; i < CalculatedBasesCount; ++i)
        {
            int index = i + PrecalculatedFunctionsCount;
            LegendrePolinomials[index] = (xi) => (2d * index - 1d) / index * (2d * xi - 1d) * LegendrePolinomials[index - 1](xi) - 
                                    (index - 1d) / index * LegendrePolinomials[index-2](xi);
        }

        var Result = PreCalculatedHierarchicalLegendreBases.Take(PreCalculatedBasesCount).Concat(
                new Func<double, double>[CalculatedBasesCount]).ToArray();
        
        for(int i = 0; i < CalculatedBasesCount; ++i)
        {
            int index = i + PrecalculatedFunctionsCount;
            Result[index] = (xi) => LegendrePolinomials[index](xi) -
                                    LegendrePolinomials[index - 2](xi);
        }

        return Result;
    }

    private static Func<double, double>[] GetSimpleBases(int order)
    {
        int CalculatedBasesCount = Math.Max(0, order - PrecalculatedFunctionsCount + 1);
        int PreCalculatedBasesCount = Math.Min(PrecalculatedFunctionsCount, order + 1);

        var Result = PreCalculatedHierarchicalLegendreBases.Take(PreCalculatedBasesCount).Concat(
                new Func<double, double>[CalculatedBasesCount]).ToArray();
        
        for(int i = 0; i < CalculatedBasesCount; ++i)
        {
            int index = i + PrecalculatedFunctionsCount;
            Result[index] = (xi) => Result[index - 1](xi) * (xi * 2d - 1d);
        }

        return Result;
    }

    public static Func<double, double>[] BasisFunctions(int order, PolinomialType polinomial)
    {

        switch(polinomial)
        {
            case PolinomialType.Legendre:
                    return GetLegendreBases(order);
            case PolinomialType.Simple:
                return GetSimpleBases(order);
            default:
                throw new NotImplementedException();
        }
    }

    private static double LocalCoordinateToHierarchicalLocalCoordinate(double xi) => 2d * xi - 1d;

    static LineHierarchicalBases()
    {
        Func<double, double>[] XiPow =
        [
            (xi) => 1d,
            (xi) => xi,
            (xi) => xi*xi,
            (xi) => xi*xi*xi,
            (xi) => xi*xi*xi*xi,
            (xi) => xi*xi*xi*xi*xi
        ];

        Func<double, double>[] LegendrePolinomialsForHierarchicalLocalCoordinates =
        [
            (xi) => XiPow[0](xi),
            (xi) => XiPow[1](xi),
            (xi) => 1d/2d * (3*XiPow[2](xi) - 1d),
            (xi) => 1d/2d * (5*XiPow[3](xi) - 3*XiPow[1](xi)),
            (xi) => 1d/8d * (35*XiPow[4](xi) - 30*XiPow[2](xi) + 3),
            (xi) => 1d/8d * (63*XiPow[5](xi) - 70*XiPow[3](xi) + 15*XiPow[1](xi))
        ];

        PreCalculatedLegendrePolinomials = [.. LegendrePolinomialsForHierarchicalLocalCoordinates.
                                    Select<Func<double,double>, Func<double,double>>
                                    (f => xi => f(LocalCoordinateToHierarchicalLocalCoordinate(xi)))];

        Func<double, double>[] HierarchicalLegendreBasesForHierarchicalLocalCoordinates =
        [
            (xi) => (1d - XiPow[1](xi))/2d,
            (xi) => (1d + XiPow[1](xi))/2d,
            (xi) => XiPow[2](xi) - 1d,
            (xi) => XiPow[3](xi) - XiPow[1](xi),
            (xi) => 5*XiPow[4](xi) - 6*XiPow[2](xi) + 1d,
            (xi) => 7*XiPow[5](xi) - 10*XiPow[3](xi) + 3*XiPow[1](xi)
        ];

        PreCalculatedHierarchicalLegendreBases = [.. HierarchicalLegendreBasesForHierarchicalLocalCoordinates.
                                    Select<Func<double,double>, Func<double,double>>
                                    (f => xi => f(LocalCoordinateToHierarchicalLocalCoordinate(xi)))];

        var PreCalculatedHierarchicalSimpleBasesForHierarchicalLocalCoordinates = new Func<double, double>[PrecalculatedFunctionsCount]; //To be implemented later

        PreCalculatedHierarchicalSimpleBasesForHierarchicalLocalCoordinates[0] = (xi) => (1d - XiPow[1](xi))/2d;
        PreCalculatedHierarchicalSimpleBasesForHierarchicalLocalCoordinates[1] = (xi) => (1d + XiPow[1](xi))/2d;

        for(int i = 2; i < PrecalculatedFunctionsCount; ++i)
        {
            int index = i;
            PreCalculatedHierarchicalSimpleBasesForHierarchicalLocalCoordinates[i] = (xi) => XiPow[index - 2](xi) * (1d - XiPow[2](xi));
        }
            
        PreCalculatedHierarchicalSimpleBases = [.. PreCalculatedHierarchicalSimpleBasesForHierarchicalLocalCoordinates.
                                    Select<Func<double,double>, Func<double,double>>
                                    (f => xi => f(LocalCoordinateToHierarchicalLocalCoordinate(xi)))];
        PreCalculatedHierarchicalAlternatingSimpleBases = []; //To be implemented later
    }
}