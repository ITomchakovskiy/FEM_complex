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
    public static double Xi<VectorT>(ReadOnlySpan<VectorT> vertices, VectorT point) where VectorT : VectorBase<double, VectorT>
    {
        return VectorBase<double, VectorT>.Length(vertices[0], point) / (vertices[1] - vertices[0]).Norm();
    }

    public static double Xi(ReadOnlySpan<double> vertices, double point)
    {
        return (point - vertices[0]) / (vertices[1] - vertices[0]);
    }

    public static double LocarCoordinatesToGlobal(ReadOnlySpan<double> vertices, double xi)
    {
        double h = vertices[1] - vertices[0];
        return h * xi + vertices[0];
    }

    public static VectorT LocarCoordinatesToGlobal<VectorT>(ReadOnlySpan<VectorT> vertices, double xi) where VectorT : VectorBase<double, VectorT>
    {
        var h = vertices[1] - vertices[0];
        return h * xi + vertices[0];
    }


    public static Func<double, double>[] Psi(int order)
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
