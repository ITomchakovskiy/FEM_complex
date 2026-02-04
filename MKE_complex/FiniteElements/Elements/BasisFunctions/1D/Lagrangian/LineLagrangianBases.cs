using global::MKE_complex.Vector;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;

public static class LineLagrangianBases
{
    //public static class LineLocalCoordinates
    //{
        public static double Xi(ReadOnlySpan<Vector1D> vertices, Vector1D point)
        {
            return (point.X - vertices[0].X) / (vertices[1].X - vertices[0].X);
        }

        public static double Xi(ReadOnlySpan<double> vertices, double point)
        {
            return (point - vertices[0]) / (vertices[1] - vertices[0]);
        }
 //   }

//    private static class LineLagrangianLinearBases
//   {
        private static Func<double, double>[] PsiLinear =
          [
             (double xi) => 1d - xi,
             (double xi) => xi,
          ];
//    }

    //private static class LineLagrangianQuadraticBases
    //{
        private static Func<double, double>[] PsiQuadratic =
          [
             (double xi) => 2d * (xi - 1d/2d) * (xi - 1d),
         (double xi) => - 4d * xi * (xi - 1),
         (double xi) => 2d * xi * (xi - 1d/2d),
      ];
    //}

    //private static class LineLagrangianCubicBases
    //{
        private static Func<double, double>[] PsiCubic =
          [
             (double xi) => -2d / 9d *(xi - 1d/3d) * (xi - 2d/3d) * (xi - 1d),
         (double xi) => 2d / 27d * xi * (xi - 2d/3d) * (xi - 1d),
         (double xi) => -2d / 27d * xi * (xi - 1d/3d) * (xi - 1d),
         (double xi) => 2d / 9d * xi * (xi - 1d/3d) * (xi - 2d/3d),
      ];
    //}

}
