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

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        var edge = Geometry.LocalEdge(localEdgeNumber);
        var edge_global = (Geometry.VertexNumber[edge.Item1], Geometry.VertexNumber[edge.Item2]);
        int increment = 1;
        if (edge_global.Item1 > edge_global.Item2)
        {
            ++dofNumber;
            increment = -1;
        }
        for (int i = 0; i < DofsOnEdgeCount; ++i)
            DOFs[Geometry.VertexNumber.Length + localEdgeNumber * DofsOnEdgeCount + i] = dofNumber + increment * i;
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

    public (List<double> x, List<double> y, List<int> dofs) ReturnDofs(ReadOnlySpan<Vector2D> vertices) //функция для вывода в файл дофов для отображения(только для тестов в лабе)
    {
        List<double> x = new();
        List<double> y = new();

        for (int i = 0; i < Geometry.VertexNumber.Length; ++i)
        {
            x.Add(vertices[Geometry.VertexNumber[i]].X);
            y.Add(vertices[Geometry.VertexNumber[i]].Y);
        }

        for (int i = 0; i < Geometry.EdgesCount; ++i)
        {
            Vector2D A = vertices[Geometry.VertexNumber[Geometry.LocalEdge(i).Item1]];
            Vector2D B = vertices[Geometry.VertexNumber[Geometry.LocalEdge(i).Item2]];
            for (int j = 0; j < DofsOnEdgeCount; ++j)
            {
                Vector2D newVertex = (Vector2D)((A * (DofsOnEdgeCount - j) + B * (1 + j)) / 3d);
                int dofnum = DOFs[2 + j];
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        }

        return (x, y, DOFs.ToList());
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        if (DOFs.Contains(dof1) && DOFs.Contains(dof2))
        {
            return true;
        }
        else return false;
    }
}
