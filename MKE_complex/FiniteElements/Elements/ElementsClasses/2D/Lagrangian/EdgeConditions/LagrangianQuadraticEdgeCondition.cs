using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;

[FiniteElementAttribute(GeometryType.Line, BasisType.Lagrangian, 2)]
public class LagrangianQuadraticEdgeCondition(string volume_material, string edge_material, Line geometry) : IBoundaryCondition<Vector2D>
{
    private Line geometry { get; init; } = geometry;
    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public string VolumeMaterial { get; init; } = volume_material;

    public string EdgeMaterial { get; init; } = edge_material;

    public int[] DOFs { get; private set; } = new int[3];

    public int DofsOnEdgeCount => 1;

    public int DofsOnVertexCount => 1;

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        var edge = Geometry.Edge(localEdgeNumber);
        var edge_global = (Geometry.VertexNumber[edge.Item1], Geometry.VertexNumber[edge.Item1]);
        int increment = 1;
        if (edge_global.Item1 > edge_global.Item2)
        {
            ++dofNumber;
            increment = -1;
        }
        for (int i = 0; i < DofsOnEdgeCount; ++i)
            DOFs[Geometry.VertexNumber.Length + localEdgeNumber + i] = dofNumber + increment * i;
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
        if (localVertexNumber >= Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        DOFs[localVertexNumber] = dofNumber;
    }
}
