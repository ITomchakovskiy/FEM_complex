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

[FiniteElementAttribute(GeometryType.Triangle,BasisType.Lagrangian,1)]
public class TriangleLagrangianLinearFiniteElement(string material, Triangle geometry) : IFiniteElement<Vector2D>
{
    public string Material { get; init; } = material;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public int[] DOFs { get; private set; } = new int[3];

    public int DofsOnEdgeCount => 0;

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

    private Triangle geometry { get; init; } = geometry;

    public bool IsDofsConnected(int dof1, int dof2)
    {
        if (DOFs.Contains(dof1) && DOFs.Contains(dof2))
        {
            return true;
        }
        else return false;
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

    public (List<double> x, List<double> y, List<int> dofs) ReturnDofs(ReadOnlySpan<Vector2D> vertices) //функция для вывода в файл дофов для отображения(только для тестов в лабе)
    {
        List<double> x = new();
        List<double> y = new();

        for (int i = 0; i < Geometry.VertexNumber.Length; ++i)
        {
            x.Add(vertices[Geometry.VertexNumber[i]].X);
            y.Add(vertices[Geometry.VertexNumber[i]].Y);
        }

        return (x, y, DOFs.ToList());
    }

    public double[][] CalcLocalMatrix(Vector2D[] vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma)
    {
        var detD = Alpha.CalcDetD(vertices);

        var Alphas = Alpha.CalcAlphas(vertices);

        double[][] localMatrix = new double[3][];
        for (int i = 0; i < 3; ++i)
            localMatrix[i] = new double[i + 1];

        double[][] localStiffnessMatrix = TriangleLagrangianLinearLocalMatrices.GetStiffnessMatrix(Alphas);

        double[][] localMassMatrix = TriangleLagrangianLinearLocalMatrices.GetMassMatrix();

        double avgLambda = (Lambda(vertices[0]) +
                           Lambda(vertices[1]) +
                           Lambda(vertices[2])) / 3d;

        double avgGamma = (Gamma(vertices[0]) +
                           Gamma(vertices[1]) +
                           Gamma(vertices[2])) / 3d;
        for (int i = 0; i < 3; ++i)
        {
            for(int j = 0; j <= i; ++j)
                localMatrix[i][j] = Math.Abs(detD) * (avgLambda * localStiffnessMatrix[i][j] + avgGamma * localMassMatrix[i][j]);
        }

        return localMatrix;
    }

    public double[] CalcLocalRightPart(Vector2D[] vertices, Func<Vector2D, double> F)
    {
        double[][] localMassMatrix = TriangleLagrangianLinearLocalMatrices.GetMassMatrix();
        double detd = Math.Abs(Alpha.CalcDetD(vertices));
        double[] f_values = vertices.Select(v => F(v)).ToArray();
        double[] localRightPart = new double[3];
        for(int i = 0; i < 3; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += detd * localMassMatrix[i][j] * f_values[j];
            for(int j = i + 1; j < 3; ++j)
                localRightPart[i] += detd * localMassMatrix[j][i] * f_values[j];
        }

        return localRightPart;
    }

    public double CalcResultAtPoint(Vector2D[] vertices, ReadOnlySpan<double> localSolution, Vector2D point)
    {
        throw new NotImplementedException();
    }

    public IFiniteElement<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }
}
