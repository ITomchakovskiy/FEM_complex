using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Rectangle(int[] VertexNumber) : Quadrangle(VertexNumber)
{
    public new GeometryType GeometryType => GeometryType.Rectangle;

    public new bool IsPointInElement(Vector2D point, Vector2D[] vertices)
    {
        double minX = vertices.Min(v => v.X);
        double maxX = vertices.Max(v => v.X);
        double minY = vertices.Min(v => v.Y);
        double maxY = vertices.Max(v => v.Y);
        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

    public new IFiniteElementGeometry<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        IsElementVertexNeeded = true;
        return [new Rectangle([VertexNumber[0], EdgeVertices[0], ElementVertex, EdgeVertices[3]]),
                new Rectangle([EdgeVertices[0], VertexNumber[1], EdgeVertices[1], ElementVertex]),
                new Rectangle([ElementVertex, EdgeVertices[1], VertexNumber[2], EdgeVertices[2]]),
                new Rectangle([EdgeVertices[3], ElementVertex, EdgeVertices[2], VertexNumber[3]]),];
    }

    public static (double hx, double hy) CalcH(ReadOnlySpan<Vector2D> vertices)
    {
        double hx = Math.Abs(vertices[0].X - vertices[2].X);
        double hy = Math.Abs(vertices[0].Y - vertices[2].Y); 
        return (hx, hy);
    }
}
