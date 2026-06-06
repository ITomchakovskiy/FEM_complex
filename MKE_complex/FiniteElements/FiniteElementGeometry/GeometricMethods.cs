using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry;
public static class GeometricMethods
{
    public static Vector2D PointOnQuadrangle(Vector2D[] vertices, int n_x, double k_x, int ind_x, int n_y, double k_y, int ind_y) //for mesh initialization
    {
        Vector2D A = PointOnLine(vertices[0], vertices[3], n_x, k_x, ind_x);

        Vector2D B = PointOnLine(vertices[1], vertices[2], n_x, k_x, ind_x);

        return PointOnLine(A, B, n_y, k_y, ind_y);
    }

    public static VectorT PointOnLine<VectorT>(VectorT A, VectorT B, int n, double k, int ind) where VectorT : VectorBase<double, VectorT>
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        if (ind == 0) return A;
        if (ind == n) return B;
        var r = B - A;
        double l = r.Norm();
        if (Math.Abs(k - 1d) < 1.0E-13)
            return A + r / n * ind;

        double l_ind = l * (1d - Math.Pow(Math.Abs(k), ind)) / (1d - Math.Pow(Math.Abs(k), n));

        l_ind = k > 0 ? l_ind : l - l_ind;
        return A + l_ind / l * r;
    }

    public static double PointOnLine(double A, double B, int n, double k, int ind)
    {
        if (ind == 0) return A;
        if (ind == n) return B;
        var r = B - A;
        double l = Math.Abs(r);
        if (Math.Abs(k - 1d) < 1.0E-13)
            return A + r / n * ind;
        if(k < 0d)
            ind = n - ind ;

        double l_ind = l * (1d - Math.Pow(Math.Abs(k), ind)) / (1d - Math.Pow(Math.Abs(k), n));

        l_ind = k > 0 ? l_ind : l - l_ind;
        return A + l_ind / l * r;
    }

    public static Vector2D[] _2DProjection(ReadOnlySpan<Vector3D> vertices, out string projectionPlane) //projectionPlane - const coordinate
    {
        var cross = (vertices[1] - vertices[0]).CrossProduct(vertices[2] - vertices[0]);

        Vector2D[] vertices2D;

        if(Math.Abs(cross.Z) == cross.components.Max(Math.Abs))
        {
            vertices2D = vertices.ToArray().Select(v => new Vector2D(v.X, v.Y)).ToArray();
            projectionPlane = "Z";
        }
        else if(Math.Abs(cross.Y) == cross.components.Max(Math.Abs))
        {
            vertices2D = vertices.ToArray().Select(v => new Vector2D(v.X, v.Z)).ToArray();
            projectionPlane = "Y";
        }
        else
        {
            vertices2D = vertices.ToArray().Select(v => new Vector2D(v.Y, v.Z)).ToArray();
            projectionPlane = "X";
        }

            
        return vertices2D;
    }

    public static Vector3D PointOnHexagon(Vector3D[] vertices, int n_x, double k_x, int ind_x, int n_y, double k_y, int ind_y, int n_z, double k_z, int ind_z) //for mesh initialization
    {
        Vector3D A = PointOnLine(vertices[0], vertices[3], n_x, k_x, ind_x);

        Vector3D B = PointOnLine(vertices[1], vertices[2], n_x, k_x, ind_x);

        Vector3D C = PointOnLine(vertices[4], vertices[7], n_x, k_x, ind_x);

        Vector3D D = PointOnLine(vertices[5], vertices[6], n_x, k_x, ind_x);

        Vector3D MAB = PointOnLine(A, B, n_y, k_y, ind_y);

        Vector3D MCD = PointOnLine(C, D, n_y, k_y, ind_y);

        return PointOnLine(MAB, MCD, n_y, k_y, ind_y);
    }

    public static int[] SimplifyFace(int[] face)
    {
        int[] res = new int[3];
        int startVertex = face.Min();
        var minIndex = face.IndexOf(startVertex);
        (int leftVertex, int rightVertex) = (getLeftFaceNumber(face, minIndex),
                                             getRightFaceNumber(face, minIndex));
               
        int midVertex = Math.Min(leftVertex, rightVertex);
            
        int lastVertex = leftVertex < rightVertex ? getLeftFaceNumber(face, minIndex - 1) :
                                                    getRightFaceNumber(face, minIndex + 1);

        return [startVertex, midVertex, lastVertex];

        
    }

    private static int getLeftFaceNumber(ReadOnlySpan<int> face, int startIndex) =>
        face[(startIndex - 1) >= 0 ? (startIndex - 1) : ^(-(startIndex - 1))];

    private static int getRightFaceNumber(ReadOnlySpan<int> face, int startIndex) =>
        face[(startIndex + 1) % face.Length];
    
}