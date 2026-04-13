using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._1D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._1D.Lagrangian.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;

[FiniteElementAttribute(GeometryType.Line, BasisType.Lagrangian)]
public class LagrangianEdgeCondition(string material, Line<Vector2D> geometry, int order) : IBoundaryCondition<Vector2D>
{
    private Line<Vector2D> geometry { get; init; } = geometry;
    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public int Order { get; } = order;

    public string Material { get; init; } = material;

    public int[] DOFs { get; private set; } = new int[order + 1];

    public int DofsOnEdgeCount => Order - 1;

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
        var edge_global = (Geometry.VertexNumber[edge.Item1], Geometry.VertexNumber[edge.Item1]);
        int increment = 1;
        if (edge_global.Item1 > edge_global.Item2)
        {
            dofNumber += DofsOnEdgeCount -1;
            increment = -1;
        }
        for (int i = 0; i < DofsOnEdgeCount; ++i)
            DOFs[1 + i] = dofNumber + increment * i;
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
        //if (localVertexNumber >= Geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
        switch(localVertexNumber)
        {
            case 0:
                DOFs[0] = dofNumber;
                break;
            case 1:
                DOFs[^1] = dofNumber;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public (List<double> x, List<double> y, List<int> dofs) ReturnDofs(ReadOnlySpan<Vector2D> vertices) //функция для вывода в файл дофов для отображения(только для тестов в лабе)
    {
        List<double> x = new();
        List<double> y = new();

        x.Add(vertices[Geometry.VertexNumber[0]].X);
        y.Add(vertices[Geometry.VertexNumber[0]].Y);

        //for (int i = 0; i < Geometry.VertexNumber.Length; ++i)
        //{
        //    x.Add(vertices[Geometry.VertexNumber[i]].X);
        //    y.Add(vertices[Geometry.VertexNumber[i]].Y);
        //}

        //for (int i = 0; i < Geometry.EdgesCount; ++i)
        //{
            Vector2D A = vertices[Geometry.VertexNumber[Geometry.LocalEdge(0).Item1]];
            Vector2D B = vertices[Geometry.VertexNumber[Geometry.LocalEdge(0).Item2]];
            for (int j = 0; j < DofsOnEdgeCount; ++j)
            {
                Vector2D newVertex = (Vector2D)((A * (DofsOnEdgeCount - j) + B * (1 + j)) / (double)(DofsOnEdgeCount + 1));
                //int dofnum = DOFs[3 + i * 2 + j];
                x.Add(newVertex.X);
                y.Add(newVertex.Y);
            }
        //}

        x.Add(vertices[Geometry.VertexNumber[1]].X);
        y.Add(vertices[Geometry.VertexNumber[1]].Y);

        //Vector2D A_ = vertices[Geometry.VertexNumber[0]];
        //Vector2D B_ = vertices[Geometry.VertexNumber[1]];
        //Vector2D C_ = vertices[Geometry.VertexNumber[2]];

        //Vector2D newVertex_ = (Vector2D)((A_ + B_ + C_) / 3d);

        //x.Add(newVertex_.X);
        //y.Add(newVertex_.Y);

        return (x, y, DOFs.ToList());
    }

    private double[] GetLocalCoordinatesForDofs()
    {
        var LocalCoordinates = new double[DOFs.Length];
        LocalCoordinates[0] = 0d;
        LocalCoordinates[^1] = 1d;
        for(int i = 1; i < DOFs.Length - 1; ++i)
            LocalCoordinates[i] = (double)i/(double)Order;
        return LocalCoordinates;
    }

    public double[][] CalcLocalMatrixForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta)
    {
        var VerticesAtDofs = GetLocalCoordinatesForDofs().
                             Select(i => LineLocalCoordinates.LocalCoordinatesToGlobal(vertices,i));
        double AvgBeta = VerticesAtDofs.Select(i => Beta(i)).Average();
        double h = VectorBase<double, Vector2D>.Length(vertices[0], vertices[1]);

        return LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(Order, h, AvgBeta);
    }
    public double[] CalcLocalRightPartForNeumannCondition(Vector2D[] vertices, Func<Vector2D, double> Theta)
    {
        var VerticesAtDofs = GetLocalCoordinatesForDofs().
                             Select(i => LineLocalCoordinates.LocalCoordinatesToGlobal(vertices,i));
        var thetaValues = VerticesAtDofs.Select(i => Theta(i)).ToArray();
        double h = VectorBase<double, Vector2D>.Length(vertices[0], vertices[1]);

        var C = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(Order, h, 1d);

        var localRightPart = new double[DOFs.Length];

        for (int i = 0; i < localRightPart.Length; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += C[i][j] * thetaValues[j];
            for (int j = i + 1; j < localRightPart.Length; ++j)
                localRightPart[i] += C[j][i] * thetaValues[j];
        }

        return localRightPart;
    }
    public double[] CalcLocalRightPartForRobinCondition(Vector2D[] vertices, Func<Vector2D, double> Beta, Func<Vector2D, double> UBeta)
    {
        var VerticesAtDofs = GetLocalCoordinatesForDofs().
                             Select(i => LineLocalCoordinates.LocalCoordinatesToGlobal(vertices,i));
        var UBetaValues = VerticesAtDofs.Select(i => UBeta(i)).ToArray();
        double AvgBetta = VerticesAtDofs.Average(i => Beta(i));
        double h = VectorBase<double, Vector2D>.Length(vertices[0], vertices[1]);

        var M = LineLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(Order, h, AvgBetta);

        var localRightPart = new double[DOFs.Length];

        for (int i = 0; i < localRightPart.Length; ++i)
        {
            for (int j = 0; j <= i; ++j)
                localRightPart[i] += M[i][j] * UBetaValues[j];
            for (int j = i + 1; j < localRightPart.Length; ++j)
                localRightPart[i] += M[j][i] * UBetaValues[j];
        }

        return localRightPart;
    }
    public double[] CalcLocalRightPartForDirichletCondition(Vector2D[] vertices, Func<Vector2D, double> Ug)
    {
        var VerticesAtDofs = GetLocalCoordinatesForDofs().
                             Select(i => LineLocalCoordinates.LocalCoordinatesToGlobal(vertices,i));
        return VerticesAtDofs.Select(i => Ug(i)).ToArray();
    }

    IBoundaryCondition<Vector2D>[] IBoundaryCondition<Vector2D>.Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices)
    {
        var refinedGeometry = geometry.Refine(FaceVertices, EdgeVertices, -1, out _ );
        return refinedGeometry.Select(g => new LagrangianEdgeCondition(Material, (Line<Vector2D>)g, Order)).ToArray();
    }
}
