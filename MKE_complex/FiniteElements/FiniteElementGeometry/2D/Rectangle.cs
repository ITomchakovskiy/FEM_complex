using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;
public record Rectangle<VectorT>(int[] VertexNumber) : Quadrangle<VectorT>(VertexNumber) where VectorT : VectorBase<double, VectorT>
{
    public new GeometryType GeometryType => GeometryType.Rectangle;

    public new bool IsPointInElement(VectorT point, VectorT[] vertices)
    {
        if(point is Vector2D point2D && vertices is Vector2D[] vertices2D)
        {
            var LocalCoordinates = RectangleLocalCoordinates.XiEta(vertices2D, point2D);
            return LocalCoordinates.xi >= 0d && LocalCoordinates.xi <= 1d && LocalCoordinates.eta >= 0d && LocalCoordinates.eta <= 1d;
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public static int[] LocalEdgeNumToLocalEdgeNumForVectorHierarchicalBasis => [0, 3, 1, 2];

    public static VectorT CalcH(ReadOnlySpan<VectorT> vertices)
    {
        return vertices[2] - vertices[0];
    }
}