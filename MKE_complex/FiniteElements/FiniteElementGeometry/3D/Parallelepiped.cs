using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Parallelepiped(int[] VertexNumber) : Hexahedron(VertexNumber)
{
    public new GeometryType GeometryType => GeometryType.Parallelepiped;

    public new bool IsPointInElement(Vector3D point, Vector3D[] vertices)
    {
        Vector3D A = vertices[0];
        Vector3D B = vertices[^1];
        
        throw new NotImplementedException();
    }

    public static int[] LocalEdgeNumToLocalEdgeNumForVectorHierarchicalBasis => [0, 4, 5, 1, 8, 9, 10, 11, 2, 6, 7, 3];
    public static int[] LocalFaceNumToLocalFaceNumForVectorHierarchicalBasis => [4, 2, 0, 1, 3, 5];

    public static Vector3D CalcH(ReadOnlySpan<Vector3D> vertices)
    {
        return vertices[^1] - vertices[0];
    }
}
