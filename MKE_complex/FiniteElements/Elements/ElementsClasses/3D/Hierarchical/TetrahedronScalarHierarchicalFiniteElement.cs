using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Hierarchical.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Hierarchical;
[FiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical)]
public class TetrahedronScalarHierarchicalFiniteElement : IFiniteElement3D, IFiniteElementScalarEllipticProblemCalculation<Vector3D>, IIntegrationElement<Vector3D>
{
    public TetrahedronScalarHierarchicalFiniteElement(string material, Tetrahedron geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        var SortedVertexNumber = geometry.VertexNumber.Order().ToArray();
        this.geometry = new Tetrahedron(SortedVertexNumber);
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

    public TetrahedronScalarHierarchicalFiniteElement(string material, Tetrahedron geometry, int order, int[] DOFs) : this(material, geometry, order)
    {
        this.DOFs = DOFs;
    }

    private static int CalcDofsCount(int order) => TetrahedronHierarchicalBases.CalcDofsCount(order);

    private static int NewDofsOnEdgesCountForOrder() => TetrahedronHierarchicalBases.NewDofsOnEdgesCountForOrder();
    private static int NewDofsOnFacesCountForOrder(int order) => TetrahedronHierarchicalBases.NewDofsOnFacesCountForOrder(order);
    private static int NewDofsOnElementCountForOrder(int order) => TetrahedronHierarchicalBases.NewDofsOnElementCountForOrder(order);

    private Tetrahedron geometry;
    public IFiniteElementGeometry3D Geometry => geometry;

    public int DofsOnFaceCount => (Order - 1) * (Order - 2) / 2;

    public int Order {get;}

    public string Material {get;}

    public int[] DOFs {get;}

    public int[] SortedDofs => [.. SortedDofIndices.Select(i => DOFs[i])];

    private Lazy<int[]> sortedDofIndices;

    public int[] SortedDofIndices => sortedDofIndices.Value;

    public int DofsOnEdgeCount => Order - 1;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => (Order - 1) * (Order - 2) * (Order - 3) / 6;

    IFiniteElementGeometry<Vector3D> IFiniteElement<Vector3D>.Geometry => Geometry;

    public bool IsDofsConnected(int dof1, int dof2)
    {
        if(DOFs.Contains(dof1) &&
           DOFs.Contains(dof2))
            return true;
        return false;
    }

    public IFiniteElement<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var geometries = Geometry.Refine(FaceVertices, EdgeVertices, ElementVertex, out IsElementVertexNeeded);

        return [.. geometries.Select(g => new TetrahedronScalarHierarchicalFiniteElement(Material, (Tetrahedron)g, Order))];
    }

    private int[] EdgeDofsShift()
    {
        return [.. Enumerable.Range(1, Order - 1).Select(CalcDofsCount)];
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(localEdgeNumber, geometry.EdgesCount);
        var shift = EdgeDofsShift();
        for(int i = 0; i < shift.Length; ++i)
            DOFs[shift[i] + localEdgeNumber] = dofNumber + i;
    }

    private int[] ElementDofsShift()
    {
        return [.. Enumerable.Range(3, Math.Max(Order - 3, 0)).Select(i => CalcDofsCount(i) + NewDofsOnEdgesCountForOrder() + NewDofsOnFacesCountForOrder(i+1))] ;
    }

    public void SetElementDofs(int startDofNumber)
    {
        var shift = ElementDofsShift();

        for(int i = 0; i < shift.Length; ++i)
        {
            var dofsCount = NewDofsOnElementCountForOrder(4 + i);
            for(int j = 0; j < dofsCount; ++j, ++startDofNumber)
                DOFs[shift[i] + j] = startDofNumber;
        }
    }

    private int[] FaceDofsShift()
    {
        return [.. Enumerable.Range(2, Math.Max(Order - 2, 0)).Select(i => CalcDofsCount(i) + NewDofsOnEdgesCountForOrder())] ;
    }

    public void SetFaceDofs(int localFaceNumber, int[] baseVerices, int dofNumber)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(localFaceNumber,geometry.FacesCount);

        var shift = FaceDofsShift();

        for(int i = 0; i < shift.Length; ++i)
        {
            int dofsCount = NewDofsOnFacesCountForOrder(i + 3)/geometry.FacesCount;
            for(int j = 0; j < dofsCount; ++j, ++dofNumber)
                DOFs[shift[i] + dofsCount * localFaceNumber + j] = dofNumber;
        }
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(dofsNumbers.Length, geometry.VertexNumber.Length);

        for(int i = 0; i < dofsNumbers.Length; ++i)
            SetVertexDofs(i, dofsNumbers[i]);
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(localVertexNumber, geometry.VertexNumber.Length);

        DOFs[localVertexNumber] = dofNumber;
    }

    public double[][] LocalLagrangianVerticesAtDofs()
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

        for(int faceNum = 0; faceNum < Geometry.FacesCount; ++faceNum) //faces dofs
        {
            var face = Geometry.LocalFace(faceNum);
            
            for(int i = 0; i < Order - 2; ++i) //MAYBE NEEDS TO BE CHANGED FOR HIGHER ORDERS
            {
                double coordinate3 = (double)(i + 1) / (double)Order;
                for(int j = 0; j < Order - 2 - i; ++j, ++dofnumber)
                {
                    var CoordinatesForDof = new double[Geometry.VertexNumber.Length];
                    CoordinatesForDof[face[0]] = (double)(Order - 2 - i - j) / (double)Order;
                    CoordinatesForDof[face[1]] = (double)(j + 1) / (double)Order;
                    CoordinatesForDof[face[2]] = coordinate3;

                    LocalCoordinates[dofnumber] = CoordinatesForDof;
                }
            }
        }

        for(int i = 0; i < Order - 3; ++i) //elements dofs
        {
            double coordinate4 = (double)(i + 1) / (double)Order;
            for(int j = 0; j < Order - 3 - i; ++j)
            {
                double coordinate3 = (double)(j + 1) / (double)Order;
                for(int k = 0; k < Order - 3 - i - j; ++k, ++dofnumber)
                {
                    double[] CoordinatesForDof = [
                                                (double)(Order - 3 - i - j - k) / (double)Order,
                                                (double)(k + 1) / (double)Order,
                                                coordinate3,
                                                coordinate4
                    ];

                    LocalCoordinates[dofnumber] = CoordinatesForDof;
                }
                
            }
        }

        return LocalCoordinates;
    }

    Vector3D[] GlobalLagrangianVerticesAtDofs(ReadOnlySpan<Vector3D> vertices)
    {
        Vector3D[] P1 = vertices.ToArray();
        Vector3D A = vertices[0], B = vertices[1], C = vertices[2], D = vertices[3];
        Vector3D[] P2 = [A,B,C, D ,(A + B)/2d,(A + C)/2d,(A + D)/2d,
                                   (B + C)/2d, (B + D)/2d, (D + C)/2d
                                    ];
        Vector3D[] P3 = [A,B,C, D, A + (B - A)/3d, A + 2d*(B - A)/3d,
                                   A + (C - A)/3d, A + 2d*(C - A)/3d,
                                   A + (D - A)/3d, A + 2d*(D - A)/3d,
                                   B + (C - B)/3d, B + 2d*(C - B)/3d,
                                   B + (D - B)/3d, B + 2d*(D - B)/3d,
                                   C + (D - C)/3d, C + 2d*(D - C)/3d,
                                   (A + B + C)/3d,
                                   (A + B + D)/3d,
                                   (A + C + D)/3d,
                                   (B + C + D)/3d,
                                  ];
        return Order switch
        {
            1 => P1,
            2 => P2,
            3 => P3,
            _ => throw new NotImplementedException()
        };
    }

    public double[][] CalcLocalMatrix(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> Lambda, Func<Vector3D, double> Gamma)
    {
        var verticesArray = vertices.ToArray();
        //var LagrangianVerticesAtDofs = LocalLagrangianVerticesAtDofs().Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(verticesArray,i));
        var LagrangianVerticesAtDofs = GlobalLagrangianVerticesAtDofs(vertices);
        var AvgLambda = LagrangianVerticesAtDofs.Average(Lambda);
        var AvgGamma = LagrangianVerticesAtDofs.Average(Gamma);

        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);

        var Result = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(Order, AbsDetD, AvgGamma, PolinomialType.Simple);

        var StiffnessMatrix = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalStiffnessMatrix(Order, Alpha, AbsDetD, AvgLambda, PolinomialType.Simple);

        for(int i = 0; i < Result.Length; ++i)
        {
            for(int j = 0; j < Result[i].Length; ++j)
                Result[i][j] += StiffnessMatrix[i][j];
        }
        return Result;
    }

    public double[] CalcLocalRightPart(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> F)
    {
        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var verticesArray = vertices.ToArray();
        //var weights = LocalLagrangianVerticesAtDofs().Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(verticesArray,i)).Select(F).ToArray();
        var weights = GlobalLagrangianVerticesAtDofs(vertices).Select(F).ToArray();
        var Hierarchical_LagrangianMassMatrix = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(Order, AbsDetD, PolinomialType.Simple);

        var Result = new double[DOFs.Length];

        for(int i = 0; i < Hierarchical_LagrangianMassMatrix.Length; ++i)
        {
            for(int j = 0; j < Hierarchical_LagrangianMassMatrix[i].Length; ++j)
                Result[i] += Hierarchical_LagrangianMassMatrix[i][j] * weights[j];
        }

        return Result;
    }

    public double CalcResultAtPoint(ReadOnlySpan<Vector3D> vertices, ReadOnlySpan<double> localSolution, Vector3D point)
    {
        var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);
        var LocalPoint = TetrahedronLocalCoordinates.LocalCoordinates.Select(i => i(point, Alpha)).ToArray();
        var BasisValues = TetrahedronHierarchicalBases.BasisFunctions(Order, PolinomialType.Simple).Select(f => f(LocalPoint)).ToArray();

        double Result = 0d;

        for(int i = 0; i < DOFs.Length; ++i)
            Result += BasisValues[i] * localSolution[i];
        
        return Result;
    }

    public double CalcResultAtPointLocal(ReadOnlySpan<double> localSolution, double[] pointL)
    {
        var BasisValues = TetrahedronHierarchicalBases.BasisFunctions(Order, PolinomialType.Simple).Select(f => f(pointL)).ToArray();

        double Result = 0d;

        for(int i = 0; i < DOFs.Length; ++i)
            Result += BasisValues[i] * localSolution[i];
        
        return Result;
    }

    public double IntegrateElement(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> F, int scheme)
    {
        var Quadrature = TetrahedronQuadratures.GetQuadrature(scheme);
        double res = 0d;
        for(int i = 0; i < Quadrature.Weights.Length; ++i)
        {
            var pointL = Quadrature.LocalPoints[i];
            var w = Quadrature.Weights[i];
            var pointG = TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, pointL);

            res += w * F(pointG);
        }
        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        return res * AbsDetD;
    }

    public double IntegrateDiscrepancy(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> F, ReadOnlySpan<double> localSolution, int scheme)
    {
        var Quadrature = TetrahedronQuadratures.GetQuadrature(scheme);
        double res = 0d;
        for(int i = 0; i < Quadrature.Weights.Length; ++i)
        {
            var pointL = Quadrature.LocalPoints[i];
            var w = Quadrature.Weights[i];
            var pointG = TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, pointL);

            var value = CalcResultAtPointLocal(localSolution, pointL);
            res += w * (F(pointG) - value) * (F(pointG) - value);
        }
        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        return res * AbsDetD;
    }
}
