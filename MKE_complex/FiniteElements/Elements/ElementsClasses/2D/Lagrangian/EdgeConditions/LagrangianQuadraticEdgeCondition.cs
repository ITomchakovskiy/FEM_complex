using MKE_complex.FiniteElements.Elements.LocalMatrices;
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
                int dofnum = DOFs[2 + j];
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        }

        return (x, y, DOFs.ToList());
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if (localEdgeNumber >= Geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        switch(localEdgeNumber)
        {
            case 0:
                DOFs[1] = dofNumber;
                break;
        }
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
        switch(localVertexNumber)
        {
            case 0:
                DOFs[0] = dofNumber;
                break;
            case 1:
                DOFs[2] = dofNumber;
                break;
        }
    }

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }

    public double[][] CalcLocalMatrixForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta)
    {
        double BetaAvg = (Beta(vertices[0]) + Beta(vertices[1])) / 2.0;

        double h = geometry.Length(vertices);

        var localMatrix = EdgeLagrangianQuadraticLocalMatrices.GetMassMatrix();

        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localMatrix[i][j] *= BetaAvg * h;
        }
        return localMatrix;
    }

    public double[] CalcLocalRightPartForNeumannCondition(Vector2D[] vertices, Func<Vector2D, double> Theta)
    {
        double[] thetaValues = [Theta(vertices[0]), Theta((vertices[0] + vertices[1])/2d), Theta(vertices[1])];
        double h = geometry.Length(vertices);

        var M = EdgeLagrangianQuadraticLocalMatrices.GetMassMatrix();

        double[] localRightPart = new double[3];

        for (int i = 0; i < localRightPart.Length; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += M[i][j] * thetaValues[j];
            for (int j = i + 1; j < localRightPart.Length; ++j)
                localRightPart[i] += M[j][i] * thetaValues[j];
        }

        for (int i = 0; i < localRightPart.Length; ++i)
            localRightPart[i] *= h;

        return localRightPart;
    }

    public double[] CalcLocalRightPartForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta, Func<Vector2D, double> UBeta)
    {
        Vector2D[] all_vertices = [vertices[0], (vertices[0] + vertices[1]) / 2d, vertices[1]];
        double[] uBetaValues = all_vertices.Select(v => UBeta(v)).ToArray();

        double h = geometry.Length(vertices);

        double BetaAvg = vertices.Select(v => Beta(v)).Sum() / 2d;

        var M = EdgeLagrangianQuadraticLocalMatrices.GetMassMatrix();

        double[] localRightPart = new double[3];

        for (int i = 0; i < localRightPart.Length; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += M[i][j] * uBetaValues[j];
            for (int j = i + 1; j < localRightPart.Length; ++j)
                localRightPart[i] += M[j][i] * uBetaValues[j];
        }

        for (int i = 0; i < localRightPart.Length; ++i)
            localRightPart[i] *= h * BetaAvg;

        return localRightPart;
    }

    public double[] CalcLocalRightPartForDirichletCondition(Vector2D[] vertices, Func<Vector2D, double> Ug)
    {
        Vector2D midPoint = (vertices[0] + vertices[1]) / 2d;
        return [Ug(vertices[0]), Ug(midPoint), Ug(vertices[1])];
    }


}
