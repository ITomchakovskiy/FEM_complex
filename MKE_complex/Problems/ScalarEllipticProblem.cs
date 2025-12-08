using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.Matrix;
using MKE_complex.Matrix.SLAESolvers;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
using MKE_complex.Problems.Materials;
using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MKE_complex.Problems;

public class ScalarEllipticProblem<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public IFiniteElementMesh<VectorT> Mesh { get; private set; }
    public double[] Solution { get; private set; }

    public void Solve()
    {
        var GeometryTypesForDimension = new Dictionary<Dimension, GeometryType[]>()
{
    {Dimension.D2, new GeometryType[] {GeometryType.Triangle,GeometryType.Quadrangle} },
    {Dimension.D3, new GeometryType[] {GeometryType.Hexagon,GeometryType.Tetrahedron} },
};

        Console.WriteLine("Choose dimension");

        //foreach(Dimension d in Enum.GetValues(typeof(Dimension)))
        //{
        //    Console.WriteLine($"{d} : {(int)d}");
        //}
        Dimension dimension = Dimension.D2; //(Dimension)int.Parse(Console.ReadLine()!);

        Console.WriteLine("Choose Mesh type");

        //foreach (GeometryType g in GeometryTypesForDimension[dimension])
        //{
        //    Console.WriteLine($"{g} : {(int)g}");
        //}

        GeometryType mesh_type = GeometryType.Triangle;      //(GeometryType)int.Parse(Console.ReadLine()!);

        //foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
        //{
        //    Console.WriteLine($"{b} : {(int)b}");
        //}

        BasisType basis = BasisType.Lagrangian; //(BasisType)int.Parse(Console.ReadLine()!);

        //Console.WriteLine($"Choose basis order");

        //int order = 3; //int.Parse(Console.ReadLine()!);
        int order = 1;

        if (order < 1) throw new Exception();

        //Console.WriteLine("Type file names for mesh building");

        string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' '); 

        PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

        //Mesh = builder.BuildMesh<VectorT>(dimension, mesh_type, basis, order, fileNames); //костыль

        Vector2D[] Vertices_ = [new(2d, 0d), new(2d, 1d), new(3d, 1d), new(2d, 4d), new(7d, 4d)];

        IFiniteElement<Vector2D>[] Elements_ = [new TriangleLagrangianLinearFiniteElement("1", new([0,1,2])),
                                                             new TriangleLagrangianLinearFiniteElement("2", new([4,2,3])),
                                                             new TriangleLagrangianLinearFiniteElement("2", new([3,2,1]))];
        IBoundaryCondition<Vector2D>[] Edges_ = [new LagrangianLinearEdgeCondition("0", "11", new([0, 2])),
                                                  new  LagrangianLinearEdgeCondition("0", "21", new([4, 3])),
                                                  new LagrangianLinearEdgeCondition("0", "22", new([0, 1])),
                                                  new LagrangianLinearEdgeCondition("0", "22", new([3, 1])),
                                                  new LagrangianLinearEdgeCondition("0", "31", new([4, 2]))];
        Mesh = (IFiniteElementMesh<VectorT>)(object)new FiniteElementMesh<Vector2D>(Vertices_.ToList(), Elements_.ToList(), Edges_.ToList());

        DofsEnumerator.EnumerateMeshDofs(Mesh);

        var Matrix = MatrixProfileBuilder.BuildMatrixProfile<double, VectorT>(Mesh);
        var Pr = new Vector.Vector<double>(new double[Matrix.N]);

        if (Mesh is FiniteElementMesh<Vector2D> mesh2d)
            mesh2d.SaveMeshGeometry("input_points", "input_triangles", "input_dofs", "input_edges", "input_edgeDofs");

        Dictionary<string, SolidMaterialForScalarEllipticProblem<VectorT>> solidMaterials = new Dictionary<string, SolidMaterialForScalarEllipticProblem<VectorT>>()
        {
            {"1", new("1", "10","0","-20", CoordinateSystem.Cartesian) },
            {"2", new("2","1","0","0",CoordinateSystem.Cartesian)}
        };

        Dictionary<string, IMaterial<VectorT>> boundaryMaterials = new Dictionary<string, IMaterial<VectorT>>()
        {
            {"11",  new DirichletConditionForScalarEllipticProblem<VectorT>("11","y^2",CoordinateSystem.Cartesian)},
            {"21", new NeumannConditionForScalarEllipticProblem<VectorT>("21","20",CoordinateSystem.Cartesian) },
            {"22", new NeumannConditionForScalarEllipticProblem<VectorT>("22","0",CoordinateSystem.Cartesian) },
            {"31", new RobinConditionForScalarEllipticProblem<VectorT>("31","2","20*y - 27",CoordinateSystem.Cartesian) }
        };

        //Dictionary<string, DirichletConditionForScalarEllipticProblem<VectorT>> dirichletConditions = [];
        //Dictionary<string, NeumannConditionForScalarEllipticProblem<VectorT>> neumannConditions = [];
        //Dictionary<string, RobinConditionForScalarEllipticProblem<VectorT>> robinConditions = [];

        foreach (var element in Mesh.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            var localMatrix = element.CalcLocalMatrix(vertices,
                                                      solidMaterials[element.Material].Lambda,
                                                      solidMaterials[element.Material].Gamma);
            SLAEAssemblyAlgorhitms.AddLocalMatrix(Matrix, localMatrix, element.DOFs, element.SortedDofIndices);

            var localRightPart = element.CalcLocalRightPart(vertices,
                                                            solidMaterials[element.Material].F);

            SLAEAssemblyAlgorhitms.AddLocalRightPart(Pr, localRightPart, element.DOFs);
        }
        foreach (var boundary in Mesh.Boundaries)
        {
            var vertices = boundary.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            var material = boundaryMaterials[boundary.EdgeMaterial];

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
            var material = boundaryMaterials[boundary.EdgeMaterial];
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

        ////
        //Console.WriteLine(new SpecificMaterials().Iron.Lambda(new Vector2D(0d, 0d)));
        //Console.WriteLine(new SpecificMaterials().Iron.Gamma(new Vector2D(0d, 0d)));
        //Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 0d)));
        //Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(1d, 0d)));
        //Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 2d)));
    }
}
