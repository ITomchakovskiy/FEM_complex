using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;

[FiniteElementAttribute(GeometryType.Triangle,BasisType.Lagrangian,1)]
public class TriangleLagrangianLinearFiniteElement(string material, Triangle geometry) : IFiniteElement<Vector2D>
{
    public string Material { get; init; } = material;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public int[] DOFs { get; private set; } = new int[3];

    public int DofsOnEdgeCount => 0;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => 0;

    public int[] SortedDofs => throw new NotImplementedException();

    public int[] SortedDofIndices => throw new NotImplementedException();

    private Triangle geometry { get; init; } = geometry;

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber) { }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers) { }

    public void SetElementDofs(int startDofNumber) { }

    public void SetElementsDofs(int start_dof_number) { }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if (dofsNumbers.Length != DOFs.Length) throw new ArgumentException();
        DOFs = dofsNumbers.ToArray();
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        if (localVertexNumber > Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        else DOFs[localVertexNumber] = dofNumber;
    }
}
