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

    public int EdgesCount => 1;

    public (int, int) LocalEdge(int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0:
                return (0, 1);
            default:
                throw new Exception("wrong edge");
        }
    }

    public double Length(Vector2D[] vertices) => Math.Sqrt((vertices[1].X - vertices[0].X) *
                                                         (vertices[1].X - vertices[0].X) +
                                                         (vertices[1].Y - vertices[0].Y) *
                                                         (vertices[1].Y - vertices[0].Y));
}
