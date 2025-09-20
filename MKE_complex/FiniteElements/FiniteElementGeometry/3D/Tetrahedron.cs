using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Tetrahedron(int[] VertexNumber) : IFiniteElementGeometry<Vector3D>
{
    public (int, int) Edge(int edgeNumber)
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
}
