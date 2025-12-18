using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions;

public static class RectangleLagrangianBases
{
    public static int localXDofNum(int i , int order)  =>  i % (order + 1);

    public static int localYDofNum(int i, int order) => i / (order + 1);

    public static (double xi, double eta) XiEta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        return (xi, eta);
    }
    public static double Xi(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        return xi;
    }

    public static double Eta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        return eta;
    }

    public static class RectangleLagrangianLinearBases
    {
        public static Func<int, double, double, double> Psi =
                          (int i, double xi, double eta) => LineLagrangianLinearBases.Psi[localXDofNum(i, 1)](xi) * 
                                                            LineLagrangianLinearBases.Psi[localYDofNum(i, 1)](eta);
       
    }

    public static class RectangleLagrangianQuadraticBases
    {
        public static Func<int, double, double, double> Psi =
                           (int i, double xi, double eta) => LineLagrangianQuadraticBases.Psi[localXDofNum(i, 2)](xi) *
                                                             LineLagrangianQuadraticBases.Psi[localYDofNum(i, 2)](eta);
    }
    public static class RectangleLagrangianCubicBases
    {
        public static Func<int, double, double, double> Psi =
                           (int i, double xi, double eta) => LineLagrangianCubicBases.Psi[localXDofNum(i, 3)](xi) *
                                                             LineLagrangianCubicBases.Psi[localYDofNum(i, 3)](eta);
    }
}


