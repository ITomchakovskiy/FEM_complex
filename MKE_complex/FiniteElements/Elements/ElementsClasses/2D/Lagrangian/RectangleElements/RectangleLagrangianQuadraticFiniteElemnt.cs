using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.RectangleElements;

[FiniteElementAttribute(GeometryType.Rectangle, BasisType.Lagrangian, 2)]
public class RectangleLagrangianQuadraticFiniteElemnt(string material, FiniteElementGeometry._2D.Rectangle geometry) : IFiniteElement<Vector2D>
{
    private FiniteElementGeometry._2D.Rectangle geometry = geometry;
    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public string Material { get; init; }  = material;

    public int[] DOFs { get; private set; } = new int[9];

    private int[]? sortedDofIndices;

    public int[] SortedDofIndices
    {
        get
        {
            if (sortedDofIndices != null) return sortedDofIndices;
            var dofs = new int[DOFs.Length];
            Array.Copy(DOFs, dofs, DOFs.Length);
            var indices = new int[DOFs.Length];
            for (int i = 0; i < DOFs.Length; ++i)
                indices[i] = i;
            Array.Sort(dofs, indices);
            sortedDofIndices = indices;
            return indices;
        }
    }

    public int[] SortedDofs => SortedDofIndices.Select(i => DOFs[i]).ToArray();

    public int DofsOnEdgeCount => 1;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => 1;

    public double[][] CalcLocalMatrix(Vector2D[] vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma)
    {
        throw new NotImplementedException();
    }

    public double[] CalcLocalRightPart(Vector2D[] vertices, Func<Vector2D, double> F)
    {
        throw new NotImplementedException();
    }

    public double CalcResultAtPoint(Vector2D[] vertices, ReadOnlySpan<double> localSolution, Vector2D point)
    {
        throw new NotImplementedException();
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }

    public IFiniteElement<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        throw new NotImplementedException();
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        throw new NotImplementedException();
    }

    public void SetElementDofs(int startDofNumber)
    {
        throw new NotImplementedException();
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        throw new NotImplementedException();
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        throw new NotImplementedException();
    }
}
