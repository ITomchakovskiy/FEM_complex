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

    public double[][] CalcLocalMatrixForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta)
    {
        double BetaAvg = (Beta(vertices[0]) + Beta(vertices[1])) / 2.0;

        double h = geometry.Length(vertices);

        var localMatrix = EdgeLagrangianLinearLocalMatrices.GetMassMatrix();

        for (int i = 0; i < 2; ++i)
        {
            for(int j = 0; j <= i; ++j)
                localMatrix[i][j] *= BetaAvg * h;
        }
        return localMatrix;
    }

    public double[] CalcLocalRightPartForNeumannCondition(Vector2D[] vertices, Func<Vector2D, double> Theta)
    {
        double[] thetaValues = [Theta(vertices[0]), Theta(vertices[1])];
        double h = geometry.Length(vertices);

        var M = EdgeLagrangianLinearLocalMatrices.GetMassMatrix();

        double[] localRightPart = new double[2];

        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += M[i][j] * thetaValues[j];
            for (int j = i + 1; j < 2; ++j)
                localRightPart[i] += M[j][i] * thetaValues[j];
        }

        for(int i = 0; i < 2; ++i)
            localRightPart[i] *= h;

        return localRightPart;
    }

    public double[] CalcLocalRightPartForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta, Func<Vector2D, double> UBeta)
    {
        double[] uBetaValues = vertices.Select(v => UBeta(v)).ToArray();

        double h = geometry.Length(vertices);

        double BetaAvg = vertices.Select(v => Beta(v)).Sum() / 2d;

        var M = EdgeLagrangianLinearLocalMatrices.GetMassMatrix();

        double[] localRightPart = new double[2];

        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += M[i][j] * uBetaValues[j];
            for (int j = i + 1; j < 2; ++j)
                localRightPart[i] += M[j][i] * uBetaValues[j];
        }

        for (int i = 0; i < 2; ++i)
            localRightPart[i] *= h * BetaAvg;

        return localRightPart;
    }

    public double[] CalcLocalRightPartForDirichletCondition(Vector2D[] vertices, Func<Vector2D, double> Ug)
    {
        return vertices.Select(i => Ug(i)).ToArray();
    }
}
