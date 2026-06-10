using MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Lagrangian.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;

[FiniteElementAttribute(GeometryType.Triangle, BasisType.Lagrangian)]
public class TriangleLagrangianFiniteElement : IFiniteElement<Vector2D>, IFiniteElementScalarEllipticProblemCalculation<Vector2D>, IIntegrationElement<Vector2D>
{
    public TriangleLagrangianFiniteElement(string material, Triangle<Vector2D> geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        this.geometry = geometry;
        Order = order;

        DOFs = new int[DofsOnVertexCount * geometry.VertexNumber.Length + 
                       DofsOnEdgeCount * geometry.EdgesCount + 
                       DofsOnElementCount];
        sortedDofIndices = new Lazy<int[]>(()=>
        {
            var dofs = DOFs.ToArray();
            var indices = Enumerable.Range(0,DOFs.Length).ToArray();
            Array.Sort(dofs, indices);
            return indices;
        });
    }

    private Triangle<Vector2D> geometry;

    public IFiniteElementGeometry<Vector2D> Geometry => geometry;

    public int Order { get; }

    public string Material { get; }

    public int[] DOFs { get; private set; }

    public int DofsOnEdgeCount => Order - 1;

    public int DofsOnVertexCount => 1;

    public int DofsOnElementCount => (Order - 2) * (Order - 1) / 2;

    private Lazy<int[]> sortedDofIndices;
    
    public int[] SortedDofIndices => sortedDofIndices.Value;

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

    private double[][] GetLocalCoordinatesForDofs()
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

    public double[][] CalcLocalMatrix(ReadOnlySpan<Vector2D> vertices, Func<Vector2D, double> Lambda, Func<Vector2D, double> Gamma)
    {
        var Alpha = TriangleLocalCoordinates.Alpha.CalcAlphas(vertices);

        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        var verticesArray = vertices.ToArray();

        var VerticesAtDofs = GetLocalCoordinatesForDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(verticesArray, i)).ToArray();

        double LambdaAvg = VerticesAtDofs.Average(i => Lambda(i));
        double GammaAvg = VerticesAtDofs.Average(i => Gamma(i));

        var LocalStiffnessMatrix = TriangleLagrangianCartesianLocalMatrices.
                                    CalculateLocalStiffnessMatrix(Order, Alpha, AbsDetD, LambdaAvg);

        var LocalMassMatrix = TriangleLagrangianCartesianLocalMatrices.
                              CalculateLocalMassMatrix(Order, Alpha, AbsDetD, GammaAvg);

        var result = LocalStiffnessMatrix;

        for(int i = 0; i < DOFs.Length; ++i)
        {
            for(int j = 0; j < LocalMassMatrix[i].Length; ++j)
                result[i][j] += LocalMassMatrix[i][j];
        }
                                  
        return result;
    }

    public double[] CalcLocalRightPart(ReadOnlySpan<Vector2D> vertices, Func<Vector2D, double> F)
    {
        var Alpha = TriangleLocalCoordinates.Alpha.CalcAlphas(vertices);

        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        var verticesArray = vertices.ToArray();

        var VerticesAtDofs = GetLocalCoordinatesForDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(verticesArray, i)).ToArray();

        double[] FValuesAtDofs = VerticesAtDofs.Select(i => F(i)).ToArray();

        var LocalMassMatrix = TriangleLagrangianCartesianLocalMatrices.
                              CalculateLocalMassMatrix(Order, Alpha, AbsDetD, 1d);

        var result = new double[DOFs.Length];

        for(int i = 0; i < DOFs.Length; ++i)
        {
            for(int j = 0; j < LocalMassMatrix[i].Length; ++j)
                result[i] += LocalMassMatrix[i][j] * FValuesAtDofs[j];
            for(int j = LocalMassMatrix[i].Length; j < DOFs.Length; ++j)
                result[i] += LocalMassMatrix[j][i] * FValuesAtDofs[j];
        }
                    
        return result;
    }

    public double CalcResultAtPoint(ReadOnlySpan<Vector2D> vertices, ReadOnlySpan<double> localSolution, Vector2D point)
    {
        var Alpha = TriangleLocalCoordinates.Alpha.CalcAlphas(vertices);

        var LocalCoordinates = TriangleLocalCoordinates.LocalCoordinates.Select(i => i(point, Alpha)).ToArray();
         
        var basisFunctionValues = TriangleLagrangianBases.BasisFunctions(Order).Select(i => i(LocalCoordinates)).ToArray();

        double result = 0d;

        for(int i = 0; i < DOFs.Length; ++i)
            result += localSolution[i] * basisFunctionValues[i];
        return result;
    }

    public double CalcResultAtPointLocal(ReadOnlySpan<double> localSolution, double[] PointL)
    {
        //var Alpha = TriangleLocalCoordinates.Alpha.CalcAlphas(vertices);

        //var LocalCoordinates = TriangleLocalCoordinates.LocalCoordinates.Select(i => i(point, Alpha)).ToArray();
         
        var basisFunctionValues = TriangleLagrangianBases.BasisFunctions(Order).Select(i => i(PointL)).ToArray();

        double result = 0d;

        for(int i = 0; i < DOFs.Length; ++i)
            result += localSolution[i] * basisFunctionValues[i];
        return result;
    }


    public IFiniteElement<Vector2D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        var geometries = geometry.Refine(FaceVertices, EdgeVertices, ElementVertex, out IsElementVertexNeeded);
        return [.. geometries.Select(i => new TriangleLagrangianFiniteElement(Material, (Triangle<Vector2D>)i, Order))];
    }

    public double IntegrateElement(ReadOnlySpan<Vector2D> vertices, Func<Vector2D, double> F, int scheme)
    {
        var Quadrature = TriangleQuadratures.GetQuadrature(scheme);
        double res = 0d;
        for(int i = 0; i < Quadrature.Weights.Length; ++i)
        {
            var pointL = Quadrature.LocalPoints[i];
            var w = Quadrature.Weights[i];
            var pointG = TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices, pointL);

            res += w * F(pointG);
        }
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        return res * AbsDetD;
    }

    public double IntegrateDiscrepancy(ReadOnlySpan<Vector2D> vertices, Func<Vector2D, double> F, ReadOnlySpan<double> localSolution, int scheme)
    {
        var Quadrature = TriangleQuadratures.GetQuadrature(scheme);
        double res = 0d;
        for(int i = 0; i < Quadrature.Weights.Length; ++i)
        {
            var pointL = Quadrature.LocalPoints[i];
            var w = Quadrature.Weights[i];
            var pointG = TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices, pointL);

            var value = CalcResultAtPointLocal(localSolution, pointL);
            res += w * (F(pointG) - value) * (F(pointG) - value);
        }
        var AbsDetD = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);

        return res * AbsDetD;
    }
}
