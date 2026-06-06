using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;
public class TetrahedronScalarLagrangianBases
{
    public static Func<double[], double>[] BasisFunctions(int order)
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
        L => L[0],
        L => L[1],
        L => L[2],
        L => L[3]
    ];

    private static Func<double[], double>[] PsiQuadratic = 
    [
        L => L[0] * (2d * L[0] - 1d),
        L => L[1] * (2d * L[1] - 1d),
        L => L[2] * (2d * L[2] - 1d),
        L => L[3] * (2d * L[3] - 1d),
        L => 4d * L[0] * L[1],
        L => 4d * L[0] * L[2],
        L => 4d * L[0] * L[3],
        L => 4d * L[1] * L[2],
        L => 4d * L[1] * L[3],
        L => 4d * L[2] * L[3],
    ];

    private static Func<double[], double>[] PsiCubic = 
    [
        L => 1d / 2d * L[0] * (3d * L[0] - 1d) * (3d * L[0] - 2d),
        L => 1d / 2d * L[1] * (3d * L[1] - 1d) * (3d * L[1] - 2d),
        L => 1d / 2d * L[2] * (3d * L[2] - 1d) * (3d * L[2] - 2d),
        L => 1d / 2d * L[3] * (3d * L[3] - 1d) * (3d * L[3] - 2d),
        L => 9d / 2d * L[0] * L[1] * (3d * L[0] - 1d),
        L => 9d / 2d * L[0] * L[1] * (3d * L[1] - 1d),
        L => 9d / 2d * L[0] * L[2] * (3d * L[0] - 1d),
        L => 9d / 2d * L[0] * L[2] * (3d * L[2] - 1d),
        L => 9d / 2d * L[0] * L[3] * (3d * L[0] - 1d),
        L => 9d / 2d * L[0] * L[3] * (3d * L[3] - 1d),
        L => 9d / 2d * L[1] * L[2] * (3d * L[1] - 1d),
        L => 9d / 2d * L[1] * L[2] * (3d * L[2] - 1d),
        L => 9d / 2d * L[1] * L[3] * (3d * L[1] - 1d),
        L => 9d / 2d * L[1] * L[3] * (3d * L[3] - 1d),
        L => 9d / 2d * L[2] * L[3] * (3d * L[2] - 1d),
        L => 9d / 2d * L[2] * L[3] * (3d * L[3] - 1d),
        L => 27d * L[0] * L[1] * L[2],
        L => 27d * L[0] * L[1] * L[3],
        L => 27d * L[0] * L[2] * L[3],
        L => 27d * L[1] * L[2] * L[3],
    ];  
}