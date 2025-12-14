using MKE_complex.FiniteElements.Elements.BasisFunctions;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Triangle(int[] VertexNumber) : IFiniteElementGeometry<Vector2D>
{
    public GeometryType GeometryType => GeometryType.Triangle;

    public int EdgesCount => 3;

    public Vector2D CalculateCenterVertex(ReadOnlySpan<Vector2D> vertices)
    {
        var center = new Vector2D(0d, 0d);
        for (int i = 0; i < vertices.Length; i++)
            center += vertices[i];
        center /= vertices.Length;

        return center;
    }

    public bool IsPointInElement(Vector2D point, Vector2D[] vertices)
    {
        Vector2D[][] verticesSet = [vertices, [vertices[0], vertices[1], point], [vertices[1], vertices[2], point], [vertices[2], vertices[0], point]];
        double double_area1 = Math.Abs(Alpha.CalcDetD(verticesSet[0]));
        double double_area2 = Math.Abs(Alpha.CalcDetD(verticesSet[1])) + Math.Abs(Alpha.CalcDetD(verticesSet[2])) + Math.Abs(Alpha.CalcDetD(verticesSet[3]));
        if(double_area2 - double_area1 < 1e-15)
            return true;
        return false;
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

    
    public IFiniteElementGeometry<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        IsElementVertexNeeded = false;
        return [new Triangle([VertexNumber[0], EdgeVertices[0], EdgeVertices[2]]),
                new Triangle([EdgeVertices[2], EdgeVertices[1], VertexNumber[2]]),
                new Triangle([EdgeVertices[0], EdgeVertices[1], EdgeVertices[2]]),
                new Triangle([EdgeVertices[0], VertexNumber[1], EdgeVertices[1]])];
    }
}
