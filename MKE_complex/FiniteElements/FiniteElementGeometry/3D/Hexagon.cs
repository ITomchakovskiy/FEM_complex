using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Hexagon(int[] VertexNumber) : IFiniteElementGeometry<Vector3D>
{
    public GeometryType GeometryType => GeometryType.Hexagon;

    public (int, int) Edge(int edgeNumber)
    {
        switch(edgeNumber)
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 3);
            case 3: return (3, 0);
            case 4: return (4, 5);
            case 5: return (5, 6);
            case 6: return (6, 7);
            case 7: return (7, 4);
            case 8: return (0, 4);
            case 9: return (1, 5);
            case 10: return (2, 6);
            case 11: return (3, 7);
            default: throw new Exception("Wrong edge number");
        }
    }
}
