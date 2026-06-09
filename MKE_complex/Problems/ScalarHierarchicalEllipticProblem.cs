using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.Matrix;
using MKE_complex.Matrix.SLAESolvers;
using MKE_complex.Mesh;
using MKE_complex.Problems.Materials;
using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;
using MKE_complex.Vector;
using Xunit.Internal;

namespace MKE_complex.Problems;
public class ScalarHierarchicalEllipticProblem<VectorT> where VectorT : VectorBase<double, VectorT>
{
    private Dictionary<string, IMaterial<VectorT>> Materials;
    public IFiniteElementMesh<VectorT>? Mesh { get;  set; }
    public double[]? Solution { get; set; }

    public void LoadMaterials(string folder, string file)
    {
        var materialsPath = Path.Join(folder, file);
        Materials = MaterialsReader.ReadMaterials<VectorT>(materialsPath, PDE_Type.Elliptic, FieldType.Scalar, CoordinateSystem.Cartesian);
    }

    public double EvaluateDiscrepancy(ReadOnlySpan<VectorT> vertices, Func<VectorT,double> u)
    {
        double discrepancy = 0d;
        int n = 0;
        string format = "E3";
        string separator = "\t";
        foreach(var vertex in vertices)
        {
            double value = 0;
            if (CalculateFunctionAtPoint(vertex, out value))
            {
                Console.WriteLine($" {vertex.AsString(format, separator)}{separator}{value.ToString(format)}{separator}{u(vertex).ToString(format)}{separator}{Math.Abs(value - u(vertex)):E3}");
                ++n;
                discrepancy += Math.Abs(value - u(vertex)) * Math.Abs(value - u(vertex));
            }
            else
                Console.WriteLine($"{vertex.AsString(format, separator)}{separator}not found");
        }
        
        discrepancy = Math.Sqrt(discrepancy/n);
        return discrepancy;

    }

    // public double EvaluateDiscrepancyGaussParallelepiped(Vector3D A, Vector3D B, Vector3D H, Func<Vector3D,double> u)
    // {
    //     double[] localPoints1D = [-Math.Sqrt(3d/5d), 0d, Math.Sqrt(3d/5d) ];
    //     double[] weights1D = [5d/9d,8d/9d,5d/9d];

    //     string format = "E3";
    //     string separator = "\t";

    //     Vector3D LocalPointToGlobal(Vector3D local, Vector3D a, Vector3D b)
    //     {
    //         var local01 = (local + new Vector3D(1d,1d,1d)) / 2d;
    //         Vector3D point = new(local01.X*(b.X-a.X) + a.X,
    //                              local01.Y*(b.Y-a.Y) + a.Y,
    //                              local01.Z*(b.Z-a.Z) + a.Z);
    //         return point;
    //     }

    //     Vector3D[] localPoints = new Vector3D[localPoints1D.Length * localPoints1D.Length * localPoints1D.Length];

    //     double[] weights = new double[weights1D.Length * weights1D.Length * weights1D.Length];

    //     for(int i = 0; i < localPoints1D.Length; ++i)
    //     {
    //         for(int j = 0; j < localPoints1D.Length; ++j)
    //         {
    //             for(int p = 0; p < localPoints1D.Length; ++p)
    //             {
    //                 localPoints[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = new(localPoints1D[p], 
    //                                                                                                             localPoints1D[j],
    //                                                                                                             localPoints1D[i]);
    //                 weights[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = weights1D[i]*
    //                                                                                                     weights1D[j]*
    //                                                                                                     weights1D[p];
    //             }
    //         }
    //     }

    //     int Nx = (int)((B.X - A.X) / H.X);
    //     int Ny = (int)((B.Y - A.Y) / H.Y);
    //     int Nz = (int)((B.Z - A.Z) / H.Z);

    //     H = new((B.X - A.X) / Nx, 
    //             (B.Y - A.Y) / Ny, 
    //             (B.Z - A.Z) / Nz);

    //     double discrepancy = 0;

    //     double[] discrepancies = new double[Nz];

        
    //     //for(int i = 0; i < Nz; ++i)
    //     Parallel.For(0, Nz, i =>
    //     {
    //         double Z = A.Z + H.Z * i;
    //         for(int j = 0; j < Ny; ++j)
    //         {
    //             double Y = A.Y + H.Y * j;
    //             for(int p = 0; p < Nx; ++p)
    //             {
    //                 double X = A.X + H.X * p;

    //                 Vector3D a = new(X,Y,Z);
    //                 Vector3D b = a + H;
    //                 double localDiscrepancy = 0d;
    //                 for(int q = 0; q < localPoints.Length; ++q)
    //                 {
    //                     var point = LocalPointToGlobal(localPoints[q],a,b);
    //                     //VectorT pointT;
    //                     var weight = weights[q];

    //                     if(point is VectorT pointT)
    //                     {
    //                         if (CalculateFunctionAtPoint(pointT, out double value))
    //                         {
    //                             //Console.WriteLine($" {pointT.AsString(format, separator)}{separator}{value.ToString(format)}{separator}{u(point).ToString(format)}{separator}{Math.Abs(value - u(point)):E3}");
    //                             localDiscrepancy += (value - u(point)) * (value - u(point)) * weight;
    //                         }
    //                     }
    //                 }

    //                 discrepancies[i] += localDiscrepancy;

    //                 //discrepancy += localDiscrepancy;
    //             }
    //         }
    //     });

    //     discrepancy = Math.Sqrt(discrepancies.Sum() * H.X * H.Y * H.Z / 8d);

    //     return discrepancy;
    // }

    public bool CalculateFunctionAtPoint(VectorT point, out double value)
    {
        value = 0d;
        foreach (var element in Mesh!.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if (element.Geometry.IsPointInElement(point, vertices))
            {
                var localSolution = element.DOFs.Select(dof => Solution![dof]).ToArray();
                if(element is IFiniteElementScalarEllipticProblemCalculation<VectorT> scalarElem)
                    value = scalarElem.CalcResultAtPoint(vertices, localSolution, point);
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

    // private GeometryType meshType;

    // private BasisType basisType;

    // private int basisOrder;

    
    public void Solve()
    {
        // var materialsfile = "materials5.json";
        // var materialsFolder = "TetrahedronHierarchical";
        // var materialsPath = Path.Join(materialsFolder,materialsfile);
        // var Materials = MaterialsReader.ReadMaterials<VectorT>(materialsPath, PDE_Type.Elliptic, FieldType.Scalar, CoordinateSystem.Cartesian);

        var DirichletConditions = Mesh!.Boundaries.ToArray().Where(i=>Materials[i.Material] is DirichletConditionForScalarEllipticProblem<VectorT>).ToArray();

        DofsEnumerator.EnumerateMeshDofs(Mesh);
        
        HashSet<int> DirichletDofs = [];

        foreach(var condition in DirichletConditions) DirichletDofs.AddRange(condition.DOFs);

        var SortedDirichletDofs = DirichletDofs.ToArray();
        Array.Sort(SortedDirichletDofs);

        if(Mesh.DofsCount is null) throw new Exception();
        int N = (int)Mesh.DofsCount;
        int[] newDofNumbers = [.. Enumerable.Range(0, N)]; //originalDofNumberToNewNumeration

        int N0 = (int)(Mesh.DofsCount - SortedDirichletDofs.Length);

        if(SortedDirichletDofs.Length >= 1)
            newDofNumbers[SortedDirichletDofs[0]] = N0;
        for(int i = 1; i < SortedDirichletDofs.Length; ++i)
        {
            var prevDof = SortedDirichletDofs[i-1];
            var nextDof = SortedDirichletDofs[i];
            newDofNumbers[nextDof] = N0 + i;
            for(int j = prevDof + 1; j < nextDof; ++j)
                newDofNumbers[j] -= i;
        }
        if(SortedDirichletDofs.Length >= 1)
        {
            for(int i = SortedDirichletDofs[^1] + 1; i < N; ++i)
                newDofNumbers[i] -= N - N0;
        }
        

        var RenumeratedElements = new IFiniteElement<VectorT>[Mesh.Elements.Length];
        for(int i = 0; i < RenumeratedElements.Length; ++i)  //dofs renumeration For elements
        {
            var elem = Mesh.Elements[i];
            var elemAttribute = (FiniteElementAttribute)elem.GetType().GetCustomAttributes(false).First(t => t is FiniteElementAttribute);
            RenumeratedElements[i] = FiniteElementsCreator.CreateFiniteElement(elemAttribute.GeometryType, elemAttribute.BasisType, elem.Order, elem.Material, elem.Geometry,
                                                                               [.. elem.DOFs.Select(i=>newDofNumbers[i])]);
        }
            
        // foreach(var elem in Mesh.Elements)  //dofs renumeration For elements
        // {
        //     // if(elem is IFiniteElementVectorProblemCalculation<VectorT> vecElem)
        //     //     vecElem.SetDofs(elem.DOFs.Select(i=>newDofNumbers[i]).ToArray());

        // }
        var RenumeratedBoundaries = new IBoundaryCondition<VectorT>[Mesh.Boundaries.Length]; 
        for(int i = 0; i < Mesh.Boundaries.Length; ++i) //dofs renumeration for boundaries
        {
            var boundary = Mesh.Boundaries[i];
            var elemAttribute = (FiniteElementAttribute)boundary.GetType().GetCustomAttributes(false).First(t => t is FiniteElementAttribute);
            RenumeratedBoundaries[i] = FiniteElementsCreator.CreateBoundaryCondition(elemAttribute.GeometryType, elemAttribute.BasisType, boundary.Order, boundary.Material, boundary.Geometry,
                                                                                     [.. boundary.DOFs.Select(i => newDofNumbers[i])]);
        }

        Mesh = new FiniteElementMesh<VectorT>(Mesh.Vertices.ToArray().ToList(), RenumeratedElements.ToList(), RenumeratedBoundaries.ToList()); //renumeratedMesh;
        Mesh.DofsCount = N;

        for(int i = 0; i < DirichletConditions.Length; ++i)
        {
            var boundary = DirichletConditions[i];
            var elemAttribute = (FiniteElementAttribute)boundary.GetType().GetCustomAttributes(false).First(t => t is FiniteElementAttribute);
            DirichletConditions[i] = FiniteElementsCreator.CreateBoundaryCondition(elemAttribute.GeometryType, elemAttribute.BasisType, boundary.Order, boundary.Material, boundary.Geometry,
                                                                                     [.. boundary.DOFs.Select(i => newDofNumbers[i] - N0)]);
        }

        
        //dirichlet boundaries SLAE evaluation
        var BoundariesMatrix = MatrixProfileBuilder.BuildBoundariesMatrixProfile<double, VectorT>(DirichletConditions.ToArray(),N-N0);
        var BoundariesRS = new Vector<double>(new double[N-N0]);

        foreach(var boundary in DirichletConditions)
        {
            var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if(Materials[boundary.Material] is DirichletConditionForScalarEllipticProblem<VectorT> dirichlet &&
               boundary is IBoundaryConditionScalarHierarchicalEllipticProblemCalculation<VectorT> boundaryCalc)
            {
                var localMatrix = boundaryCalc.CalcLocalMatrixForDirichletCondition(vertices);
                SLAEAssemblyAlgorhitms.AddLocalMatrix(BoundariesMatrix, localMatrix, boundary.DOFs, boundary.SortedDofIndices);
                var localRS = boundaryCalc.CalcLocalRightPartForDirichletCondition(vertices,dirichlet.Ug);
                SLAEAssemblyAlgorhitms.AddLocalRightPart(BoundariesRS,localRS,boundary.DOFs);
            }
        }

        var solver = new LOSSolver("LOS.txt");

        var BoundarySolution = solver.Solve(Preconditioning.Diagonal, BoundariesMatrix, BoundariesRS).components;

        var ElementsMatrix = MatrixProfileBuilder.BuildMatrixProfile<double, VectorT>(Mesh, N0);
        var ElementsRs = new Vector<double>(new double[ElementsMatrix.N]);

        foreach (var element in Mesh.Elements)
        {
            if(element is IFiniteElementScalarEllipticProblemCalculation<VectorT> scalarElem)
            {
                var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();

                if(Materials[element.Material] is SolidMaterialForScalarEllipticProblem<VectorT> solidMaterial)
                {
                    var localMatrix = scalarElem.CalcLocalMatrix(vertices,
                                                                 solidMaterial.Lambda,
                                                                 solidMaterial.Gamma);

                    SLAEAssemblyAlgorhitms.AddLocalMatrixVectorFEM(ElementsMatrix, localMatrix, element.DOFs, element.SortedDofIndices,ElementsRs,BoundarySolution);

                    var localRightPart = scalarElem.CalcLocalRightPart(vertices,
                                                                       solidMaterial.F);

                    SLAEAssemblyAlgorhitms.AddLocalRightPartVectorFEM(ElementsRs, localRightPart, element.DOFs);
                }
            }
            
        }
        foreach (var boundary in Mesh.Boundaries) //for neumann conditions
        {
            if(boundary is IBoundaryConditionScalarEllipticProblemCalculation<VectorT> scalarCondition)
            {
                var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
                var material = Materials[boundary.Material];

                if(material is NeumannConditionForScalarEllipticProblem<VectorT> neumannMetarial)
                {
                    var localRightPart = scalarCondition.CalcLocalRightPartForNeumannCondition(vertices, neumannMetarial.Theta);

                    SLAEAssemblyAlgorhitms.AddLocalRightPartVectorFEM(ElementsRs, localRightPart, boundary.DOFs);
                }
                else if(material is RobinConditionForScalarEllipticProblem<VectorT> robinMaterial)
                {
                    var localMatrix = scalarCondition.CalcLocalMatrixForRobinCondition(vertices, robinMaterial.Beta);

                    SLAEAssemblyAlgorhitms.AddLocalMatrixVectorFEM(ElementsMatrix, localMatrix, boundary.DOFs, boundary.SortedDofIndices, ElementsRs, BoundarySolution);

                    var localRightPart = scalarCondition.CalcLocalRightPartForRobinCondition(vertices, robinMaterial.Beta, robinMaterial.UBeta);

                    SLAEAssemblyAlgorhitms.AddLocalRightPartVectorFEM(ElementsRs, localRightPart, boundary.DOFs);
                }
            }
        }

        var ElementsSolution = solver.Solve(Preconditioning.Diagonal, ElementsMatrix, ElementsRs).components;

        Solution = [.. ElementsSolution, .. BoundarySolution];

        Console.WriteLine("Done");
    }
}