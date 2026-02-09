using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
public class RectangleLagrangianBases
{
    public static int localXDofNum(int i , int order)  =>  i % (order + 1);

    public static int localYDofNum(int i, int order) => i / (order + 1);

    public static (double xi, double eta) XiEta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double xi = LineLagrangianBases.Xi(x, point.X);
        double eta = LineLagrangianBases.Xi(y, point.Y);
        return (xi, eta);
    }
    public static double Xi(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double xi = LineLagrangianBases.Xi(x, point.X);
        return xi;
    }

    public static double Eta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double eta = LineLagrangianBases.Xi(y, point.Y);
        return eta;
    }

    public Func<int, double, double, double> Psi(int order)
    {
        if(order < 1) throw new ArgumentException("wrong element order");
        return (int i, double xi, double eta) => LineLagrangianBases.Psi(order)[localXDofNum(i, order)](xi) * 
                                                 LineLagrangianBases.Psi(order)[localYDofNum(i, order)](eta);
    }
}