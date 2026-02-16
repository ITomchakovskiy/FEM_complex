using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
public class TriangleLagrangianBases
{

    public static Func<double[], double>[] Psi(int order)
    {
        switch(order)
        {
            case 1: return PsiLinear;
            case 2: return PsiQuadratic;
            case 3: return PsiCubic;
            default: throw new ArgumentException("wrong element order");
        }
    }

    private static Func<double[], double>[] PsiLinear =
    [
        (double[] L) => L[0],
        (double[] L) => L[1],
        (double[] L) => L[2],
    ];

    private static Func<double[], double>[] PsiQuadratic = 
    [
        (double[] L) => L[0] * (2d * L[0] - 1d),
        (double[] L) => L[1] * (2d * L[1] - 1d),
        (double[] L) => L[2] * (2d * L[2] - 1d),
        (double[] L) => 4d * L[0] * L[1],
        (double[] L) => 4d * L[1] * L[2],
        (double[] L) => 4d * L[0] * L[2],
    ];

    private static Func<double[], double>[] PsiCubic = 
    [
        (double[] L) => 1d / 2d * L[0] * (3d * L[0] - 1d) * (3d * L[0] - 2d),
        (double[] L) => 1d / 2d * L[1] * (3d * L[1] - 1d) * (3d * L[1] - 2d),
        (double[] L) => 1d / 2d * L[2] * (3d * L[2] - 1d) * (3d * L[2] - 2d),
        (double[] L) => 9d / 2d * L[0] * L[1] * (3d * L[0] - 1d),
        (double[] L) => 9d / 2d * L[0] * L[1] * (3d * L[1] - 1d),
        (double[] L) => 9d / 2d * L[1] * L[2] * (3d * L[1] - 1d),
        (double[] L) => 9d / 2d * L[1] * L[2] * (3d * L[2] - 1d),
        (double[] L) => 9d / 2d * L[0] * L[2] * (3d * L[2] - 1d),
        (double[] L) => 9d / 2d * L[0] * L[2] * (3d * L[0] - 1d),
        (double[] L) => 27d * L[0] * L[1] * L[2],
    ];  
}