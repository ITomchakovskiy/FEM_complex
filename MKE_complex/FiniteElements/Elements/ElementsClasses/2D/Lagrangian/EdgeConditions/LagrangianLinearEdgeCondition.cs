using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;

[FiniteElementAttribute(GeometryType.Line,BasisType.Lagrangian,1)]
public class LagrangianLinearEdgeCondition(string volume_material, string edge_material, Line geometry) : IBoundaryCondition<Vector2D>
{
    private Line geomerty { get; init; } = geometry;
    public FiniteElementGeometry.IFiniteElementGeometry<Vector2D> Geometry => geomerty;
    public string VolumeMaterial { get; init; } = volume_material;
    public string EdgeMaterial { get; init; } = edge_material;

    public int[] DOFs { get; private set; } = new int[2];

    public int DofsOnEdgeCount => 0;

    public int DofsOnVertexCount => 1;

    public int[] SortedDofs => throw new NotImplementedException();

    public int[] SortedDofIndices => throw new NotImplementedException();

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        DOFs[localVertexNumber] = dofNumber;
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if(dofsNumbers.Length != DOFs.Length) throw new ArgumentOutOfRangeException();
        DOFs = dofsNumbers.ToArray();
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber) { }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers) { }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }
}
