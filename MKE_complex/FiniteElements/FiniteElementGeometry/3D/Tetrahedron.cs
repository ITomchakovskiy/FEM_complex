using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Tetrahedron(int[] VertexNumber) : IFiniteElementGeometry<Vector3D>
{
    public GeometryType GeometryType => GeometryType.Tetrahedron;

    public int EdgesCount => throw new NotImplementedException();

    public bool IsPointInElement(Vector3D point, Vector3D[] vertices)
    {
        throw new NotImplementedException();
    }

    public (int, int) LocalEdge(int edgeNumber)
    {
        switch(edgeNumber)
        {
            case 0: return (0, 1);
                case 1: return (1, 2);
                case 2: return (2, 0);
                case 3: return (0, 3);
                case 4: return (1, 3);
                case 5 : return (2, 3);
            default: throw new Exception("Wrong edge number");
        }
    }

    public IFiniteElementGeometry<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }

    public (int, int) GlobalEdge(int edgeNumber)
    {
        var local = LocalEdge(edgeNumber);
        return (VertexNumber[local.Item1], VertexNumber[local.Item2]);
    }
}
