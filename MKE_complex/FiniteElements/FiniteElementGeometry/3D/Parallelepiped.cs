using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Parallelepiped(int[] VertexNumber) : Hexahedron(VertexNumber)
{
    public new GeometryType GeometryType => GeometryType.Parallelepiped;

    public override bool IsPointInElement(Vector3D point, Vector3D[] vertices)
    {
        var LocalCoordinates = ParallelepipedLocalCoordinates.CalcLocalCoordinates(vertices, point);

        if(LocalCoordinates.xi >= 0 && LocalCoordinates.eta >=0 && LocalCoordinates.zeta >=0 &&
           LocalCoordinates.xi <= 1d && LocalCoordinates.eta <= 1d && LocalCoordinates.zeta <= 1d)
            return true;
        return false;
    }

    public static int[] LocalEdgeNumToLocalEdgeNumForVectorHierarchicalBasis => [0, 4, 5, 1, 8, 9, 10, 11, 2, 6, 7, 3];
    public static int[] LocalFaceNumToLocalFaceNumForVectorHierarchicalBasis => [4, 2, 0, 1, 3, 5];

    public static Vector3D CalcH(ReadOnlySpan<Vector3D> vertices)
    {
        return vertices[^1] - vertices[0];
    }
}
