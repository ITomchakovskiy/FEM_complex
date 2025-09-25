using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._2D;

public record Line(int[] VertexNumber) : IFiniteElementGeometry<Vector2D>
{
    public GeometryType GeometryType => GeometryType.Line;

    public (int, int) Edge(int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0:
                return (0, 1);
            default:
                throw new Exception("wrong edge");
        }
    }
}
