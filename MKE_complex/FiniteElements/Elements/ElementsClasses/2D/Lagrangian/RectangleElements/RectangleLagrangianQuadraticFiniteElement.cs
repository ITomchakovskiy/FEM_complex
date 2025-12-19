using MKE_complex.FiniteElements.Elements.BasisFunctions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.FiniteElements.Elements.LocalMatrices;
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
public class RectangleLagrangianQuadraticFiniteElement(string material, FiniteElementGeometry._2D.Rectangle geometry) : IFiniteElement<Vector2D>
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
        double LambdaAverage = vertices.Select(v => Lambda(v)).Average();

        double GammaAverage = vertices.Select(v => Gamma(v)).Average();

        var h = FiniteElementGeometry._2D.Rectangle.CalcH(vertices);

        var LocalStiffnessMatrix = RectangleLagrangianLocalMatrices.QuadraticMatrices.GetStiffnessMatrix(h.hx,h.hy);

        var LocalMassMatrix = RectangleLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(h.hx,h.hy);

        var result = new double[9][];

        for(int i = 0; i < 9; ++i)
        {
            result[i] = new double[i + 1];
            for (int j = 0; j <= i; ++j)
                result[i][j] = LambdaAverage * LocalStiffnessMatrix[i][j] + GammaAverage * LocalMassMatrix[i][j];
        }

        return result;
    }

    private Vector2D[] GetVerticesAtDofs(Vector2D[] vertices)
    {
        var result = new Vector2D[DOFs.Length];

        var A = vertices[0];

        //Vector2D[] verticesInDofsOrder = [vertices[0], vertices[3], vertices[1], vertices[2]];

        var h = FiniteElementGeometry._2D.Rectangle.CalcH(vertices);

        for(int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
                result[i * 3 + j] = new(A.X + h.hx / 2d * j, 
                                        A.Y + h.hy / 2d * i);
        }
        return result;
    }

    public double[] CalcLocalRightPart(Vector2D[] vertices, Func<Vector2D, double> F)
    {
        var verticesAtDofsFValues = GetVerticesAtDofs(vertices).Select(v => F(v)).ToArray();

        var h = FiniteElementGeometry._2D.Rectangle.CalcH(vertices);

        var LocalMassMatrix = RectangleLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(h.hx, h.hy);

        var result = new double[DOFs.Length];

        for(int i = 0; i < result.Length; ++i)
        {
            double rpElement = 0d;
            for (int j = 0; j <= i; ++j)
                rpElement += verticesAtDofsFValues[j] * LocalMassMatrix[i][j];
            for (int j = i + 1; j < result.Length; ++j)
                rpElement += verticesAtDofsFValues[j] * LocalMassMatrix[j][i];
            result[i] = rpElement;
        }

        return result;
    }

    public double CalcResultAtPoint(Vector2D[] vertices, ReadOnlySpan<double> localSolution, Vector2D point)
    {
        var localCoords = RectangleLagrangianBases.XiEta(vertices, point);

        double[] BasesValues = Enumerable.Range(0, DOFs.Length).
                                          Select(i => RectangleLagrangianBases.QuadraticBases.Psi(i, localCoords.xi,
                                                                                                     localCoords.eta)).ToArray();
        double result = 0d;

        for (int i = 0; i < DOFs.Length; ++i)
            result += BasesValues[i] * localSolution[i];
        return result;
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        return DOFs.Contains(dof1) && DOFs.Contains(dof2);
    }

    public IFiniteElement<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var geometries = geometry.Refine(FaceVertices, EdgeVertices, ElementVertex, out IsElementVertexNeeded);
        var refinedElements = geometries.Select(g => new RectangleLagrangianQuadraticFiniteElement(Material, (FiniteElementGeometry._2D.Rectangle)g));
        return refinedElements.ToArray();
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        switch(localEdgeNumber)
        {
            case 0:
                DOFs[3] = dofNumber;
                break;
            case 1:
                DOFs[7] = dofNumber;
                break;
            case 2:
                DOFs[5] = dofNumber;
                break;
            case 3:
                DOFs[1] = dofNumber;
                break;
        }
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if(dofsNumbers.Length != geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        for (int i = 0; i < dofsNumbers.Length; ++i) SetEdgeDofs(i, dofsNumbers[i]);
    }

    public void SetElementDofs(int startDofNumber)
    {
        DOFs[4] = startDofNumber;
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        if (dofsNumbers.Length != geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        for (int i = 0; i < dofsNumbers.Length; ++i) SetVertexDofs(i, dofsNumbers[i]);
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        if (localVertexNumber >= geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        switch(localVertexNumber)
        {
            case 0:
                DOFs[0] = dofNumber;
                break;
            case 1:
                DOFs[6] = dofNumber;
                break;
            case 2:
                DOFs[8] = dofNumber;
                break;
            case 3:
                DOFs[2] = dofNumber;
                break;  
        }
    }

    public IFiniteElement<Vector2D>[] Triangulate()
    {
        var geometries = geometry.Triangulate();
        var elements = geometries.Select(i => new TriangleLagrangianQuadraticFiniteElement(Material, (Triangle)i)).ToArray();
        int[] dofsEdge1 = [3, 4, 1]; dofsEdge1 = dofsEdge1.Select(i => DOFs[i]).ToArray();
        int[] dofsVertex1 = [0, 6, 2]; dofsVertex1 = dofsVertex1.Select(i => DOFs[i]).ToArray();
        elements[0].SetEdgesDofs(dofsEdge1);
        elements[0].SetVericesDofs(dofsVertex1);
        int[] dofsEdge2 = [7, 5, 4]; dofsEdge2 = dofsEdge2.Select(i => DOFs[i]).ToArray();
        int[] dofsVertex2 = [6, 8, 2]; dofsVertex2 = dofsVertex2.Select(i => DOFs[i]).ToArray();
        elements[1].SetEdgesDofs(dofsEdge2);
        elements[1].SetVericesDofs(dofsVertex2);
        return elements;
    }
}
