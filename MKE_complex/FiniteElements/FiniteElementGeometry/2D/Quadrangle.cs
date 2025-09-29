using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Quadrangle(int[] VertexNumber) : IFiniteElementGeometry<Vector2D>
{
    public GeometryType GeometryType => GeometryType.Quadrangle;

    public (int, int) Edge(int edgeNumber)
    {
        switch(edgeNumber)
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 3);
            case 3: return (3, 0);
            default: throw new Exception("Wrong edge number");
        }
    }

    public Triangle[] ToTriangles()
    {
        int[][] triangleVertices_local = [[0, 1, 3], [1, 2, 3]];
        // int[][] triangleVertices = 
        Triangle[] triangles = new Triangle[2];
        for(int i = 0; i < triangles.Length; ++i)
            triangles[i] = new Triangle(triangleVertices_local[i].Select(j => VertexNumber[j]).ToArray());
        return triangles;
    }

    public static Vector2D PointOnQuadrangle(Vector2D[] vertices, int n_x, double k_x, int ind_x, int n_y, double k_y, int ind_y) //for mesh initialization
    {
        Vector2D A = (Vector2D)Vector2D.PointOnLine(vertices[0], vertices[3], n_x, k_x, ind_x);

        Vector2D B = (Vector2D)Vector2D.PointOnLine(vertices[1], vertices[2], n_x, k_x, ind_x);

        return (Vector2D)Vector2D.PointOnLine(A, B, n_y, k_y, ind_y);
    }
}
