using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Lagrangian.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Lagrangian;
[FiniteElement(GeometryType.Tetrahedron,BasisType.Lagrangian)]
public class TetrahedronScalarLagrangianFiniteElement : IFiniteElement3D, IFiniteElementScalarEllipticProblemCalculation<Vector3D>
{
    public TetrahedronScalarLagrangianFiniteElement(string material, Tetrahedron geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        this.geometry = geometry;
        Order = order;
        DOFs = new int[10];         //hardcode

        sortedDofIndices = new Lazy<int[]>(()=>
        {
            var dofs = DOFs.ToArray();
            var indices = Enumerable.Range(0,DOFs.Length).ToArray();
            Array.Sort(dofs, indices);
            return indices;
        });
    }
    private Tetrahedron geometry;
    public IFiniteElementGeometry3D Geometry => geometry;
    public int DofsOnFaceCount => 0; //hardcode
    public int Order {get;}
    public string Material {get;}
    public int[] DOFs {get; private set;}
    public int[] SortedDofs => [.. SortedDofIndices.Select(i => DOFs[i])];
    private Lazy<int[]> sortedDofIndices;
    public int[] SortedDofIndices => sortedDofIndices.Value;
    public int DofsOnEdgeCount => 1;                   //hardcode
    public int DofsOnVertexCount => 1;              //hardcode;
    public int DofsOnElementCount => 0;             //hardcode
    IFiniteElementGeometry<Vector3D> IFiniteElement<Vector3D>.Geometry => Geometry;
    public bool IsDofsConnected(int dof1, int dof2)
    {
        return true;
    }
    public IFiniteElement<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var geometries = geometry.Refine(FaceVertices, EdgeVertices, ElementVertex,out IsElementVertexNeeded);

        return [.. geometries.Select(g => new TetrahedronScalarLagrangianFiniteElement(Material, (Tetrahedron)g, Order))];
    }
    public void SetEdgeDofs(int localEdgeNumber, int dofNumber) //everything about dofs is hardcode
    {
        DOFs[4 + localEdgeNumber] = dofNumber;
    }
    public void SetElementDofs(int startDofNumber)
    {
        ;
    }
    public void SetFaceDofs(int localFaceNumber, int[] baseVerices, int dofNumber)
    {
        ;
    }
    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        for(int i = 0; i < 4; ++i)
            DOFs[i] = dofsNumbers[i];
    }
    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        DOFs[localVertexNumber] = dofNumber;
    }

    public double[][] CalcLocalMatrix(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> Lambda, Func<Vector3D, double> Gamma)
    {
        //Functions are hardcode as constants
        var lambdaAvg = Lambda(new(0d,0d,0d));
        var gammaAvg = Gamma(new(0d,0d,0d));

        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        var Result = TetrahedronScalarLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(Order, AbsDetD, gammaAvg);

        var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);

        var StiffnessMatrix = TetrahedronScalarLagrangianCartesianLocalMatrices.CalculateLocalStiffnessMatrix(Order, Alpha, AbsDetD, lambdaAvg);

        for(int i = 0; i < Result.Length; ++i)
        {
            for(int j = 0; j < Result[i].Length; ++j)
                Result[i][j] += StiffnessMatrix[i][j];
        }

        return Result;
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

    public double[] CalcLocalRightPart(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, double> F)
    {
        var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);
        var verticesCopy = vertices.ToArray();
        var GlobalLagrangianVerticesAtDOFs = LocalLagrangianVerticesAtDofs().Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(verticesCopy, i));
        var weights = GlobalLagrangianVerticesAtDOFs.Select(F).ToArray();

        var matrix = TetrahedronScalarLagrangianCartesianLocalMatrices.CalculateLocalMassMatrix(Order, AbsDetD, 1d);

        double[] result = new double[matrix.Length];

        for(int i = 0; i < result.Length; ++i)
        {
            for(int j = 0; j < matrix[i].Length; ++j)
                result[i] += matrix[i][j] * weights[j];
            for(int j = matrix[i].Length; j < matrix.Length; ++j)
                result[i] += matrix[j][i] * weights[j];
        }

        return result;
    }

    public double CalcResultAtPoint(ReadOnlySpan<Vector3D> vertices, ReadOnlySpan<double> localSolution, Vector3D point)
    {
        var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);
        var localPoint = TetrahedronLocalCoordinates.LocalCoordinates.Select(i => i(point, Alpha)).ToArray();
        var basesValues = TetrahedronScalarLagrangianBases.BasisFunctions(Order).Select(f => f(localPoint)).ToArray();
        double res = 0d;
        for(int i = 0; i < basesValues.Length; ++i)
            res += basesValues[i] * localSolution[i];
        return res;
    }
}