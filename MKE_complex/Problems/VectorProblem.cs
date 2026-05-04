using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.Matrix;
using MKE_complex.Matrix.SLAESolvers;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
using MKE_complex.Problems.Materials;
using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Vector;
using MKE_complex.Vector;
using Xunit.Internal;
using Xunit.Sdk;

namespace MKE_complex.Problems;
public class VectorProblem<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public IFiniteElementMesh<VectorT>? Mesh { get; private set; }

    private double[]? Solution { get; set; }

    // public double EvaluateDiscrepancy(ReadOnlySpan<VectorT> vertices, Func<VectorT,VectorT> u)
    // {
    //     double discrepancy = 0d;
    //     int n = 0;
    //     string format = "E3";
    //     string separator = "\t";
    //     foreach(var vertex in vertices)
    //     {
    //         double value = 0;
    //         if (CalculateFunctionAtPoint(vertex, out value))
    //         {
    //             Console.WriteLine($" {vertex.AsString(format, separator)}{separator}{value.ToString(format)}{separator}{u(vertex).ToString(format)}{separator}{Math.Abs(value - u(vertex)):E3}");
    //             ++n;
    //             discrepancy += Math.Abs(value - u(vertex)) * Math.Abs(value - u(vertex));
    //         }
    //         else
    //             Console.WriteLine($"{vertex.AsString(format, separator)}{separator}not found");
    //     }
        
    //     discrepancy = Math.Sqrt(discrepancy/n);
    //     return discrepancy;

    // }

    public bool CalculateFunctionAtPoint(VectorT point, out VectorT? value)
    {
        value = null;
        foreach (var element in Mesh!.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if (element.Geometry.IsPointInElement(point, vertices))
            {
                var localSolution = element.DOFs.Select(dof => Solution![dof]).ToArray();
                if(element is IFiniteElementVectorProblemCalculation<VectorT> vectorElem)
                    value = vectorElem.CalcResultAtPoint(vertices, localSolution, point);
                return true;
            }
        }
        return false;
    }

    private Dimension dimension => typeof(VectorT) switch
    {
        Type t when t == typeof(Vector1D) => Dimension.D1,
        Type t when t == typeof(Vector2D) => Dimension.D2,
        Type t when t == typeof(Vector3D) => Dimension.D3,
        _ => throw new Exception("Invalid vector type")
    };

    private GeometryType meshType;

    private BasisType basisType;

    private int basisOrder;

    public void InputUserDefinedData()
    {
        var GeometryTypesForDimension = new Dictionary<Dimension, GeometryType[]>()
        {
            {Dimension.D2, new GeometryType[] {GeometryType.Triangle,GeometryType.Quadrangle, GeometryType.Rectangle} },
            {Dimension.D3, new GeometryType[] {GeometryType.Hexahedron, GeometryType.Parallelepiped ,GeometryType.Tetrahedron} },
        };

        Console.WriteLine("Choose Mesh type");

        foreach (GeometryType g in GeometryTypesForDimension[dimension])
           Console.WriteLine($"{g} : {(int)g}");

        meshType = (GeometryType)int.Parse(Console.ReadLine()!);

        foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
           Console.WriteLine($"{b} : {(int)b}");

        basisType = (BasisType)int.Parse(Console.ReadLine()!);

        Console.WriteLine($"Choose basis order");

        basisOrder = int.Parse(Console.ReadLine()!);

        if (basisOrder < 1) throw new Exception();
    }
    public void Solve()
    {
        string directory = "MeshTest1";
        string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' ');
        fileNames = fileNames.Select(i => Path.Combine(directory,i)).ToArray();

        RegularParallelepipedMeshBuilder builder = new RegularParallelepipedMeshBuilder();

        Mesh = builder.BuildMesh<VectorT>(dimension, meshType, basisType, basisOrder, fileNames);

        var Materials = MaterialsReader.ReadMaterials<VectorT>("material2.json", PDE_Type.Elliptic, FieldType.Scalar, CoordinateSystem.Cartesian);

        var DirichletConditions = Mesh.Boundaries.ToArray().Where(i=>Materials[i.Material] is DirichletConditionForVectorEllipticProblem<VectorT>);

        DofsEnumerator.EnumerateMeshDofs(Mesh);
        
        HashSet<int> DirichletDofs = [];

        foreach(var condition in DirichletConditions) DirichletDofs.AddRange(condition.Geometry.VertexNumber);

        var SortedDirichletDofs = DirichletDofs.ToArray();
        Array.Sort(SortedDirichletDofs);

        if(Mesh.DofsCount is null) throw new Exception();
        int N = (int)Mesh.DofsCount;
        int[] newDofNumbers = Enumerable.Range(0, N).ToArray(); //originalDofNumberToNewNumeration

        int N0 = (int)(Mesh.DofsCount - SortedDirichletDofs.Length);

        newDofNumbers[SortedDirichletDofs[0]] = N0;
        for(int i = 1; i < SortedDirichletDofs.Length; ++i)
        {
            var prevDof = SortedDirichletDofs[i-1];
            var nextDof = SortedDirichletDofs[i];
            newDofNumbers[nextDof] = N0 + i;
            for(int j = prevDof + 1; j < nextDof; ++j)
                newDofNumbers[j] -= i;
        }

        
        foreach(var elem in Mesh.Elements)  //dofs renumeration For elements
        {
            if(elem is IFiniteElementVectorProblemCalculation<VectorT> vecElem)
                vecElem.SetDofs(elem.DOFs.Select(i=>newDofNumbers[i]).ToArray());
        }
        foreach(var boundary in Mesh.Boundaries)   //dofs renumeration for boundaries
        {
            if(boundary is IBoundaryConditionVectorEllipticProblemCalculation<VectorT> vecBound)
            {
                if(Materials[boundary.Material] is DirichletConditionForVectorEllipticProblem<VectorT>)
                    vecBound.SetDofs(boundary.DOFs.Select(i=>newDofNumbers[i]-N0).ToArray());
                else vecBound.SetDofs(boundary.DOFs.Select(i=>newDofNumbers[i]).ToArray());
            }    
        }
        //dirichlet boundaries SLAE evaluation
        var BoundariesMatrix = MatrixProfileBuilder.BuildBoundariesMatrixProfile<double, VectorT>(DirichletConditions.ToArray(),N-N0);
        var BoundariesRS = new Vector<double>(new double[N-N0]);

        foreach(var boundary in DirichletConditions)
        {
            var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if(Materials[boundary.Material] is DirichletConditionForVectorEllipticProblem<VectorT> dirichlet &&
               boundary is IBoundaryConditionVectorEllipticProblemCalculation<VectorT> boundaryCalc)
            {
                var localMatrix = boundaryCalc.CalcLocalMatrixForDirichletCondition(vertices);
                SLAEAssemblyAlgorhitms.AddLocalMatrix(BoundariesMatrix, localMatrix, boundary.DOFs, boundary.SortedDofIndices);
                var localRS = boundaryCalc.CalcLocalRightPart(vertices,dirichlet.Ag);
                SLAEAssemblyAlgorhitms.AddLocalRightPart(BoundariesRS,localRS,boundary.DOFs);
            }
        }

        var solver = new LOSSolver("LOS.txt");

        var BoundarySolution = solver.Solve(Preconditioning.None, BoundariesMatrix, BoundariesRS).components;

        //elements SLAE evaluation
        var ElementsMatrix = MatrixProfileBuilder.BuildMatrixProfile<double, VectorT>(Mesh, N0);
        var ElementsRs = new Vector<double>(new double[ElementsMatrix.N]);

        foreach (var element in Mesh.Elements)
        {
            if(element is IFiniteElementVectorProblemCalculation<VectorT> scalarElem)
            {
                var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();

                if(Materials[element.Material] is SolidMaterialForVectorEllipticProblem<VectorT> solidMaterial)
                {
                    var localMatrix = scalarElem.CalcLocalMatrix(vertices,
                                                              solidMaterial.Mu,
                                                             solidMaterial.Gamma);

                    SLAEAssemblyAlgorhitms.AddLocalMatrix(ElementsMatrix, localMatrix, element.DOFs, element.SortedDofIndices);

                    var localRightPart = scalarElem.CalcLocalRightPart(vertices,
                                                                    solidMaterial.F);

                    SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, element.DOFs);
                }
            }
            
        }
        foreach (var boundary in Mesh.Boundaries)
        {
            if(boundary is IBoundaryConditionScalarEllipticProblemCalculation<VectorT> scalarCondition)
            {
                var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
                var material = Materials[boundary.Material];

                if(material is NeumannConditionForScalarEllipticProblem<VectorT> neumannMetarial)
                {
                    var localRightPart = scalarCondition.CalcLocalRightPartForNeumannCondition(vertices, neumannMetarial.Theta);

                    SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, boundary.DOFs);
                }
                else if(material is RobinConditionForScalarEllipticProblem<VectorT> robinMaterial)
                {
                    var localMatrix = scalarCondition.CalcLocalMatrixForRobinCondition(vertices, robinMaterial.Beta);

                    SLAEAssemblyAlgorhitms.AddLocalMatrix(Matrix, localMatrix, boundary.DOFs, boundary.SortedDofIndices);

                    var localRightPart = scalarCondition.CalcLocalRightPartForRobinCondition(vertices, robinMaterial.Beta, robinMaterial.UBeta);

                    SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, boundary.DOFs);
                }
            }
        }

        foreach (var boundary in Mesh.Boundaries)
        {
            var material = Materials[boundary.Material];
            if (material is DirichletConditionForScalarEllipticProblem<VectorT> dirichletMaterial)
            {
                var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
                double[] localRightPart;
                if(boundary is IBoundaryConditionScalarEllipticProblemCalculation<VectorT> scalarCondition)
                    localRightPart = scalarCondition.CalcLocalRightPartForDirichletCondition(vertices, dirichletMaterial.Ug);
                else throw new ArgumentException();
                SLAEAssemblyAlgorhitms.ApplyDirichletConditions(Matrix,Pr, localRightPart, boundary.DOFs);
            }
        }

        var solver = new LOSSolver("LOS.txt");

        Solution = solver.Solve(Preconditioning.None, Matrix, Pr).components;

        Console.WriteLine("Done");
    }
}