using global::MKE_complex.Vector;
//using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;

public static class LineLagrangianBases
{
    public static Func<double, double>[] BasisFunctions(int order)
    {
        switch(order)
        {
            case 1: return PsiLinear;
            case 2: return PsiQuadratic;
            case 3: return PsiCubic;
            default: throw new ArgumentException("Wrong element order");
        }
    }
 
    private static Func<double, double>[] PsiLinear =
        [
            (double xi) => 1d - xi,
            (double xi) => xi,
        ];

    private static Func<double, double>[] PsiQuadratic =
        [
            (double xi) => 2d * (xi - 1d/2d) * (xi - 1d),
            (double xi) => - 4d * xi * (xi - 1),
            (double xi) => 2d * xi * (xi - 1d/2d),
        ];
    private static Func<double, double>[] PsiCubic =
        [
            (double xi) => -2d / 9d *(xi - 1d/3d) * (xi - 2d/3d) * (xi - 1d),
            (double xi) => 2d / 27d * xi * (xi - 2d/3d) * (xi - 1d),
            (double xi) => -2d / 27d * xi * (xi - 1d/3d) * (xi - 1d),
            (double xi) => 2d / 9d * xi * (xi - 1d/3d) * (xi - 2d/3d),
        ];
 
}
