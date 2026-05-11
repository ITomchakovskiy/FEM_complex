using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Java;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
public static class TriangleLocalCoordinates
{
    public static class Alpha
    {
        public static double CalcSignedDetD(ReadOnlySpan<Vector2D> vertices)
        {
            return (vertices[1].X - vertices[0].X) * (vertices[2].Y - vertices[0].Y)
                 - (vertices[2].X - vertices[0].X) * (vertices[1].Y - vertices[0].Y);
        }

        public static double CalcAbsDetD(ReadOnlySpan<Vector2D> vertices)
        {
            return Math.Abs(CalcSignedDetD(vertices));
        }

        public static double CalcAbsDetD(ReadOnlySpan<Vector3D> vertices)
        {
            return (vertices[1] - vertices[0]).CrossProduct(vertices[2] - vertices[0]).Norm();
        }

        public static double[,] CalcD(ReadOnlySpan<Vector2D> vertices)
        {
            double[,] D = { {1d, 1d, 1d }, { vertices[0].X, vertices[1].X, vertices[2].X },
                                           { vertices[0].Y, vertices[1].Y, vertices[2].Y }};
            return D;
        }

        public static double[,] CalcAlphas(ReadOnlySpan<Vector2D> vertices)
        {
            double detD = CalcSignedDetD(vertices);

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

        public static double[,] CalcAlphas(ReadOnlySpan<Vector3D> vertices, out string projectionPlane)
        {
            var vertices2D = GeometricMethods._2DProjection(vertices, out projectionPlane);

            return CalcAlphas(vertices2D);
        }
    }

    public static Func<Vector2D, double[,], double>[] LocalCoordinates =
    [
        (p,  alpha) => alpha[0,0] + alpha[0,1] * p.X + alpha[0,2] * p.Y,
        (p,  alpha) => alpha[1,0] + alpha[1,1] * p.X + alpha[1,2] * p.Y,
        (p,  alpha) => alpha[2,0] + alpha[2,1] * p.X + alpha[2,2] * p.Y,
    ];

    private static Func<Vector3D, double[,], double>[] LocalCoordinatesZConst =
    [
        (p,  alpha) => alpha[0,0] + alpha[0,1] * p.X + alpha[0,2] * p.Y,
        (p,  alpha) => alpha[1,0] + alpha[1,1] * p.X + alpha[1,2] * p.Y,
        (p,  alpha) => alpha[2,0] + alpha[2,1] * p.X + alpha[2,2] * p.Y,
    ];

    private static Func<Vector3D, double[,], double>[] LocalCoordinatesYConst =
    [
        (p,  alpha) => alpha[0,0] + alpha[0,1] * p.X + alpha[0,2] * p.Z,
        (p,  alpha) => alpha[1,0] + alpha[1,1] * p.X + alpha[1,2] * p.Z,
        (p,  alpha) => alpha[2,0] + alpha[2,1] * p.X + alpha[2,2] * p.Z,
    ];

    private static Func<Vector3D, double[,], double>[] LocalCoordinatesXConst =
    [
        (p,  alpha) => alpha[0,0] + alpha[0,1] * p.Y + alpha[0,2] * p.Z,
        (p,  alpha) => alpha[1,0] + alpha[1,1] * p.Y + alpha[1,2] * p.Z,
        (p,  alpha) => alpha[2,0] + alpha[2,1] * p.Y + alpha[2,2] * p.Z,
    ];

    public static Func<Vector3D, double[,], double>[] GetLocalCoordinates(string projectionPlane)
    {
        switch(projectionPlane)
        {
            case "Z": return LocalCoordinatesZConst;
            case "Y": return LocalCoordinatesYConst;
            case "X": return LocalCoordinatesXConst;
            default: throw new ArgumentException();
        }
    }
    
    public static Vector2D LocalCoordinatesToGlobal(ReadOnlySpan<Vector2D> vertices, ReadOnlySpan<double> localCoordinates)
    {
        double x = 0d;
        for(int i = 0; i < localCoordinates.Length; ++i)
            x += localCoordinates[i] * vertices[i].X;

        double y = 0d;
        for(int i = 0; i < localCoordinates.Length; ++i)
            y += localCoordinates[i] * vertices[i].Y;
        
        return new(x,y);
    }

    public static Vector3D LocalCoordinatesToGlobal(ReadOnlySpan<Vector3D> vertices, ReadOnlySpan<double> localCoordinates)
    {
        var xy = LocalCoordinatesToGlobal(vertices.ToArray().Select(v => new Vector2D(v.X, v.Y)).ToArray(), localCoordinates);

        double z = 0d;
        for(int i = 0; i < localCoordinates.Length; ++i)
            z += localCoordinates[i] * vertices[i].Z;

        return new(xy.X, xy.Y, z);
    }
}