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
public class LagrangianLinearEdgeCondition(string volume_material, string edge_material, Line geometry) : IBoundaryCondition<Vector2D>
{
    private Line geomerty { get; init; } = geometry;
    public FiniteElementGeometry.IFiniteElementGeometry<Vector2D> Geometry => geomerty;
    public string VolumeMaterial { get; init; } = volume_material;
    public string EdgeMaterial { get; init; } = edge_material;
}
