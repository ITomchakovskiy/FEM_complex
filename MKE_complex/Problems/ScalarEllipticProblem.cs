using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.RectangleElements;
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
    private double[] Solution { get; set; }

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
                {
                    Console.WriteLine($"{v2[i].X:F1}    {v2[i].Y:F1}     not found");
                }
            }
            
            discrepancy = Math.Sqrt(discrepancy/n);

            return discrepancy;
        }
        return 0d;
    }

    public bool CalculateFunctionAtPoint(VectorT point, out double value)
    {
        value = 0d;
        foreach (var element in Mesh.Elements)
        {
            var vertices = element.Geometry.VertexNumber.Select(i => Mesh.Vertices[i]).ToArray();
            if (element.Geometry.IsPointInElement(point, vertices))
            {
                var localSolution = element.DOFs.Select(dof => Solution[dof]).ToArray();
                value = element.CalcResultAtPoint(vertices, localSolution, point);
                return true;
            }
        }
        return false;
    }
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
        int order = 2;

        if (order < 1) throw new Exception();

        //Console.WriteLine("Type file names for mesh building");

        string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' '); 

        PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();



        //Mesh = builder.BuildMesh<VectorT>(dimension, mesh_type, basis, order, fileNames);

        Vector2D[] Vertices_ = [new(1d, 0.5d), new(2d, 0.5d), new(3d, 0.5d), new(5d, 0.5d), new(7d, 0.5d), new(9d, 0.5d) ,
                                new(1d, 2d),   new(2d, 2d),   new(3d, 2d),   new(5d, 2d),   new(7d, 2d),   new(9d, 2d),
                                new(1d, 3d),   new(2d, 4d),   new(3d, 4d),   new(7d, 2.5),  new(9d, 3d)];

        IFiniteElement<Vector2D>[] Elements_ = [new RectangleLagrangianQuadraticFiniteElement("1", new([0, 6, 7, 1])),
                                                new RectangleLagrangianQuadraticFiniteElement("1", new([1, 7, 8, 2])),
                                                new RectangleLagrangianQuadraticFiniteElement("1", new([2, 8, 9, 3])),
                                                new RectangleLagrangianQuadraticFiniteElement("1", new([3, 9, 10, 4])),
                                                new RectangleLagrangianQuadraticFiniteElement("1", new([4, 10, 11, 5])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([6, 12, 7])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([12, 13, 7])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([7, 13, 8])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([8, 13, 14])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([10, 15, 11])),
                                                new TriangleLagrangianQuadraticFiniteElement("1", new([15, 16, 11])),];

        IBoundaryCondition<Vector2D>[] Edges_ = [new LagrangianQuadraticEdgeCondition("0", "21", new([0, 6])),
                                                  new LagrangianQuadraticEdgeCondition("0", "21", new([6, 12])),
                                                  new LagrangianQuadraticEdgeCondition("0", "1", new([12, 13])),
                                                  new LagrangianQuadraticEdgeCondition("0", "22", new([13, 14])),
                                                  new LagrangianQuadraticEdgeCondition("0", "23", new([14, 8])),
                                                  new LagrangianQuadraticEdgeCondition("0", "22", new([8, 9])),
                                                  new LagrangianQuadraticEdgeCondition("0", "22", new([9, 10])),
                                                  new LagrangianQuadraticEdgeCondition("0", "21", new([10, 15])),
                                                  new LagrangianQuadraticEdgeCondition("0", "1", new([15, 16])),
                                                  new LagrangianQuadraticEdgeCondition("0", "23", new([16, 11])),
                                                  new LagrangianQuadraticEdgeCondition("0", "23", new([11, 5])),
                                                  new LagrangianQuadraticEdgeCondition("0", "24", new([5, 4])),
                                                  new LagrangianQuadraticEdgeCondition("0", "24", new([4, 3])),
                                                  new LagrangianQuadraticEdgeCondition("0", "24", new([3, 2])),
                                                  new LagrangianQuadraticEdgeCondition("0", "24", new([2, 1])),
                                                  new LagrangianQuadraticEdgeCondition("0", "24", new([1, 0])),];
        //Mesh = (IFiniteElementMesh<VectorT>)(object)new FiniteElementMesh<Vector2D>(Vertices_.ToList(), Elements_.ToList(), Edges_.ToList());



        //Vector2D[] Vertices_ = [new(2d, 0d), new(2d, 1d), new(3d, 1d), new(2d, 4d), new(7d, 4d)];

        //IFiniteElement<Vector2D>[] Elements_ = [new TriangleLagrangianQuadraticFiniteElement("1", new([0,1,2])),
        //                                                     new TriangleLagrangianQuadraticFiniteElement("2", new([4,2,3])),
        //                                                     new TriangleLagrangianQuadraticFiniteElement("2", new([3,2,1]))];
        //IBoundaryCondition<Vector2D>[] Edges_ = [new LagrangianQuadraticEdgeCondition("0", "11", new([0, 2])),
        //                                          new  LagrangianQuadraticEdgeCondition("0", "21", new([4, 3])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "22", new([0, 1])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "22", new([3, 1])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "31", new([4, 2]))];
        //Mesh = (IFiniteElementMesh<VectorT>)(object)new FiniteElementMesh<Vector2D>(Vertices_.ToList(), Elements_.ToList(), Edges_.ToList());

        //Vector2D[] Vertices_ = [new(1d, 5d), new(1.5d, 5d), new(4d, 5d),
        //                        new(1d, 6d), new(1.5d, 6d), new(4d, 6d),
        //                         new(1d, 8.5d), new(4d, 8.5d),
        //                         new(1d, 10d), new(4d, 10d),];

        //IFiniteElement<Vector2D>[] Elements_ = [new TriangleLagrangianQuadraticFiniteElement("1", new([0,4,1])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("1", new([0,3,4])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("1", new([1,4,5])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("1", new([1,5,2])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("2", new([3,6,4])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("2", new([6,7,4])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("2", new([4,7,5])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("2", new([6,9,7])),
        //                                        new TriangleLagrangianQuadraticFiniteElement("2", new([6,8,9]))];
        //IBoundaryCondition<Vector2D>[] Edges_ = [new LagrangianQuadraticEdgeCondition("0", "11", new([2, 5])),
        //                                          new  LagrangianQuadraticEdgeCondition("0", "21", new([0, 1])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "21", new([1, 2])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "22", new([0, 3])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "22", new([3, 6])),
        //                                          new  LagrangianQuadraticEdgeCondition("0", "22", new([6, 8])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "23", new([8, 9])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "31", new([5, 7])),
        //                                          new LagrangianQuadraticEdgeCondition("0", "31", new([7, 9])),
    //                                       ];
        Mesh = (IFiniteElementMesh<VectorT>)(object)new FiniteElementMesh<Vector2D>(Vertices_.ToList(), Elements_.ToList(), Edges_.ToList());

        int refinement = 6;

        for (int i = 0; i < refinement; ++i)
            Mesh = Mesh.Refine();

        Mesh = Mesh.Triangulate();

        DofsEnumerator.EnumerateMeshDofs(Mesh);

        var Matrix = MatrixProfileBuilder.BuildMatrixProfile<double, VectorT>(Mesh);
        var Pr = new Vector.Vector<double>(new double[Matrix.N]);

        //if (Mesh is FiniteElementMesh<Vector2D> mesh2d)
        //    mesh2d.SaveMeshGeometry("input_points", "input_triangles", "input_dofs", "input_edges", "input_edgeDofs");

        Dictionary<string, SolidMaterialForScalarEllipticProblem<VectorT>> solidMaterials = new Dictionary<string, SolidMaterialForScalarEllipticProblem<VectorT>>()
        {
            //{"1", new("1", "2","1","2 * x^2 + 3* y^2 + 6*x*y - 20", CoordinateSystem.Cartesian) },
            //{"1", new("1", "1","0","-6*(x+y)", CoordinateSystem.Cartesian) },
            //{"1", new("1", "1","0","2 * cos(x) * cos(y)", CoordinateSystem.Cartesian) },
            {"1", new("1", "1","1","-(1.0/(x*y) + 2.0*x*log(x)/y^3) + x/y*log(x)", CoordinateSystem.Cartesian) },
        };

        Dictionary<string, IMaterial<VectorT>> boundaryMaterials = new Dictionary<string, IMaterial<VectorT>>()
        {

            //{"1",  new DirichletConditionForScalarEllipticProblem<VectorT>("11","2 * x^2 + 3* y^2 + 6*x*y",CoordinateSystem.Cartesian)},
            //{"21", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-(8*x + 12*y)",CoordinateSystem.Cartesian) },
            //{"22", new NeumannConditionForScalarEllipticProblem<VectorT>("21","12 * (x + y)",CoordinateSystem.Cartesian) },
            //{"23", new NeumannConditionForScalarEllipticProblem<VectorT>("21","8*x + 12*y",CoordinateSystem.Cartesian) },
            //{"24", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-12 * (x + y)",CoordinateSystem.Cartesian) },

            //{"1",  new DirichletConditionForScalarEllipticProblem<VectorT>("11","x^3 + y^3",CoordinateSystem.Cartesian)},
            //{"21", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-3*x^2",CoordinateSystem.Cartesian) },
            //{"22", new NeumannConditionForScalarEllipticProblem<VectorT>("21","3*y^2",CoordinateSystem.Cartesian) },
            //{"23", new NeumannConditionForScalarEllipticProblem<VectorT>("21","3*x^2",CoordinateSystem.Cartesian) },
            //{"24", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-3*y^2",CoordinateSystem.Cartesian) },

            //{"1",  new DirichletConditionForScalarEllipticProblem<VectorT>("11","cos(x) * cos(y)",CoordinateSystem.Cartesian)},
            //{"21", new NeumannConditionForScalarEllipticProblem<VectorT>("21","sin(x)*cos(y)",CoordinateSystem.Cartesian) },
            //{"22", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-cos(x)*sin(y)",CoordinateSystem.Cartesian) },
            //{"23", new NeumannConditionForScalarEllipticProblem<VectorT>("21","-sin(x)*cos(y)",CoordinateSystem.Cartesian) },
            //{"24", new NeumannConditionForScalarEllipticProblem<VectorT>("21","cos(x)*sin(y)",CoordinateSystem.Cartesian) },

            {"1",  new DirichletConditionForScalarEllipticProblem<VectorT>("11","x/y*log(x)",CoordinateSystem.Cartesian)},
            {"21", new DirichletConditionForScalarEllipticProblem<VectorT>("21","x/y*log(x)",CoordinateSystem.Cartesian) },
            {"22", new DirichletConditionForScalarEllipticProblem<VectorT>("21","x/y*log(x)",CoordinateSystem.Cartesian) },
            {"23", new DirichletConditionForScalarEllipticProblem<VectorT>("21","x/y*log(x)",CoordinateSystem.Cartesian) },
            {"24", new DirichletConditionForScalarEllipticProblem<VectorT>("21","x/y*log(x)",CoordinateSystem.Cartesian) },
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
