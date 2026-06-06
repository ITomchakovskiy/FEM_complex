using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Hierarchical;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Hierarchical.Cartesian;
using MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Lagrangian.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Hierarchical;

[FiniteElement(GeometryType.TriangleBoundary, BasisType.Hierarchical)]
public class TriangleHierarchicalBoundaryCondition : IBoundaryCondition3D, IBoundaryConditionScalarHierarchicalEllipticProblemCalculation<Vector3D>
{
    public TriangleHierarchicalBoundaryCondition(string material, Triangle<Vector3D> geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        var SortedVertexNumber = geometry.VertexNumber.Order().ToArray();
        this.geometry = new TriangleBoundary(SortedVertexNumber);
        Order = order;
        DOFs = new int[CalcDofsCount(Order)];

        sortedDofIndices = new Lazy<int[]>(()=>
        {
            var dofs = DOFs.ToArray();
            var indices = Enumerable.Range(0,DOFs.Length).ToArray();
            Array.Sort(dofs, indices);
            return indices;
        });
    }

    public TriangleHierarchicalBoundaryCondition(string material, Triangle<Vector3D> geometry, int order, int[] DOFs) : this(material, geometry, order)
    {
        this.DOFs = DOFs;
    }

    private static int CalcDofsCount(int order) => TriangleHierarchicalBases.CalcDofsCount(order);
    private static int NewDofsOnEdgesCountForOrder() => TriangleHierarchicalBases.NewDofsOnEdgesCountForOrder();
    private static int NewDofsOnElementCountForOrder(int order) => TriangleHierarchicalBases.NewDofsOnElementCountForOrder(order);

    private TriangleBoundary geometry;
    public IFiniteElementGeometry3D Geometry => geometry;

    public int DofsOnFaceCount => (Order - 1) * (Order - 2) / 2;

    public string Material {get;}

    public int Order {get;}

    public int[] DOFs {get;}

    public int[] SortedDofs => [.. SortedDofIndices.Select(i => DOFs[i])];

    private Lazy<int[]> sortedDofIndices;
    public int[] SortedDofIndices => sortedDofIndices.Value;

    public int DofsOnEdgeCount => Order - 1;

    public int DofsOnVertexCount => 1;

    IFiniteElementGeometry<Vector3D> IBoundaryCondition<Vector3D>.Geometry => Geometry;

    public IBoundaryCondition<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices)
    {
        var geometries = Geometry.Refine(FaceVertices, EdgeVertices, 0, out bool IsElementVertexNeeded);

        return [.. geometries.Select(g => new TriangleHierarchicalBoundaryCondition(Material, (TriangleBoundary)g, Order))];
    }

    private int[] EdgeDofShift()
    {
        int N = DofsOnEdgeCount;

        int[] shift = new int[N];

        for(int iOrder = 1; iOrder < Order; ++iOrder)
            shift[iOrder - 1] = CalcDofsCount(iOrder);
        
        return shift;
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        var shift = EdgeDofShift();

        for(int i = 0; i < shift.Length; ++i)
            DOFs[shift[i] + localEdgeNumber] = dofNumber + i;
    }

    private int[] FaceDofsShift()
    {
        int N = Order - 2;

        var shift = new int[Math.Max(N,0)];

        for(int i = 2; i < Order; ++i)
            shift[i-2] = CalcDofsCount(i) + NewDofsOnEdgesCountForOrder();

        return shift;
    }

    public void SetFaceDofs(int[] baseVerices, int dofNumber)
    {
        var shift = FaceDofsShift();

        int localDofNum = dofNumber;
        for(int i = 0; i < shift.Length; ++i)
        {
            for(int j = 0; j < NewDofsOnElementCountForOrder(i+3) ; ++j, ++localDofNum)
                DOFs[shift[i] + j] = localDofNum;
        }
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(dofsNumbers.Length, geometry.VertexNumber.Length);
        for(int i = 0; i < dofsNumbers.Length; ++i)
            SetVertexDofs(i, dofsNumbers[i]);
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(localVertexNumber, geometry.VertexNumber.Length);
        
        DOFs[localVertexNumber] = dofNumber;
    }

    public double[][] CalcLocalMatrixForRobinCondition(Vector3D[] vertices, Func<Vector3D, double> Beta)
    {
        var AvgBeta = LagrangianVerticesAtDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices,i)).Average(Beta);

        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        return TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(Order,AbsDetD,AvgBeta, PolinomialType.Simple);
    }

    public double[] CalcLocalRightPartForNeumannCondition(Vector3D[] vertices, Func<Vector3D, double> Theta)
    {
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var Hierarchical_LagrangianMassMatrix = TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(Order,AbsDetD,PolinomialType.Simple);
        var weights = LagrangianVerticesAtDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices,i)).Select(i => Theta(i)).ToArray();

        var result = new double[Hierarchical_LagrangianMassMatrix.Length];

        for(int i = 0; i < result.Length; ++i)
        {
            for(int j = 0; j < Hierarchical_LagrangianMassMatrix[i].Length; ++j)
                result[i] += weights[j] * Hierarchical_LagrangianMassMatrix[i][j];
        }

        return result;
    }

    public double[] CalcLocalRightPartForRobinCondition(Vector3D[] vertices, Func<Vector3D, double> Beta, Func<Vector3D, double> UBeta)
    {
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var Hierarchical_LagrangianMassMatrix = TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(Order,AbsDetD,PolinomialType.Simple);
        var weights = LagrangianVerticesAtDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices,i)).Select(i => UBeta(i)*Beta(i)).ToArray();

        var result = new double[Hierarchical_LagrangianMassMatrix.Length];

        for(int i = 0; i < result.Length; ++i)
        {
            for(int j = 0; j < Hierarchical_LagrangianMassMatrix[i].Length; ++j)
                result[i] += weights[j] * Hierarchical_LagrangianMassMatrix[i][j];
        }

        return result;
    }

    public double[][] LagrangianVerticesAtDofs()
    {
        double[][] LocalCoordinates = new double[DOFs.Length][];

        int dofnumber = 0;

        for(; dofnumber < Geometry.VertexNumber.Length; ++dofnumber) //vertices dofs
        {
            LocalCoordinates[dofnumber] = new double[Geometry.VertexNumber.Length];
            LocalCoordinates[dofnumber][dofnumber] = 1d;
        }

        for(int i = 0; i < Geometry.EdgesCount; ++i) //edges dofs
        {
            var LocalEdge = Geometry.LocalEdge(i);
            for(int j = 0; j < DofsOnEdgeCount; ++j, ++dofnumber)
            {
                var CoordinatesForDof = new double[Geometry.VertexNumber.Length];

                CoordinatesForDof[LocalEdge.Item1] = (double)(DofsOnEdgeCount - j) / (double)(DofsOnEdgeCount + 1);
                CoordinatesForDof[LocalEdge.Item2] = (double)(j + 1) / (double)(DofsOnEdgeCount + 1);

                LocalCoordinates[dofnumber] = CoordinatesForDof;
            }
        }

        for(int i = 0; i < Order - 2; ++i) //elements dofs
        {
            double coordinate3 = (double)(i + 1) / (double)Order;
            for(int j = 0; j < Order - 2 - i; ++j, ++dofnumber)
            {
                double[] CoordinatesForDof = [
                                                (double)(Order - 2 - i - j) / (double)Order,
                                                (double)(j + 1) / (double)Order,
                                                coordinate3
                ];

                LocalCoordinates[dofnumber] = CoordinatesForDof;
            }
        }

        return LocalCoordinates;
    }

    public double[] CalcLocalRightPartForDirichletCondition(Vector3D[] vertices, Func<Vector3D, double> Ug)
    {
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var Hierarchical_LagrangianMassMatrix = TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(Order,AbsDetD,PolinomialType.Simple);
        var GlobalLagrangianVerticesAtDOFs = LagrangianVerticesAtDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices,i));
        var weights = GlobalLagrangianVerticesAtDOFs.Select(Ug).ToArray();

        var result = new double[Hierarchical_LagrangianMassMatrix.Length];

        for(int i = 0; i < result.Length; ++i)
        {
            for(int j = 0; j < Hierarchical_LagrangianMassMatrix[i].Length; ++j)
                result[i] += weights[j] * Hierarchical_LagrangianMassMatrix[i][j];
        }

        return result;
    }

    public double[][] CalcLocalMatrixForDirichletCondition(Vector3D[] vertices)
    {
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        return TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(Order,AbsDetD,1d, PolinomialType.Simple);
    }
}