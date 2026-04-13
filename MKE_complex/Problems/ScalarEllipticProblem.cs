using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.DofsEnumerators;
using MKE_complex.Matrix;
using MKE_complex.Matrix.SLAESolvers;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
using MKE_complex.Problems.Materials;
using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;
using MKE_complex.Vector;

namespace MKE_complex.Problems;
public class ScalarEllipticProblem<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public IFiniteElementMesh<VectorT>? Mesh { get; private set; }
    private double[]? Solution { get; set; }

    public double EvaluateDiscrepancy(VectorT[] vertices, Func<VectorT,double> u)
    {
        if(vertices is Vector2D[] v2)
        {
            double discrepancy = 0d;
            int n = 0;
            for(int i = 0; i < v2.Length;++i)
            {
                double value = 0;
                if (CalculateFunctionAtPoint(vertices[i], out value))
                {
                    Console.WriteLine($"{v2[i].X:F1}    {v2[i].Y:F2}     {value}      {u(vertices[i])}     {Math.Abs(value - u(vertices[i])):E3}");
                    ++n;
                    discrepancy += Math.Abs(value - u(vertices[i])) * Math.Abs(value - u(vertices[i]));
                }
                else
                    Console.WriteLine($"{v2[i].X:F1}    {v2[i].Y:F1}     not found");
            }
            
            discrepancy = Math.Sqrt(discrepancy/n);

            return discrepancy;
        }
        return 0d;
    }

    public bool CalculateFunctionAtPoint(VectorT point, out double value)
    {
        value = 0d;
        foreach (var element in Mesh!.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if (element.Geometry.IsPointInElement(point, vertices))
            {
                var localSolution = element.DOFs.Select(dof => Solution![dof]).ToArray();
                value = element.CalcResultAtPoint(vertices, localSolution, point);
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
            {Dimension.D3, new GeometryType[] {GeometryType.Hexagon,GeometryType.Tetrahedron} },
        };

        Console.WriteLine("Choose Mesh type");

        foreach (GeometryType g in GeometryTypesForDimension[dimension])
           Console.WriteLine($"{g} : {(int)g}");

        meshType = (GeometryType)int.Parse(Console.ReadLine()!);

        foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
           Console.WriteLine($"{b} : {(int)b}");

        BasisType basis = (BasisType)int.Parse(Console.ReadLine()!);

        Console.WriteLine($"Choose basis order");

        basisOrder = int.Parse(Console.ReadLine()!);
        //int order = 2;

        if (basisOrder < 1) throw new Exception();
    }
    public void Solve()
    {
        var GeometryTypesForDimension = new Dictionary<Dimension, GeometryType[]>()
        {
            {Dimension.D2, new GeometryType[] {GeometryType.Triangle,GeometryType.Quadrangle, GeometryType.Rectangle} },
            {Dimension.D3, new GeometryType[] {GeometryType.Hexagon,GeometryType.Tetrahedron} },
        };

        

        //Console.WriteLine("Type file names for mesh building");

        string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' '); 

        PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

        Mesh = builder.BuildMesh<VectorT>(dimension, mesh_type, basis, order, fileNames);

        var Materials = MaterialsReader.ReadMaterials<VectorT>("material1.json", PDE_Type.Elliptic, FieldType.Scalar, CoordinateSystem.Cartesian);

        DofsEnumerator.EnumerateMeshDofs(Mesh);

        var Matrix = MatrixProfileBuilder.BuildMatrixProfile<double, VectorT>(Mesh);
        var Pr = new Vector.Vector<double>(new double[Matrix.N]);

        foreach (var element in Mesh.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();

            if(Materials[element.Material] is SolidMaterialForScalarEllipticProblem<VectorT> solidMaterial)
            {
                var localMatrix = element.CalcLocalMatrix(vertices,
                                                          solidMaterial.Lambda,
                                                         solidMaterial.Gamma);

                SLAEAssemblyAlgorhitms.AddLocalMatrix(Matrix, localMatrix, element.DOFs, element.SortedDofIndices);

                var localRightPart = element.CalcLocalRightPart(vertices,
                                                                solidMaterial.F);

                SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, element.DOFs);
            }
        }
        foreach (var boundary in Mesh.Boundaries)
        {
            var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            var material = Materials[boundary.Material];

            if(material is NeumannConditionForScalarEllipticProblem<VectorT> neumannMetarial)
            {
                var localRightPart = boundary.CalcLocalRightPartForNeumannCondition(vertices, neumannMetarial.Theta);

                SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, boundary.DOFs);
            }
            else if(material is RobinConditionForScalarEllipticProblem<VectorT> robinMaterial)
            {
                var localMatrix = boundary.CalcLocalMatrixForRobinCondition(vertices, robinMaterial.Beta);

                SLAEAssemblyAlgorhitms.AddLocalMatrix(Matrix, localMatrix, boundary.DOFs, boundary.SortedDofIndices);

                var localRightPart = boundary.CalcLocalRightPartForRobinCondition(vertices, robinMaterial.Beta, robinMaterial.UBeta);

                SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, boundary.DOFs);
            }
        }

        foreach (var boundary in Mesh.Boundaries)
        {
            var material = Materials[boundary.Material];
            if (material is DirichletConditionForScalarEllipticProblem<VectorT> dirichletMaterial)
            {
                 var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
                 var localRightPart = boundary.CalcLocalRightPartForDirichletCondition(vertices, dirichletMaterial.Ug);
                 SLAEAssemblyAlgorhitms.ApplyDirichletConditions(Matrix,Pr, localRightPart, boundary.DOFs);
            }
        }

        var solver = new LOSSolver("LOS.txt");

        Solution = solver.Solve(Preconditioning.None, Matrix, Pr).components;

        Console.WriteLine("Done");
    }
}