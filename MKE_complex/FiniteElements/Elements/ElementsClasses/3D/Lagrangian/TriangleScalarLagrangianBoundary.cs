using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Lagrangian;
[FiniteElement(GeometryType.TriangleBoundary,BasisType.Lagrangian)]
public class TriangleScalarLagrangianBoundary : IBoundaryCondition3D, IBoundaryConditionScalarEllipticProblemCalculation<Vector3D>
{
    public TriangleScalarLagrangianBoundary(string material, TriangleBoundary geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        this.geometry = geometry;
        Order = order;
        DOFs = new int[6];         //hardcode

        sortedDofIndices = new Lazy<int[]>(()=>
        {
            var dofs = DOFs.ToArray();
            var indices = Enumerable.Range(0,DOFs.Length).ToArray();
            Array.Sort(dofs, indices);
            return indices;
        });
    }

    private TriangleBoundary geometry;
    public IFiniteElementGeometry3D Geometry => geometry;

    public int DofsOnFaceCount => 0; //hardcode

    public string Material {get;}

    public int Order {get;}

    public int[] DOFs {get; private set;}

    public int[] SortedDofs => [.. SortedDofIndices.Select(i => DOFs[i])];

    private Lazy<int[]> sortedDofIndices;

    public int[] SortedDofIndices => sortedDofIndices.Value;

    public int DofsOnEdgeCount => 1; //hardcode

    public int DofsOnVertexCount => 1; //hardcode

    IFiniteElementGeometry<Vector3D> IBoundaryCondition<Vector3D>.Geometry => Geometry;

    public IBoundaryCondition<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices)
    {
        var geometries = Geometry.Refine(FaceVertices, EdgeVertices, 0, out bool flag);

        return [.. geometries.Select(g => new TriangleScalarLagrangianBoundary(Material, (TriangleBoundary)g, Order))];
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        DOFs[3 + localEdgeNumber] = dofNumber;
    }

    public void SetFaceDofs(int[] baseVerices, int dofNumber)
    {
        ;
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        for(int i = 0; i < 3; ++i)
            DOFs[i] = dofsNumbers[i];
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        DOFs[localVertexNumber] = dofNumber;
    }

    private double[][] LagrangianVerticesAtDofs()
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

    public double[][] CalcLocalMatrixForRobinCondition(Vector3D[] vertices, Func<Vector3D, double> Beta)
    {
        throw new NotImplementedException();
    }

    public double[] CalcLocalRightPartForNeumannCondition(Vector3D[] vertices, Func<Vector3D, double> Theta)
    {
        throw new NotImplementedException();
    }

    public double[] CalcLocalRightPartForRobinCondition(Vector3D[] vertices, Func<Vector3D, double> Beta, Func<Vector3D, double> UBeta)
    {
        throw new NotImplementedException();
    }

    public double[] CalcLocalRightPartForDirichletCondition(Vector3D[] vertices, Func<Vector3D, double> Ug)
    {
        var GlobalVertices = LagrangianVerticesAtDofs().Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices, i));

        return [.. GlobalVertices.Select(Ug)];
    }
}