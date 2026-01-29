using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;

[FiniteElementAttribute(GeometryType.Triangle, BasisType.Lagrangian)]
public class TriangleLagrangianFiniteElement : IFiniteElement<Vector2D>
{
    public TriangleLagrangianFiniteElement(string material, Triangle geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        this.geometry = geometry;
        Order = order;

        DOFs = new int[DofsOnVertexCount * geometry.VertexNumber.Length + 
                       DofsOnEdgeCount * geometry.EdgesCount + 
                       DofsOnElementCount];
    }

    private Triangle geometry;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public int Order { get; }

    public string Material { get; }

    public int[] DOFs { get; private set; }

    public int DofsOnEdgeCount => Order - 1;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => (Order - 2) * (Order - 1) / 2;

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

    public bool IsDofsConnected(int dof1, int dof2)
    {
        if (DOFs.Contains(dof1) && DOFs.Contains(dof2)) return true;

        else return false;
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        var edge = Geometry.LocalEdge(localEdgeNumber);
        var edge_global = (Geometry.VertexNumber[edge.Item1], Geometry.VertexNumber[edge.Item2]);
        int increment = 1;
        if (edge_global.Item1 > edge_global.Item2)
        {
            dofNumber += DofsOnEdgeCount - 1;
            increment = -1;
        }
        for (int i = 0; i < DofsOnEdgeCount; ++i)
            DOFs[Geometry.VertexNumber.Length + localEdgeNumber * DofsOnEdgeCount + i] = dofNumber + increment * i;
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if(dofsNumbers.Length != Geometry.EdgesCount * DofsOnEdgeCount) throw new ArgumentOutOfRangeException();
        for(int i = 0; i < dofsNumbers.Length; ++i)
            SetEdgeDofs(i, dofsNumbers[i]);
    }

    public void SetElementDofs(int startDofNumber)
    {
        for(int i = 0; i < DofsOnElementCount; ++i)
            DOFs[Geometry.VertexNumber.Length * DofsOnVertexCount + Geometry.EdgesCount * DofsOnEdgeCount + i] = startDofNumber + i;
    }

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
                Vector2D newVertex = (A * (DofsOnEdgeCount - j) + B * (1 + j)) / (double)(DofsOnEdgeCount + 1);
                //int dofnum = DOFs[3 + i * 2 + j];
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        }

        Vector2D A_ = vertices[Geometry.VertexNumber[0]];
        Vector2D B_ = vertices[Geometry.VertexNumber[1]];
        Vector2D C_ = vertices[Geometry.VertexNumber[2]];

        for (int i = 0; i < Order - 2; ++i)
        {
            for (int j = 0; j < Order - 2 - i; ++j)
            {
                Vector2D newVertex = (A_ * (Order - 2 - i - j) + B_ * (j + 1) + C_ * (i + 1)) / (double)Order;
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        }
        

        return (x, y, DOFs.ToList());
    }
}
