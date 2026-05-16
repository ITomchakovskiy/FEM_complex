using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Triangle<VectorT>(int[] VertexNumber) : IFiniteElementGeometry<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public GeometryType GeometryType => GeometryType.Triangle;

    public int EdgesCount => 3;

    public bool IsPointInElement(VectorT point, VectorT[] vertices)
    {
        if(point is Vector2D point2D && vertices is Vector2D[] vertices2D)
        {
            var alphas = TriangleLocalCoordinates.Alpha.CalcAlphas(vertices2D);
            var LocalCoordinates = TriangleLocalCoordinates.LocalCoordinates.Select(i => i(point2D, alphas));
            return LocalCoordinates.All(i => i >= -1E-15);
        }
        else
        {
            throw new ArgumentException("Method is used only for volume elements");
        }
    }

    public (int, int) LocalEdge(int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 0);
            default: throw new Exception("wrong edge number");
        }
    }

    public IFiniteElementGeometry<VectorT>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }

    public (int, int) GlobalEdge(int edgeNumber)
    {
        var local = LocalEdge(edgeNumber);
        return (VertexNumber[local.Item1], VertexNumber[local.Item2]);
    }
}
