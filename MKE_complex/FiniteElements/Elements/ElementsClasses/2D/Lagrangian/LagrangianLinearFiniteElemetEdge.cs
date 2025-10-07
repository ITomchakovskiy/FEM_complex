using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian;

[FiniteElementAttribute(GeometryType.Line,BasisType.Lagrangian,1)]
public class LagrangianLinearFiniteElemetEdge(string material, Line geometry) : IFiniteElementEdge<Vector2D>
{
    private Line geomerty { get; init; } = geometry;
    public FiniteElementGeometry.IFiniteElementGeometry<Vector2D> Geometry => geomerty;
    public string Material { get; init; } = material;
}
