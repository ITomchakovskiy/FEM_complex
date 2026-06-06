using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record TriangleBoundary(int[] VertexNumber) : Triangle<Vector3D>(VertexNumber), IFiniteElementGeometry3D
{
    public int FacesCount => 1;

    public int[] GlobalFace(int faceNumber)
    {
        return [.. LocalFace(faceNumber).Select(i => VertexNumber[i])];
    }

    public int[] LocalFace(int faceNumber)
    {
        return faceNumber switch
        {
            0 => [0, 1, 2],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override IFiniteElementGeometry<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var VertexNumbers = base.Refine(FaceVertices, EdgeVertices, ElementVertex, out IsElementVertexNeeded).Select(t => t.VertexNumber).ToArray();

        return [.. VertexNumbers.Select(i => new TriangleBoundary(i))];
    }
}