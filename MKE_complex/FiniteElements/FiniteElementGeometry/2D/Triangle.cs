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

    public (int, int) Edge(int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 0);
            default: throw new Exception("wrong edge number");
        }
    }
}
