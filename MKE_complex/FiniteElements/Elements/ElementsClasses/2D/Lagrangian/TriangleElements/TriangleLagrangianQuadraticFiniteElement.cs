using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;

[FiniteElementAttribute(GeometryType.Triangle, BasisType.Lagrangian, 2)]
public class TriangleLagrangianQuadraticFiniteElement(string material, Triangle geometry) : IFiniteElement<Vector2D>
{
    private Triangle geometry = geometry;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public string Material { get; } = material;

    public int[] DOFs { get; private set; } = new int[6];

    public int DofsOnEdgeCount => 1;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => 0;

    public int[] SortedDofs => throw new NotImplementedException();

    public int[] SortedDofIndices => throw new NotImplementedException();

    public double[,] CalcLocalMatrix(Vector2D[] vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma, Func<Vector2D, double> F)
    {
        throw new NotImplementedException();
    }

    public double[][] CalcLocalMatrix(Vector2D[] vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma)
    {
        throw new NotImplementedException();
    }

    public double[] CalcLocalRightPart(Vector2D[] vertices, Func<Vector2D, double> F)
    {
        throw new NotImplementedException();
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        DOFs[Geometry.VertexNumber.Length + localEdgeNumber] = dofNumber;
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if(dofsNumbers.Length != Geometry.EdgesCount * DofsOnEdgeCount) throw new ArgumentOutOfRangeException();
        for(int i = 0;i <dofsNumbers.Length;++i)
            SetEdgeDofs(i, dofsNumbers[i]);
    }

    public void SetElementDofs(int startDofNumber) { }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if(dofsNumbers.Length != Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        for (int i = 0; i < dofsNumbers.Length; ++i)
            SetVertexDofs(i, dofsNumbers[i]);
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        if(localVertexNumber >= Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        DOFs[localVertexNumber] = dofNumber;
    }
}
