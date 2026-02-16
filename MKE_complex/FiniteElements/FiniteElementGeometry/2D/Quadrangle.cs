using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Quadrangle<VectorT>(int[] VertexNumber) : IFiniteElementGeometry<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public GeometryType GeometryType => GeometryType.Quadrangle;

    public int EdgesCount => 4;

    public (int, int) LocalEdge(int edgeNumber)
    {
        switch(edgeNumber) //x0y0 -> x0y1 -> x1y1 -> x1y0
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 3);
            case 3: return (3, 0);
            default: throw new Exception("Wrong edge number");
        }
    }

    public Triangle<VectorT>[] ToTriangles()
    {
        int[][] triangleVertices_local = [[0, 1, 3], [1, 2, 3]];
        // int[][] triangleVertices = 
        Triangle<VectorT>[] triangles = new Triangle<VectorT>[2];
        for(int i = 0; i < triangles.Length; ++i)
            triangles[i] = new Triangle<VectorT>(triangleVertices_local[i].Select(j => VertexNumber[j]).ToArray());
        return triangles;
    }

    public bool IsPointInElement(VectorT point, VectorT[] vertices)
    {
        throw new NotImplementedException();
    }

    IFiniteElementGeometry<VectorT>[] IFiniteElementGeometry<VectorT>.Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }
}
