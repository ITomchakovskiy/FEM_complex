using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Hexahedron(int[] VertexNumber) : IFiniteElementGeometry3D
{
    public GeometryType GeometryType => GeometryType.Hexahedron;

    public int EdgesCount => 12;

    public int FacesCount => 6;

    public (int, int) LocalEdge(int edgeNumber) //
    {
        return edgeNumber switch
        {
            0 => (0,1),
            1 => (0,2),
            2 => (1,3),
            3 => (2,3),
            4 => (0,4),
            5 => (1,5),
            6 => (2,6),
            7 => (3,7),
            8 => (4,5),
            9 => (4,6),
            10 => (5,7),
            11 => (6,7),
            _ => throw new ArgumentException("wrong edge number")
        };
    }

    public bool IsPointInElement(Vector3D point, Vector3D[] vertices)
    {
        throw new NotImplementedException();
    }

    public IFiniteElementGeometry<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }

    public int[] LocalFace(int faceNumber)
    {
        return faceNumber switch
        {
            0 => [0,2,3,1],
            1 => [0,1,5,4],
            2 => [0,4,6,2],
            3 => [1,3,7,5],
            4 => [2,6,7,3],
            5 => [4,5,7,6],
            _ => throw new ArgumentException("wrong face number")
        };
    }

    public static (Vector3D A, Vector3D B) OpposingVertices(ReadOnlySpan<Vector3D> vertices) =>
        (vertices[0], vertices[^1]);

    public (int, int) GlobalEdge(int edgeNumber)
    {
        var local = LocalEdge(edgeNumber);
        return (VertexNumber[local.Item1], VertexNumber[local.Item2]);
    }

    public int[] GlobalFace(int faceNumber)
    {
        return LocalFace(faceNumber).Select(i => VertexNumber[i]).ToArray();
    }
}
