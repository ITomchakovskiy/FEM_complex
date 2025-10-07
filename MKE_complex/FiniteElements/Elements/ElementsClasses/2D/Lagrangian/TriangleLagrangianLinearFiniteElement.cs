using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian;

[FiniteElementAttribute(GeometryType.Triangle,BasisType.Lagrangian,1)]
public class TriangleLagrangianLinearFiniteElement(string material, Triangle geometry) : IFiniteElement<Vector2D>
{
    public string Material { get; init; } = material;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    private Triangle geometry { get; init; } = geometry;
}
