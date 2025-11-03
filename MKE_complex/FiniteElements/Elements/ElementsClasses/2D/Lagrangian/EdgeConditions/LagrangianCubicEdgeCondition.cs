using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;

[FiniteElementAttribute(GeometryType.Line, BasisType.Lagrangian, 3)]

public class LagrangianCubicEdgeCondition(string volume_material, string edge_material, Line geometry) : IBoundaryCondition<Vector2D>
{
    private Line geometry { get; } = geometry;
    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public string VolumeMaterial { get; } = volume_material;

    public string EdgeMaterial { get; } = edge_material;

    public int[] DOFs { get; private set; } = new int[4];

    public int DofsOnEdgeCount => 2;

    public int DofsOnVertexCount => 1;

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        DOFs[Geometry.VertexNumber.Length + localEdgeNumber] = dofNumber;
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if (dofsNumbers.Length != Geometry.EdgesCount * DofsOnEdgeCount) throw new ArgumentOutOfRangeException();
        for (int i = 0; i < dofsNumbers.Length; ++i)
            SetEdgeDofs(i, dofsNumbers[i]);
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if (dofsNumbers.Length != Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        for (int i = 0; i < dofsNumbers.Length; ++i)
            SetVertexDofs(i, dofsNumbers[i]);
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        if(localVertexNumber >= Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        DOFs[localVertexNumber] = dofNumber;
    }
}
