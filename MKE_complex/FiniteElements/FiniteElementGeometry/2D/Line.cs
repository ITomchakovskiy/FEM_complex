using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Line<VectorT>(int[] VertexNumber) : IFiniteElementGeometry<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public GeometryType GeometryType => GeometryType.Line;

    public int EdgesCount => 1;

    public (int, int) LocalEdge(int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0:
                return (0, 1);
            default:
                throw new Exception("wrong edge");
        }
    }

    public static double Xi(ReadOnlySpan<VectorT> vertices, VectorT point)
    {
        return VectorBase<double, VectorT>.Length(vertices[0], point) / (vertices[1] - vertices[0]).Norm();
    }

    public static VectorT LocarCoordinatesToGlobal(ReadOnlySpan<VectorT> vertices, double xi)
    {
        var h = vertices[1] - vertices[0];
        return h * xi + vertices[0];
    }

    public bool IsPointInElement(VectorT point, VectorT[] vertices)
    {
        throw new NotImplementedException();
    }

    public IFiniteElementGeometry<VectorT>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
         IsElementVertexNeeded = false;
        return [new Line<VectorT>([VertexNumber[0], EdgeVertices[0]]),
                new Line<VectorT>([EdgeVertices[0], VertexNumber[1]])];
    }

    public (int, int) GlobalEdge(int edgeNumber)
    {
        var local = LocalEdge(edgeNumber);
        return (VertexNumber[local.Item1], VertexNumber[local.Item2]);
    }
}
