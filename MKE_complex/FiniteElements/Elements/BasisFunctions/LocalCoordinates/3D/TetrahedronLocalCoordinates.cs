using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
public static class TetrahedronLocalCoordinates
{
    public static class Alpha
    {
        public static double CalcSignedDetD(ReadOnlySpan<Vector3D> vertices)
        {
            return (vertices[1].X - vertices[0].X) * ((vertices[2].Y - vertices[0].Y)*(vertices[3].Z - vertices[0].Z)-(vertices[3].Y-vertices[0].Y)*(vertices[2].Z-vertices[0].Z))+
                   (vertices[1].Y - vertices[0].Y) * ((vertices[2].Z - vertices[0].Z)*(vertices[3].X - vertices[0].X)-(vertices[3].Z-vertices[0].Z)*(vertices[2].X-vertices[0].X))+
                   (vertices[1].Z - vertices[0].Z) * ((vertices[2].X - vertices[0].X)*(vertices[3].Y - vertices[0].Y)-(vertices[3].X-vertices[0].X)*(vertices[2].Y-vertices[0].Y));
        }

        public static double CalcAbsDetD(ReadOnlySpan<Vector3D> vertices)
        {
            return Math.Abs(CalcSignedDetD(vertices));
        }

        public static double[][] CalcD(ReadOnlySpan<Vector3D> vertices)
        {
            double[][] D = [ [1d, 1d, 1d, 1d ], 
                            [ vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X ],
                            [ vertices[0].Y, vertices[1].Y, vertices[2].Y, vertices[3].Y ],
                            [ vertices[0].Z, vertices[1].Z, vertices[2].Z, vertices[3].Z ],
                            ];
            return D;
        }

        public static double[][] CalcAlphas(ReadOnlySpan<Vector3D> vertices)
        {
            double detD = CalcSignedDetD(vertices);
            var verticesCopy = vertices.ToArray();
            double[] x = [.. verticesCopy.Select(i => i.X)];
            double[] y = [.. verticesCopy.Select(i => i.Y)];
            double[] z = [.. verticesCopy.Select(i => i.Z)];

            int N = 4;

            double[] coefs = [1d,-1d];
            (int coef, int x, int y, int z)[][] indices =
            [
                [(1,3,2,1),(0,2,3,1),(0,3,1,2),(1,1,3,2),(1,2,1,3),(0,1,2,3)],
                [(0,-1,2,1),(1,-1,3,1),(1,-1,1,2),(0,-1,3,2),(0,-1,1,3),(1,-1,2,3)],
                [(1,2,-1,1),(0,3,-1,1),(0,1,-1,2),(1,3,-1,2),(1,1,-1,3),(0,2,-1,3)],
                [(0,2,1,-1),(1,3,1,-1),(1,1,2,-1),(0,3,2,-1),(0,1,3,-1),(1,2,3,-1)],

                [(0,3,2,0),(1,2,3,0),(1,3,0,2),(0,0,3,2),(0,2,0,3),(1,0,2,3)],
                [(1,-1,2,0),(0,-1,3,0),(0,-1,0,2),(1,-1,3,2),(1,-1,0,3),(0,-1,2,3)],
                [(0,2,-1,0),(1,3,-1,0),(1,0,-1,2),(0,3,-1,2),(0,0,-1,3),(1,2,-1,3)],
                [(1,2,0,-1),(0,3,0,-1),(0,0,2,-1),(1,3,2,-1),(1,0,3,-1),(0,2,3,-1)],

                [(1,3,1,0),(0,1,3,0),(0,3,0,1),(1,0,3,1),(1,1,0,3),(0,0,1,3)],
                [(0,-1,1,0),(1,-1,3,0),(1,-1,0,1),(0,-1,3,1),(0,-1,0,3),(1,-1,1,3)],
                [(1,1,-1,0),(0,3,-1,0),(0,0,-1,1),(1,3,-1,1),(1,0,-1,3),(0,1,-1,3)],
                [(0,1,0,-1),(1,3,0,-1),(1,0,1,-1),(0,3,1,-1),(0,0,3,-1),(1,1,3,-1)],

                [(0,2,1,0),(1,1,2,0),(1,2,0,1),(0,0,2,1),(0,1,0,2),(1,0,1,2)],
                [(1,-1,1,0),(0,-1,2,0),(0,-1,0,1),(1,-1,2,1),(1,-1,0,2),(0,-1,1,2)],
                [(0,1,-1,0),(1,2,-1,0),(1,0,-1,1),(0,2,-1,1),(0,0,-1,2),(1,1,-1,2)],
                [(1,1,0,-1),(0,2,0,-1),(0,0,1,-1),(1,2,1,-1),(1,0,2,-1),(0,1,2,-1)],
            ];
            double[][] Alphas = new double[N][];
            for(int i = 0; i < N; ++i)
            {
                Alphas[i] = new double[N];
                for(int j = 0; j < N; ++j)
                {
                    int num = i * N + j;
                    double sum = 0d;
                    foreach(var elem in indices[num])
                    {
                        sum += coefs[elem.coef] * (elem.x >= 0 ? x[elem.x] : 1d)
                                                * (elem.y >= 0 ? y[elem.y] : 1d)
                                                * (elem.z >= 0 ? z[elem.z] : 1d);
                    }
                    Alphas[i][j] = sum;
                }
            }

            Alphas = [.. Alphas.Select(i => i.Select(j => j/detD).ToArray())];
            
            return Alphas;
        }
    }

    public static Func<Vector3D, double[][], double>[] LocalCoordinates =
    [
        (p, alpha) => alpha[0][0] + alpha[0][1] * p.X + alpha[0][2] * p.Y + alpha[0][3] * p.Z,
        (p, alpha) => alpha[1][0] + alpha[1][1] * p.X + alpha[1][2] * p.Y + alpha[1][3] * p.Z,
        (p, alpha) => alpha[2][0] + alpha[2][1] * p.X + alpha[2][2] * p.Y + alpha[2][3] * p.Z,
        (p, alpha) => alpha[3][0] + alpha[3][1] * p.X + alpha[3][2] * p.Y + alpha[3][3] * p.Z,
    ];
    
    public static Vector3D LocalCoordinatesToGlobal(ReadOnlySpan<Vector3D> vertices, ReadOnlySpan<double> localCoordinates)
    {
        double x = 0d, y = 0d, z = 0d;
        
        for(int i = 0; i < localCoordinates.Length; ++i)
        {
            x += localCoordinates[i] * vertices[i].X;
            y += localCoordinates[i] * vertices[i].Y;
            z += localCoordinates[i] * vertices[i].Z;
        }

        return new(x,y,z);
    }
}