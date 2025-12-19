using MKE_complex.FiniteElements.Elements.BasisFunctions;
using MKE_complex.FiniteElements.Elements.LocalMatrices;
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

    public double[][] CalcLocalMatrix(Vector2D[] vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma)
    {
        var detD = Alpha.CalcDetD(vertices);

        var Alphas = Alpha.CalcAlphas(vertices);

        double[][] localMatrix = new double[6][];
        for (int i = 0; i < localMatrix.GetLength(0); ++i)
            localMatrix[i] = new double[i + 1];

        double[][] localStiffnessMatrix = TriangleLagrangianQuadraticLocalMatrices.GetStiffnessMatrix(Alphas);

        double[][] localMassMatrix = TriangleLagrangianQuadraticLocalMatrices.GetMassMatrix();

        double avgLambda = (Lambda(vertices[0]) +
                           Lambda(vertices[1]) +
                           Lambda(vertices[2])) / 3d;

        double avgGamma = (Gamma(vertices[0]) +
                           Gamma(vertices[1]) +
                           Gamma(vertices[2])) / 3d;
        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localMatrix[i][j] = Math.Abs(detD) * (avgLambda * localStiffnessMatrix[i][j] + avgGamma * localMassMatrix[i][j]);
        }

        return localMatrix;
    }

    public double[] CalcLocalRightPart(Vector2D[] vertices, Func<Vector2D, double> F)
    {
        double[][] localMassMatrix = TriangleLagrangianQuadraticLocalMatrices.GetMassMatrix();
        Vector2D[] all_vertices = [vertices[0], vertices[1], vertices[2], (vertices[0] + vertices[1])/2d, (vertices[1] + vertices[2]) / 2d, (vertices[2] + vertices[0]) / 2d];
        double[] f_values = all_vertices.Select(v => F(v)).ToArray();
        double[] localRightPart = new double[6];
        double detD = Math.Abs(Alpha.CalcDetD(vertices));
        for (int i = 0; i < localRightPart.Length; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += detD *localMassMatrix[i][j] * f_values[j];
            for (int j = i + 1; j < localRightPart.Length; ++j)
                localRightPart[i] += detD * localMassMatrix[j][i] * f_values[j];
        }

        return localRightPart;
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        return DOFs.Contains(dof1) && DOFs.Contains(dof2);
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
                Vector2D newVertex = (Vector2D)((A * (DofsOnEdgeCount - j) + B * (1 + j)) / 2d);
                int dofnum = DOFs[3 + i];
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        }

        return (x, y, DOFs.ToList());
    }

    public double CalcResultAtPoint(Vector2D[] vertices, ReadOnlySpan<double> localSolution, Vector2D point)
    {
        var alphas = Alpha.CalcAlphas(vertices);

        double[] L = TriangleLinearLagrangianBases.Psi.Select(psi => psi(point,alphas) ).ToArray();

        double[] BasesValues = TriangleQuadraticLagrangianBases.Psi.Select(psi => psi(L)).ToArray();

        double result = 0d;

        for (int i = 0; i < BasesValues.Length; ++i)
            result += BasesValues[i] * localSolution[i];
        return result;
    }

    public IFiniteElement<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var geometries = geometry.Refine(FaceVertices, EdgeVertices, ElementVertex, out IsElementVertexNeeded);
        var refinedElements = geometries.Select(g => new TriangleLagrangianQuadraticFiniteElement(Material,(Triangle)g));
        return refinedElements.ToArray();
    }

    public IFiniteElement<Vector2D>[] Triangulate()
    {
        return [this];
    }
}
