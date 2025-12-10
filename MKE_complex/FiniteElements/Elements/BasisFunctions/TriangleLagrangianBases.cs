using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions;

public static class Alpha
{
    public static double CalcDetD(ReadOnlySpan<Vector2D> vertices)
    {
        return (vertices[1].X - vertices[0].X) * (vertices[2].Y - vertices[0].Y)
             - (vertices[2].X - vertices[0].X) * (vertices[1].Y - vertices[0].Y);
    }

    public static double[,] CalcD(ReadOnlySpan<Vector2D> vertices)
    {
        double[,] D = { {1d, 1d, 1d }, { vertices[0].X, vertices[1].X, vertices[2].X },
                                       { vertices[0].Y, vertices[1].Y, vertices[2].Y }};
        return D;
    }

    public static double[,] CalcAlphas(ReadOnlySpan<Vector2D> vertices)
    {
        double detD = CalcDetD(vertices);
        double[] x = [ vertices[0].X, vertices[1].X, vertices[2].X];
        double[] y = [ vertices[0].Y, vertices[1].Y, vertices[2].Y];
        double[,] Alphas = { { x[1] * y[2] - x[2] * y[1], y[1] - y[2], x[2] - x[1] },
                             { x[2] * y[0] - x[0] * y[2], y[2] - y[0], x[0] - x[2] },
                             { x[0] * y[1] - x[1] * y[0], y[0] - y[1], x[1] - x[0] }};
        for (int i = 0; i < 3; ++i)
        {
            for(int j = 0; j < 3; ++j)
                Alphas[i, j] /= detD;
        }
        return Alphas;
    }
}

public static class TriangleLinearLagrangianBases
{
    public static Func<Vector2D, double[,], double>[] Psi =
    [
        (Vector2D p,  double [,] alpha) => alpha[0,0] + alpha[0,1] * p.X + alpha[0,2] * p.Y,
        (Vector2D p,  double [,] alpha) => alpha[1,0] + alpha[1,1] * p.X + alpha[1,2] * p.Y,
        (Vector2D p,  double [,] alpha) => alpha[2,0] + alpha[2,1] * p.X + alpha[2,2] * p.Y,
    ];
}

public static class TriangleQuadraticLagrangianBases
{
    public static Func<double[], double>[] Psi = 
    [
        (double[] L) => L[0] * (2d * L[0] - 1d),
        (double[] L) => L[1] * (2d * L[1] - 1d),
        (double[] L) => L[2] * (2d * L[2] - 1d),
        (double[] L) => 4d * L[0] * L[1],
        (double[] L) => 4d * L[1] * L[2],
        (double[] L) => 4d * L[0] * L[2],
    ];
}
